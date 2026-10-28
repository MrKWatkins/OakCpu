using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using Parameter = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Parameter;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Optimization;

// Tracks register values in locals for the duration of an instruction method, committing them back to their fields
// only before each return. For example CALL sets PC three times and SP twice; tracking turns those into register
// operations with a single store each on exit. A pair written via its halves, e.g. Z and W for an operand address,
// is tracked as two byte locals and flushed with a single 16-bit store, which also eliminates intermediate stores
// whose value is overwritten later in the instruction, e.g. the first W write of LD (nn), A. The bus handler may
// observe stale internal register state during OnActionRequired, which the emulator contract permits; Address and
// Data are data members, not registers, so they are never deferred and the bus contract is unaffected.
//
// A register is tracked when it is written at least twice, via any mix of half and full writes. Registers whose
// accesses span a call other than the bus handler, e.g. HandleInterrupts, are not tracked as such calls can observe
// register state, and neither are registers that would need to declare locals or switch between whole-value and
// half-value tracking inside a branch. Locals cannot survive a step emulator suspension, so this optimization is
// only valid for instruction emulator methods.
internal sealed class DeferRegisterStores : MethodBodyOptimization
{
    protected override IReadOnlyList<StatementSyntax> Optimize(FileGeneratorContext context, IReadOnlyList<StatementSyntax> statements)
    {
        var registers = CreateRegistersByFieldName(context.GeneratorContext.Configuration);
        var usages = Analyse(registers, statements);
        var units = CreateTrackedUnits(usages, statements);

        return units.Count == 0 ? statements : new Transformer(units).Transform(statements);
    }

    // Maps the field name of each register, and of the halves of 16-bit pairs, to the top-level register that would
    // be tracked for it.
    [Pure]
    private static IReadOnlyDictionary<string, Register> CreateRegistersByFieldName(Configuration configuration)
    {
        var byFieldName = new Dictionary<string, Register>(StringComparer.Ordinal);
        foreach (var register in configuration.Registers.Values.Where(register => register.Parent == null))
        {
            byFieldName.Add(register.FieldName, register);
            if (register.HighRegister != null)
            {
                byFieldName.Add(register.HighRegister.FieldName, register);
            }

            if (register.LowRegister != null)
            {
                byFieldName.Add(register.LowRegister.FieldName, register);
            }
        }

        return byFieldName;
    }

    [Pure]
    private static Dictionary<Register, RegisterUsage> Analyse(IReadOnlyDictionary<string, Register> registers, IReadOnlyList<StatementSyntax> statements)
    {
        var usages = new Dictionary<Register, RegisterUsage>();
        var walker = new AnalysisWalker(registers, usages);
        for (var index = 0; index < statements.Count; index++)
        {
            walker.TopLevelIndex = index;
            walker.Visit(statements[index]);
        }

        return usages;
    }

    [Pure]
    private static IReadOnlyList<TrackedUnit> CreateTrackedUnits(Dictionary<Register, RegisterUsage> usages, IReadOnlyList<StatementSyntax> statements)
    {
        var trackable = usages
            .Where(usage => usage.Value is { Writes: >= 2, SpansBarrier: false } && !IsFlagsPair(usage.Key) && CanDeclareAllLocals(usage.Value))
            .OrderBy(usage => usage.Value.FirstAccessTopLevelIndex)
            .ThenBy(usage => usage.Key.FieldName, StringComparer.Ordinal)
            .ToList();

        if (trackable.Count == 0)
        {
            return [];
        }

        var takenNames = GetLocalNames(statements);
        var units = new List<TrackedUnit>();
        foreach (var (register, usage) in trackable)
        {
            var unit = TrackedUnit.CreateOrNull(register, usage, takenNames);
            if (unit == null)
            {
                continue;
            }

            takenNames.Add(unit.FullLocalName);
            if (unit.IsPair)
            {
                takenNames.Add(unit.HighLocalName!);
                takenNames.Add(unit.LowLocalName!);
            }

            units.Add(unit);
        }

        return units;
    }

