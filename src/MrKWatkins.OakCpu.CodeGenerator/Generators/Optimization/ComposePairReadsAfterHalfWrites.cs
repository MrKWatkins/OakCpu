using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using Parameter = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Parameter;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Optimization;

// Rewrites 16-bit reads of a register pair as a composition of its half-registers, e.g. emulator.PC = emulator.WZ
// as emulator.PC = (ushort)(emulator.W << 8 | emulator.Z), when one of the halves was byte-written shortly before
// in the same method. A 16-bit load that overlaps an in-flight byte store cannot be store-to-load forwarded and
// stalls until the store buffer drains, whereas byte loads after byte stores forward at full speed.
//
// When the half was written from a simple source, e.g. W = Data, and nothing has invalidated the source since (no
// call has run and the source has not been reassigned), the composition forwards the source value instead of
// re-reading the half, e.g. emulator.PC = (ushort)(emulator.data << 8 | emulator.Z), avoiding the byte reload
// entirely.
//
// The rewrite is always semantically identical because the halves overlay the pair, so the state tracking below
// only decides profitability: reads are rewritten while the half-store can still plausibly be in the store buffer,
// i.e. at most MaxCallsSinceHalfWrite calls after it, and not once the pair has subsequently been written whole.
internal sealed class ComposePairReadsAfterHalfWrites : MethodBodyOptimization
{
    private const int MaxCallsSinceHalfWrite = 1;

    protected override IReadOnlyList<StatementSyntax> Optimize(FileGeneratorContext context, IReadOnlyList<StatementSyntax> statements)
    {
        var pairs = CreatePairsByFieldName(context.GeneratorContext.Configuration);
        var state = new State();
        return OptimizeStatements(pairs, state, statements);
    }

    // Maps the field name of each 16-bit register pair and of both its halves to the pair register.
    [Pure]
    private static IReadOnlyDictionary<string, Register> CreatePairsByFieldName(Configuration configuration) =>
        configuration.Registers.Values
            .Where(register => register is { Type: DataType.U16, HighRegister: not null, LowRegister: not null })
            .SelectMany(pair => new[] { (pair.FieldName, pair), (pair.HighRegister!.FieldName, pair), (pair.LowRegister!.FieldName, pair) })
            .ToDictionary(entry => entry.FieldName, entry => entry.pair, StringComparer.Ordinal);

    [MustUseReturnValue]
    private static IReadOnlyList<StatementSyntax> OptimizeStatements(IReadOnlyDictionary<string, Register> pairs, State state, IReadOnlyList<StatementSyntax> statements) =>
        statements.Select(statement => OptimizeStatement(pairs, state, statement)).ToList();

    [MustUseReturnValue]
    private static StatementSyntax OptimizeStatement(IReadOnlyDictionary<string, Register> pairs, State state, StatementSyntax statement)
    {
        switch (statement)
        {
            case BlockSyntax block:
                return block.WithStatements(List(OptimizeStatements(pairs, state, block.Statements)));

            case IfStatementSyntax ifStatement:
                return OptimizeIfStatement(pairs, state, ifStatement);

            case ExpressionStatementSyntax { Expression: AssignmentExpressionSyntax assignment } expressionStatement:
                return OptimizeAssignment(pairs, state, expressionStatement, assignment);

            default:
                {
                    var optimized = (StatementSyntax)RewriteReads(pairs, state, statement);
                    state.OnCalls(optimized);
                    return optimized;
                }
        }
    }

    [MustUseReturnValue]
    private static StatementSyntax OptimizeAssignment(IReadOnlyDictionary<string, Register> pairs, State state, ExpressionStatementSyntax statement, AssignmentExpressionSyntax assignment)
    {
        var newRight = (ExpressionSyntax)RewriteReads(pairs, state, assignment.Right);

        StatementSyntax optimized;
        if (assignment.Kind() is SyntaxKind.AddAssignmentExpression or SyntaxKind.SubtractAssignmentExpression &&
            TryGetEmulatorField(assignment.Left, out var fieldName) &&
            pairs.TryGetValue(fieldName, out var pair) &&
            pair.FieldName == fieldName &&
            state.IsComposable(pair))
        {
            // A compound assignment reads the pair too, so e.g. emulator.WZ += 1 becomes
            // emulator.WZ = (ushort)((emulator.W << 8 | emulator.Z) + 1).
            var operatorKind = assignment.Kind() == SyntaxKind.AddAssignmentExpression ? SyntaxKind.AddExpression : SyntaxKind.SubtractExpression;
            var value = BinaryExpression(operatorKind, ParenthesizedExpression(state.CreateCompose(pair)), newRight);
            optimized = statement.WithExpression(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    assignment.Left,
                    CastExpression(UShortType, ParenthesizedExpression(value))));
        }
        else
        {
            optimized = statement.WithExpression(assignment.WithRight(newRight));
        }

