using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using Parameter = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Parameter;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.InstructionMethods.Optimization;

// Rewrites 16-bit reads of a register pair as a composition of its half-registers, e.g. emulator.PC = emulator.WZ
// as emulator.PC = (ushort)(emulator.W << 8 | emulator.Z), when one of the halves was byte-written shortly before
// in the same method. A 16-bit load that overlaps an in-flight byte store cannot be store-to-load forwarded and
// stalls until the store buffer drains, whereas byte loads after byte stores forward at full speed. The rewrite is
// always semantically identical because the halves overlay the pair, so the state tracking below only decides
// profitability: reads are rewritten while the half-store can still plausibly be in the store buffer, i.e. at most
// MaxCallsSinceHalfWrite calls after it, and not once the pair has subsequently been written whole.
internal sealed class ComposePairReadsAfterHalfWrites : InstructionMethodOptimization
{
    private const int MaxCallsSinceHalfWrite = 1;

    protected override IReadOnlyList<StatementSyntax> Optimize(FileGeneratorContext context, IReadOnlyList<StatementSyntax> statements)
    {
        var pairs = CreatePairsByFieldName(context.GeneratorContext.Configuration);
        var state = new Dictionary<string, int>(StringComparer.Ordinal);
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
    private static IReadOnlyList<StatementSyntax> OptimizeStatements(IReadOnlyDictionary<string, Register> pairs, Dictionary<string, int> state, IReadOnlyList<StatementSyntax> statements) =>
        statements.Select(statement => OptimizeStatement(pairs, state, statement)).ToList();

    [MustUseReturnValue]
    private static StatementSyntax OptimizeStatement(IReadOnlyDictionary<string, Register> pairs, Dictionary<string, int> state, StatementSyntax statement)
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
                    CountCalls(state, optimized);
                    return optimized;
                }
        }
    }

    [MustUseReturnValue]
    private static StatementSyntax OptimizeAssignment(IReadOnlyDictionary<string, Register> pairs, Dictionary<string, int> state, ExpressionStatementSyntax statement, AssignmentExpressionSyntax assignment)
    {
        var newRight = (ExpressionSyntax)RewriteReads(pairs, state, assignment.Right);

        StatementSyntax optimized;
        if (assignment.Kind() is SyntaxKind.AddAssignmentExpression or SyntaxKind.SubtractAssignmentExpression &&
            TryGetEmulatorField(assignment.Left, out var fieldName) &&
            pairs.TryGetValue(fieldName, out var pair) &&
            pair.FieldName == fieldName &&
            IsComposable(state, pair))
        {
            // A compound assignment reads the pair too, so e.g. emulator.WZ += 1 becomes
            // emulator.WZ = (ushort)((emulator.W << 8 | emulator.Z) + 1).
            var operatorKind = assignment.Kind() == SyntaxKind.AddAssignmentExpression ? SyntaxKind.AddExpression : SyntaxKind.SubtractExpression;
            var value = BinaryExpression(operatorKind, ParenthesizedExpression(CreateCompose(pair)), newRight);
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

        RecordWrite(pairs, state, assignment.Left);
        CountCalls(state, optimized);
        return optimized;
    }

    [MustUseReturnValue]
    private static StatementSyntax OptimizeIfStatement(IReadOnlyDictionary<string, Register> pairs, Dictionary<string, int> state, IfStatementSyntax ifStatement)
    {
        var newCondition = (ExpressionSyntax)RewriteReads(pairs, state, ifStatement.Condition);
        CountCalls(state, newCondition);

        var newStatement = OptimizeBranch(pairs, state, ifStatement.Statement);
        var newElse = ifStatement.Else?.WithStatement(OptimizeBranch(pairs, state, ifStatement.Else.Statement));

        return ifStatement.WithCondition(newCondition).WithStatement(newStatement).WithElse(newElse);
    }

    // Optimizes a branch with a copy of the current state. A branch that terminates in a return or throw contributes
    // nothing on the fall-through path so the state is left untouched; otherwise the state is conservatively merged
    // to pairs that stayed active through the branch, at the worse of the two call counts.
    [MustUseReturnValue]
    private static StatementSyntax OptimizeBranch(IReadOnlyDictionary<string, Register> pairs, Dictionary<string, int> state, StatementSyntax branch)
    {
        var branchState = new Dictionary<string, int>(state, StringComparer.Ordinal);
        var optimized = OptimizeStatement(pairs, branchState, branch);
        if (!Terminates(branch))
        {
            foreach (var pairFieldName in state.Keys.ToList())
            {
                if (branchState.TryGetValue(pairFieldName, out var branchCalls))
                {
                    state[pairFieldName] = Math.Max(state[pairFieldName], branchCalls);
                }
                else
                {
                    state.Remove(pairFieldName);
                }
            }
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

    private static void RecordWrite(IReadOnlyDictionary<string, Register> pairs, Dictionary<string, int> state, ExpressionSyntax target)
    {
        if (!TryGetEmulatorField(target, out var fieldName) || !pairs.TryGetValue(fieldName, out var pair))
        {
            return;
        }

        if (fieldName == pair.FieldName)
        {
            // A whole-pair write; subsequent 16-bit reads forward from the store buffer at full speed.
            state.Remove(pair.FieldName);
        }
        else
        {
            state[pair.FieldName] = 0;
        }
    }

    private static void CountCalls(Dictionary<string, int> state, SyntaxNode node)
    {
        if (state.Count == 0)
        {
            return;
        }

        var calls = node.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().Count();
        if (calls == 0)
        {
            return;
        }

        foreach (var pairFieldName in state.Keys.ToList())
        {
            state[pairFieldName] += calls;
        }
    }

    [Pure]
    private static bool IsComposable(IReadOnlyDictionary<string, int> state, Register pair) =>
        state.TryGetValue(pair.FieldName, out var calls) && calls <= MaxCallsSinceHalfWrite;

    [Pure]
    private static SyntaxNode RewriteReads(IReadOnlyDictionary<string, Register> pairs, IReadOnlyDictionary<string, int> state, SyntaxNode node) =>
        new ReadRewriter(pairs, state).Visit(node) ?? node;

    [Pure]
    private static ExpressionSyntax CreateCompose(Register pair) =>
        BinaryExpression(
            SyntaxKind.BitwiseOrExpression,
            BinaryExpression(
                SyntaxKind.LeftShiftExpression,
                EmulatorMemberIdentifier(pair.HighRegister!.FieldName),
                GenerateNumericLiteralExpression(8)),
            EmulatorMemberIdentifier(pair.LowRegister!.FieldName));

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

    private sealed class ReadRewriter(IReadOnlyDictionary<string, Register> pairs, IReadOnlyDictionary<string, int> state) : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (TryGetEmulatorField(node, out var fieldName) &&
                pairs.TryGetValue(fieldName, out var pair) &&
                pair.FieldName == fieldName &&
                IsComposable(state, pair))
            {
                return CastExpression(UShortType, ParenthesizedExpression(CreateCompose(pair))).WithTriviaFrom(node);
            }

            return base.VisitMemberAccessExpression(node) ?? node;
        }
    }
}