    // Tracking a pair containing the flags register buys nothing: it is written by almost every instruction but
    // essentially never read at 16 bits, so its halves would flush as plain byte stores anyway - identical to just
    // writing the fields where the instruction computes them. A standalone flags register still benefits, as its
    // multiple writes collapse into a single store.
    [Pure]
    private static bool IsFlagsPair(Register register) =>
        register.HighRegister?.Flags == true || register.LowRegister?.Flags == true;

    // Replays the writes the transformer will see to check that every local declaration and every switch between
    // whole-value and half-value tracking happens at the top level of the method, where locals stay in scope and
    // the tracking state cannot diverge between branch paths.
    [Pure]
    private static bool CanDeclareAllLocals(RegisterUsage usage)
    {
        var representation = Representation.Field;
        var highDeclared = false;
        var lowDeclared = false;
        var fullDeclared = false;

        if (usage.FirstAccessIsNested)
        {
            // The transformer declares the locals from the fields at the top level just before the first access.
            if (usage.HasHalfWrites)
            {
                representation = Representation.Halves;
                highDeclared = true;
                lowDeclared = true;
            }
            else
            {
                representation = Representation.Full;
                fullDeclared = true;
            }
        }

        foreach (var write in usage.WriteAccesses)
        {
            if (write.IsFull)
            {
                if (representation != Representation.Full && write.IsNested)
                {
                    return false;
                }

                if (!fullDeclared && write.IsNested)
                {
                    return false;
                }

                representation = Representation.Full;
                fullDeclared = true;
            }
            else
            {
                if (representation == Representation.Full && write.IsNested)
                {
                    return false;
                }

                var declared = write.IsHigh ? highDeclared : lowDeclared;
                if (!declared && write.IsNested)
                {
                    return false;
                }

                if (representation == Representation.Full)
                {
                    // Switching materializes the other half too.
                    highDeclared = true;
                    lowDeclared = true;
                }
                else if (write.IsHigh)
                {
                    highDeclared = true;
                }
                else
                {
                    lowDeclared = true;
                }

                representation = Representation.Halves;
            }
        }

        return true;
    }

    [Pure]
    private static HashSet<string> GetLocalNames(IReadOnlyList<StatementSyntax> statements) =>
        statements
            .SelectMany(statement => statement.DescendantNodesAndSelf().OfType<VariableDeclaratorSyntax>())
            .Select(declarator => declarator.Identifier.ValueText)
            .ToHashSet(StringComparer.Ordinal);

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

    [Pure]
    private static SyntaxKind GetBinaryOperator(SyntaxKind compoundAssignment) =>
        compoundAssignment switch
        {
            SyntaxKind.AddAssignmentExpression => SyntaxKind.AddExpression,
            SyntaxKind.SubtractAssignmentExpression => SyntaxKind.SubtractExpression,
            SyntaxKind.AndAssignmentExpression => SyntaxKind.BitwiseAndExpression,
            SyntaxKind.OrAssignmentExpression => SyntaxKind.BitwiseOrExpression,
            SyntaxKind.ExclusiveOrAssignmentExpression => SyntaxKind.ExclusiveOrExpression,
            SyntaxKind.LeftShiftAssignmentExpression => SyntaxKind.LeftShiftExpression,
            SyntaxKind.RightShiftAssignmentExpression => SyntaxKind.RightShiftExpression,
            _ => throw new InvalidOperationException($"Unsupported compound assignment {compoundAssignment}.")
        };

    private enum Representation
    {
        // The field still holds the value; nothing is tracked yet.
        Field,

        // The whole value is in the full-width local.
        Full,

        // The value is split across the half locals, each of which is either a declared local or still in its
        // half-register field.
        Halves
    }

    private sealed record WriteAccess(bool IsFull, bool IsHigh, bool IsNested);