        state.OnWrite(pairs, assignment.Left, assignment.Kind() == SyntaxKind.SimpleAssignmentExpression ? newRight : null);
        state.OnCalls(optimized);
        return optimized;
    }

    [MustUseReturnValue]
    private static StatementSyntax OptimizeIfStatement(IReadOnlyDictionary<string, Register> pairs, State state, IfStatementSyntax ifStatement)
    {
        var newCondition = (ExpressionSyntax)RewriteReads(pairs, state, ifStatement.Condition);
        state.OnCalls(newCondition);

        var newStatement = OptimizeBranch(pairs, state, ifStatement.Statement);
        var newElse = ifStatement.Else?.WithStatement(OptimizeBranch(pairs, state, ifStatement.Else.Statement));

        return ifStatement.WithCondition(newCondition).WithStatement(newStatement).WithElse(newElse);
    }

    // Optimizes a branch with a copy of the current state. A branch that terminates in a return or throw contributes
    // nothing on the fall-through path so the state is left untouched; otherwise the state is conservatively merged
    // to pairs that stayed active through the branch, at the worse of the two call counts and only keeping forwarded
    // values both sides agree on.
    [MustUseReturnValue]
    private static StatementSyntax OptimizeBranch(IReadOnlyDictionary<string, Register> pairs, State state, StatementSyntax branch)
    {
        var branchState = state.Fork();
        var optimized = OptimizeStatement(pairs, branchState, branch);
        if (!Terminates(branch))
        {
            state.Merge(branchState);
        }

        return optimized;
    }

    [Pure]
    private static bool Terminates(StatementSyntax statement) =>
        statement switch
        {
            BlockSyntax block => block.Statements.Count > 0 && Terminates(block.Statements[^1]),
            ReturnStatementSyntax or ThrowStatementSyntax => true,
            _ => false
        };

    [Pure]
    private static SyntaxNode RewriteReads(IReadOnlyDictionary<string, Register> pairs, State state, SyntaxNode node) =>
        new ReadRewriter(pairs, state).Visit(node);

    [Pure]
    private static bool TryGetEmulatorField(ExpressionSyntax expression, [NotNullWhen(true)] out string? fieldName)
    {
        if (expression is MemberAccessExpressionSyntax
            {
                Expression: IdentifierNameSyntax { Identifier.ValueText: Parameter.Name.Emulator },
                Name.Identifier.ValueText: var name
            })
        {
            fieldName = name;
            return true;
        }

        fieldName = null;
        return false;
    }

    private sealed class PairState
    {
        public int CallsSinceHalfWrite { get; set; }

        public ExpressionSyntax? HighValue { get; set; }

        public ExpressionSyntax? LowValue { get; set; }

        [Pure]
        public PairState Clone() => new() { CallsSinceHalfWrite = CallsSinceHalfWrite, HighValue = HighValue, LowValue = LowValue };
    }

    private sealed class State
    {
        private readonly Dictionary<string, PairState> pairStates = new(StringComparer.Ordinal);

        [Pure]
        public bool IsComposable(Register pair) =>
            pairStates.TryGetValue(pair.FieldName, out var pairState) && pairState.CallsSinceHalfWrite <= MaxCallsSinceHalfWrite;

        // Composes the pair from its halves, forwarding the source value of a recent half write when it is still
        // valid, and reading the half-register otherwise.
        [Pure]
        public ExpressionSyntax CreateCompose(Register pair)
        {
            pairStates.TryGetValue(pair.FieldName, out var pairState);

            var high = pairState?.HighValue ?? EmulatorMemberIdentifier(pair.HighRegister!.FieldName);
            var low = pairState?.LowValue ?? EmulatorMemberIdentifier(pair.LowRegister!.FieldName);

            return BinaryExpression(
                SyntaxKind.BitwiseOrExpression,
                BinaryExpression(SyntaxKind.LeftShiftExpression, high, GenerateNumericLiteralExpression(8)),
                low);
        }

        public void OnWrite(IReadOnlyDictionary<string, Register> pairs, ExpressionSyntax target, ExpressionSyntax? simpleAssignmentValue)
        {
            if (!TryGetEmulatorField(target, out var fieldName))
            {
                return;
            }

            // Reassigning a field invalidates any forwarded value that reads it.
            foreach (var pairState in pairStates.Values)
            {
                if (ReadsField(pairState.HighValue, fieldName))
                {
                    pairState.HighValue = null;
                }

                if (ReadsField(pairState.LowValue, fieldName))
                {
                    pairState.LowValue = null;
                }
            }

            if (!pairs.TryGetValue(fieldName, out var pair))
            {
                return;
            }

            if (fieldName == pair.FieldName)
            {
                // A whole-pair write; subsequent 16-bit reads forward from the store buffer at full speed.
                pairStates.Remove(pair.FieldName);
                return;
            }

            if (!pairStates.TryGetValue(pair.FieldName, out var state))
            {
                state = new PairState();
                pairStates.Add(pair.FieldName, state);
            }

            state.CallsSinceHalfWrite = 0;
            var forwardable = IsForwardable(simpleAssignmentValue) ? simpleAssignmentValue : null;
            if (fieldName == pair.HighRegister!.FieldName)
            {
                state.HighValue = forwardable;
            }
            else
            {
                state.LowValue = forwardable;
            }
        }

        // A call invalidates forwarded values, as the bus handler can change the data register, and moves the half
        // writes a step further from any read that would want to compose them.
        public void OnCalls(SyntaxNode node)
        {
            if (pairStates.Count == 0)
            {
                return;
            }

            var calls = node.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().Count();
            if (calls == 0)
            {
                return;
            }

            foreach (var pairState in pairStates.Values)
            {
                pairState.CallsSinceHalfWrite += calls;
                pairState.HighValue = null;
                pairState.LowValue = null;
            }
        }

        [Pure]
        public State Fork()
        {
            var fork = new State();
            foreach (var (fieldName, pairState) in pairStates)
            {
                fork.pairStates.Add(fieldName, pairState.Clone());
            }

            return fork;
        }

        public void Merge(State branch)
        {
            foreach (var fieldName in pairStates.Keys.ToList())
            {
                if (branch.pairStates.TryGetValue(fieldName, out var branchState))
                {
                    var pairState = pairStates[fieldName];
                    pairState.CallsSinceHalfWrite = Math.Max(pairState.CallsSinceHalfWrite, branchState.CallsSinceHalfWrite);
                    pairState.HighValue = MergeValue(pairState.HighValue, branchState.HighValue);
                    pairState.LowValue = MergeValue(pairState.LowValue, branchState.LowValue);
                }
                else
                {
                    pairStates.Remove(fieldName);
                }
            }
        }

        [Pure]
        private static ExpressionSyntax? MergeValue(ExpressionSyntax? current, ExpressionSyntax? branch) =>
            current != null && branch != null && current.IsEquivalentTo(branch) ? current : null;

        // Only simple values that can be re-evaluated later are forwarded: a field read or a literal.
        [Pure]
        private static bool IsForwardable([NotNullWhen(true)] ExpressionSyntax? value) =>
            value is LiteralExpressionSyntax || (value != null && TryGetEmulatorField(value, out _));

        [Pure]
        private static bool ReadsField(ExpressionSyntax? value, string fieldName) =>
            value != null && TryGetEmulatorField(value, out var read) && read == fieldName;
    }

    private sealed class ReadRewriter(IReadOnlyDictionary<string, Register> pairs, State state) : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (TryGetEmulatorField(node, out var fieldName) &&
                pairs.TryGetValue(fieldName, out var pair) &&
                pair.FieldName == fieldName &&
                state.IsComposable(pair))
            {
                var compose = ParenthesizedExpression(state.CreateCompose(pair));

                // In an arithmetic context the pair read has been promoted to int anyway and the composed value
                // always fits in sixteen bits, so the cast is only needed where a ushort-typed expression is
                // required, e.g. the right-hand side of an assignment.
                return (node.Parent is BinaryExpressionSyntax
                        ? compose
                        : (ExpressionSyntax)CastExpression(UShortType, compose))
                    .WithTriviaFrom(node);
            }

            return base.VisitMemberAccessExpression(node) ?? node;
        }
    }
}