    private sealed class RegisterUsage
    {
        public int Writes { get; set; }

        public bool SpansBarrier { get; set; }

        public int FirstAccessTopLevelIndex { get; init; } = -1;

        public bool FirstAccessIsNested { get; init; }

        public bool HasHalfWrites { get; set; }

        public List<WriteAccess> WriteAccesses { get; } = [];
    }

    private sealed class TrackedUnit
    {
        private TrackedUnit(Register register, RegisterUsage usage, string fullLocalName, string? highLocalName, string? lowLocalName)
        {
            Register = register;
            Usage = usage;
            FullLocalName = fullLocalName;
            HighLocalName = highLocalName;
            LowLocalName = lowLocalName;
        }

        public Register Register { get; }

        public RegisterUsage Usage { get; }

        public string FullLocalName { get; }

        public string? HighLocalName { get; }

        public string? LowLocalName { get; }

        public bool IsPair => HighLocalName != null;

        public Representation Representation { get; set; } = Representation.Field;

        public bool FullDeclared { get; set; }

        public bool HighDeclared { get; set; }

        public bool LowDeclared { get; set; }

        public bool FullDirty { get; set; }

        public bool HighDirty { get; set; }

        public bool LowDirty { get; set; }

        [Pure]
        public static TrackedUnit? CreateOrNull(Register register, RegisterUsage usage, HashSet<string> takenNames)
        {
            var fullLocalName = register.FieldName.ToLowerInvariant();
            if (takenNames.Contains(fullLocalName))
            {
                return null;
            }

            if (register.HighRegister == null)
            {
                return new TrackedUnit(register, usage, fullLocalName, null, null);
            }

            var highLocalName = register.HighRegister.FieldName.ToLowerInvariant();
            var lowLocalName = register.LowRegister!.FieldName.ToLowerInvariant();
            if (takenNames.Contains(highLocalName) || takenNames.Contains(lowLocalName))
            {
                return null;
            }

            return new TrackedUnit(register, usage, fullLocalName, highLocalName, lowLocalName);
        }
    }

    private sealed class AnalysisWalker(IReadOnlyDictionary<string, Register> registers, Dictionary<Register, RegisterUsage> usages) : CSharpSyntaxWalker
    {
        private bool insideReturn;
        private int depth;

        public int TopLevelIndex { get; set; }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            insideReturn = true;
            base.VisitReturnStatement(node);
            insideReturn = false;
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            Visit(node.Condition);
            depth++;
            Visit(node.Statement);
            if (node.Else != null)
            {
                Visit(node.Else.Statement);
            }

            depth--;
        }

        public override void VisitBlock(BlockSyntax node)
        {
            // Locals declared inside a block, including the blocks some steps are wrapped in to isolate their
            // locals, would not be in scope for the rest of the method.
            depth++;
            base.VisitBlock(node);
            depth--;
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            // A call to anything other than the bus handler can legitimately observe register state, e.g.
            // HandleInterrupts or a dispatched instruction, so registers accessed before it cannot be tracked.
            // Registers accessed for the first time after the call get a fresh usage and are unaffected. Calls
            // inside a return are not barriers because tracked registers are always stored back before the return.
            if (!insideReturn && !IsBusHandlerCall(node) && !IsPureHelperCall(node))
            {
                foreach (var usage in usages.Values)
                {
                    usage.SpansBarrier = true;
                }
            }

            base.VisitInvocationExpression(node);
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (TryGetEmulatorField(node, out var fieldName) && registers.TryGetValue(fieldName, out var register))
            {
                if (!usages.TryGetValue(register, out var usage))
                {
                    usage = new RegisterUsage { FirstAccessTopLevelIndex = TopLevelIndex, FirstAccessIsNested = depth > 0 };
                    usages.Add(register, usage);
                }

                if (node.Parent is AssignmentExpressionSyntax assignment && assignment.Left == node)
                {
                    var isFullAccess = fieldName == register.FieldName;
                    usage.Writes++;
                    usage.HasHalfWrites |= !isFullAccess;
                    usage.WriteAccesses.Add(new WriteAccess(isFullAccess, fieldName == register.HighRegister?.FieldName, depth > 0));
                }
            }

            base.VisitMemberAccessExpression(node);
        }

        [Pure]
        private static bool IsBusHandlerCall(InvocationExpressionSyntax node) =>
            node.Expression is MemberAccessExpressionSyntax
            {
                Expression: IdentifierNameSyntax { Identifier.ValueText: InstructionHandlerSyntax.ParameterName }
            };

        [Pure]
        private static bool IsPureHelperCall(InvocationExpressionSyntax node) =>
            node.Expression is MemberAccessExpressionSyntax
            {
                Expression: IdentifierNameSyntax { Identifier.ValueText: "Unsafe" or "MemoryMarshal" or "BitOperations" }
            };
    }

    private sealed class Transformer(IReadOnlyList<TrackedUnit> units)
    {
        private readonly ReadRewriter rewriter = new(units);

        [MustUseReturnValue]
        public IReadOnlyList<StatementSyntax> Transform(IReadOnlyList<StatementSyntax> statements)
        {
            var result = new List<StatementSyntax>(statements.Count + units.Count);
            for (var index = 0; index < statements.Count; index++)
            {
                // A unit whose first access is nested inside a branch cannot declare its locals there, so declare
                // them from the fields at the top level just before.
                foreach (var unit in units.Where(unit => unit.Usage.FirstAccessTopLevelIndex == index && unit.Usage.FirstAccessIsNested))
                {
                    DeclareFromField(result, unit);
                }

                ProcessStatement(result, statements[index], 0);
            }

            return result;
        }

        private void ProcessStatement(List<StatementSyntax> into, StatementSyntax statement, int depth)
        {
            switch (statement)
            {
                case ReturnStatementSyntax returnStatement:
                    AddFlushes(into);
                    into.Add((StatementSyntax)rewriter.Visit(returnStatement));
                    return;

                case BlockSyntax block:
                    {
                        var statements = new List<StatementSyntax>(block.Statements.Count);
                        foreach (var child in block.Statements)
                        {
                            ProcessStatement(statements, child, depth + 1);
                        }

                        into.Add(block.WithStatements(List(statements)));
                        return;
                    }

                case IfStatementSyntax ifStatement:
                    {
                        var newCondition = (ExpressionSyntax)rewriter.Visit(ifStatement.Condition);
                        var newStatement = ProcessBranch(ifStatement.Statement, depth + 1);
                        var newElse = ifStatement.Else?.WithStatement(ProcessBranch(ifStatement.Else.Statement, depth + 1));
                        into.Add(ifStatement.WithCondition(newCondition).WithStatement(newStatement).WithElse(newElse));
                        return;
                    }

                case ExpressionStatementSyntax { Expression: AssignmentExpressionSyntax assignment } expressionStatement
                    when TryGetTargetUnit(assignment.Left, out var unit, out var targetsFull, out var targetsHigh):
                    ProcessTrackedAssignment(into, expressionStatement, assignment, unit, targetsFull, targetsHigh, depth);
                    return;

                default:
                    into.Add((StatementSyntax)rewriter.Visit(statement));
                    return;
            }
        }

        [MustUseReturnValue]
        private StatementSyntax ProcessBranch(StatementSyntax branch, int depth)
        {
            var statements = new List<StatementSyntax>();
            ProcessStatement(statements, branch, depth);
            return statements.Count == 1 ? statements[0] : Block(List(statements));
        }

        private void ProcessTrackedAssignment(List<StatementSyntax> into, ExpressionStatementSyntax statement, AssignmentExpressionSyntax assignment, TrackedUnit unit, bool targetsFull, bool targetsHigh, int depth)
        {
            var isCompound = assignment.Kind() != SyntaxKind.SimpleAssignmentExpression;
            var newRight = (ExpressionSyntax)rewriter.Visit(assignment.Right);

            if (targetsFull)
            {
                if (isCompound && unit.Representation == Representation.Full)
                {
                    into.Add(statement.WithExpression(assignment.WithLeft(IdentifierName(unit.FullLocalName)).WithRight(newRight)));
                }
                else if (isCompound)
                {
                    // A compound assignment reads the register too, so rewrite it as a simple assignment of the
                    // combined current value.
                    var value = CastExpression(
                        unit.Register.TypeSyntax,
                        ParenthesizedExpression(
                            BinaryExpression(GetBinaryOperator(assignment.Kind()), ParenthesizedExpression(CreateWholeValue(unit)), newRight)));
                    SetFull(into, unit, value, statement, depth);
                }
                else
                {
                    SetFull(into, unit, newRight, statement, depth);
                }

                unit.Representation = Representation.Full;
                unit.FullDirty = true;
                unit.HighDirty = false;
                unit.LowDirty = false;
            }
            else
            {
                // A half write; when the value lives in the full-width local the other half is materialized out of
                // it first.
                if (unit.Representation == Representation.Full)
                {
                    SetHalf(into, unit, !targetsHigh, CreateHalfOfFull(unit, !targetsHigh), statement, depth, dirty: unit.FullDirty);
                    unit.Representation = Representation.Halves;
                    unit.FullDirty = false;
                }

                if (isCompound && IsHalfDeclared(unit, targetsHigh))
                {
                    into.Add(statement.WithExpression(assignment.WithLeft(IdentifierName(GetHalfLocalName(unit, targetsHigh))).WithRight(newRight)));
                    SetHalfDirty(unit, targetsHigh);
                }
                else if (isCompound)
                {
                    // The half is still in its field; initialize the local with the operation applied.
                    var halfField = EmulatorMemberIdentifier(GetHalfRegister(unit, targetsHigh).FieldName);
                    var value = CastExpression(
                        PredefinedType(Token(SyntaxKind.ByteKeyword)),
                        ParenthesizedExpression(BinaryExpression(GetBinaryOperator(assignment.Kind()), halfField, newRight)));
                    SetHalf(into, unit, targetsHigh, value, statement, depth, dirty: true);
                }
                else
                {
                    SetHalf(into, unit, targetsHigh, newRight, statement, depth, dirty: true);
                }

                unit.Representation = Representation.Halves;
            }
        }

        private static void SetFull(List<StatementSyntax> into, TrackedUnit unit, ExpressionSyntax value, StatementSyntax original, int depth)
        {
            if (unit.FullDeclared)
            {
                into.Add(ExpressionStatement(
                        AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(unit.FullLocalName), value))
                    .WithTriviaFrom(original));
                return;
            }

            if (depth > 0)
            {
                throw new InvalidOperationException($"Cannot declare the local for {unit.Register.Name} inside a branch.");
            }

            unit.FullDeclared = true;
            into.Add(CreateDeclaration(unit.Register.TypeSyntax, unit.FullLocalName, value).WithTriviaFrom(original));
        }

        private static void SetHalf(List<StatementSyntax> into, TrackedUnit unit, bool high, ExpressionSyntax value, StatementSyntax original, int depth, bool dirty)
        {
            if (IsHalfDeclared(unit, high))
            {
                into.Add(ExpressionStatement(
                        AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(GetHalfLocalName(unit, high)), value))
                    .WithTriviaFrom(original));
            }
            else
            {
                if (depth > 0)
                {
                    throw new InvalidOperationException($"Cannot declare the local for {unit.Register.Name} inside a branch.");
                }

                SetHalfDeclared(unit, high);
                into.Add(CreateDeclaration(PredefinedType(Token(SyntaxKind.ByteKeyword)), GetHalfLocalName(unit, high), value).WithTriviaFrom(original));
            }

            if (dirty)
            {
                SetHalfDirty(unit, high);
            }
        }

        private static void DeclareFromField(List<StatementSyntax> into, TrackedUnit unit)
        {
            if (unit.Usage.HasHalfWrites && unit.IsPair)
            {
                SetHalf(into, unit, high: true, EmulatorMemberIdentifier(unit.Register.HighRegister!.FieldName), EmptyStatement(), 0, dirty: false);
                SetHalf(into, unit, high: false, EmulatorMemberIdentifier(unit.Register.LowRegister!.FieldName), EmptyStatement(), 0, dirty: false);
                unit.Representation = Representation.Halves;
            }
            else
            {
                SetFull(into, unit, EmulatorMemberIdentifier(unit.Register.FieldName), EmptyStatement(), 0);
                unit.Representation = Representation.Full;
            }
        }

        private void AddFlushes(List<StatementSyntax> into)
        {
            foreach (var unit in units)
            {
                switch (unit.Representation)
                {
                    case Representation.Full when unit.FullDirty:
                        into.Add(CreateStore(unit.Register.FieldName, IdentifierName(unit.FullLocalName)));
                        break;

                    case Representation.Halves when unit is { HighDirty: true, LowDirty: true, Register.Transient: true }:
                        // The combined store below trades two ALU operations for immunity to a 16-bit read of the
                        // pair stalling on two in-flight byte stores. A transient pair is never read at 16 bits
                        // across instruction boundaries, so plain byte stores win there.
                        into.Add(CreateStore(unit.Register.HighRegister!.FieldName, IdentifierName(unit.HighLocalName!)));
                        into.Add(CreateStore(unit.Register.LowRegister!.FieldName, IdentifierName(unit.LowLocalName!)));
                        break;

                    case Representation.Halves when unit is { HighDirty: true, LowDirty: true }:
                        into.Add(CreateStore(unit.Register.FieldName, CastExpression(UShortType, ParenthesizedExpression(CreateHalvesCompose(unit)))));
                        break;

                    case Representation.Halves when unit.HighDirty:
                        into.Add(CreateStore(unit.Register.HighRegister!.FieldName, IdentifierName(unit.HighLocalName!)));
                        break;

                    case Representation.Halves when unit.LowDirty:
                        into.Add(CreateStore(unit.Register.LowRegister!.FieldName, IdentifierName(unit.LowLocalName!)));
                        break;
                }
            }
        }

        [Pure]
        private static StatementSyntax CreateStore(string fieldName, ExpressionSyntax value) =>
            ExpressionStatement(
                AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, EmulatorMemberIdentifier(fieldName), value));

        [Pure]
        private static StatementSyntax CreateDeclaration(TypeSyntax type, string name, ExpressionSyntax value) =>
            LocalDeclarationStatement(
                VariableDeclaration(type)
                    .WithVariables([VariableDeclarator(name).WithInitializer(EqualsValueClause(value))]));

        [Pure]
        private bool TryGetTargetUnit(ExpressionSyntax target, [NotNullWhen(true)] out TrackedUnit? unit, out bool targetsFull, out bool targetsHigh)
        {
            unit = null;
            targetsFull = false;
            targetsHigh = false;
            if (!TryGetEmulatorField(target, out var fieldName))
            {
                return false;
            }

            foreach (var candidate in units)
            {
                if (fieldName == candidate.Register.FieldName)
                {
                    unit = candidate;
                    targetsFull = true;
                    return true;
                }

                if (candidate.IsPair && fieldName == candidate.Register.HighRegister!.FieldName)
                {
                    unit = candidate;
                    targetsHigh = true;
                    return true;
                }

                if (candidate.IsPair && fieldName == candidate.Register.LowRegister!.FieldName)
                {
                    unit = candidate;
                    return true;
                }
            }

            return false;
        }

        [Pure]
        private static ExpressionSyntax CreateWholeValue(TrackedUnit unit) =>
            unit.Representation switch
            {
                Representation.Full => IdentifierName(unit.FullLocalName),
                Representation.Halves => CreateHalvesCompose(unit),
                _ => EmulatorMemberIdentifier(unit.Register.FieldName)
            };

        [Pure]
        private static ExpressionSyntax CreateHalvesCompose(TrackedUnit unit) =>
            BinaryExpression(
                SyntaxKind.BitwiseOrExpression,
                BinaryExpression(SyntaxKind.LeftShiftExpression, CreateHalfValue(unit, high: true), GenerateNumericLiteralExpression(8)),
                CreateHalfValue(unit, high: false));

        [Pure]
        private static ExpressionSyntax CreateHalfValue(TrackedUnit unit, bool high) =>
            unit.Representation == Representation.Full
                ? CreateHalfOfFull(unit, high)
                : IsHalfDeclared(unit, high)
                    ? IdentifierName(GetHalfLocalName(unit, high))
                    : EmulatorMemberIdentifier(GetHalfRegister(unit, high).FieldName);

        [Pure]
        private static ExpressionSyntax CreateHalfOfFull(TrackedUnit unit, bool high) =>
            high
                ? CastExpression(
                    PredefinedType(Token(SyntaxKind.ByteKeyword)),
                    ParenthesizedExpression(
                        BinaryExpression(SyntaxKind.RightShiftExpression, IdentifierName(unit.FullLocalName), GenerateNumericLiteralExpression(8))))
                : CastExpression(PredefinedType(Token(SyntaxKind.ByteKeyword)), IdentifierName(unit.FullLocalName));

        [Pure]
        private static Register GetHalfRegister(TrackedUnit unit, bool high) => high ? unit.Register.HighRegister! : unit.Register.LowRegister!;

        [Pure]
        private static string GetHalfLocalName(TrackedUnit unit, bool high) => high ? unit.HighLocalName! : unit.LowLocalName!;

        [Pure]
        private static bool IsHalfDeclared(TrackedUnit unit, bool high) => high ? unit.HighDeclared : unit.LowDeclared;

        private static void SetHalfDeclared(TrackedUnit unit, bool high)
        {
            if (high)
            {
                unit.HighDeclared = true;
            }
            else
            {
                unit.LowDeclared = true;
            }
        }

        private static void SetHalfDirty(TrackedUnit unit, bool high)
        {
            if (high)
            {
                unit.HighDirty = true;
            }
            else
            {
                unit.LowDirty = true;
            }
        }

        private sealed class ReadRewriter(IReadOnlyList<TrackedUnit> units) : CSharpSyntaxRewriter
        {
            public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
            {
                if (TryGetEmulatorField(node, out var fieldName))
                {
                    foreach (var unit in units)
                    {
                        if (unit.Representation == Representation.Field)
                        {
                            continue;
                        }

                        if (fieldName == unit.Register.FieldName)
                        {
                            if (unit.Representation == Representation.Full)
                            {
                                return IdentifierName(unit.FullLocalName).WithTriviaFrom(node);
                            }

                            var compose = ParenthesizedExpression(CreateHalvesCompose(unit));

                            // In an arithmetic context the pair read has been promoted to int anyway and the
                            // composed value always fits in sixteen bits, so the cast is only needed where a
                            // ushort-typed expression is required.
                            return (node.Parent is BinaryExpressionSyntax
                                    ? compose
                                    : (ExpressionSyntax)CastExpression(UShortType, compose))
                                .WithTriviaFrom(node);
                        }

                        if (unit.IsPair && fieldName == unit.Register.HighRegister!.FieldName)
                        {
                            return CreateHalfValue(unit, high: true).WithTriviaFrom(node);
                        }

                        if (unit.IsPair && fieldName == unit.Register.LowRegister!.FieldName)
                        {
                            return CreateHalfValue(unit, high: false).WithTriviaFrom(node);
                        }
                    }
                }

                return base.VisitMemberAccessExpression(node) ?? node;
            }
        }
    }
}