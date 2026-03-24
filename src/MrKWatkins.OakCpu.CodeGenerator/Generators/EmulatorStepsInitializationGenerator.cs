using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorStepsInitializationGenerator : EmulatorClassGenerator
{
    public static readonly EmulatorStepsInitializationGenerator Instance = new();

    private EmulatorStepsInitializationGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{GetEmulatorClassName(context)}.initialization";

    protected override ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration)
    {
        var members = new List<MemberDeclarationSyntax>
        {
            CreateStepsField(),
            CreateOverlapsField(context)
        };

        members.AddRange(context.Configuration.OpcodeStepTables
            .Select(CreateOpcodeStepTableField)
            .Concat(context.SequenceGroups.Values.OrderBy(group => group.Name).Select(CreateSequenceGroupStepTableField))
            .Append(CreateStaticConstructor(context)));

        return classDeclaration.AddMembers(members.ToArray());
    }

    [Pure]
    private static MemberDeclarationSyntax CreateStepsField() =>
        FieldDeclaration(
                VariableDeclaration(
                        ArrayType(IdentifierName(StepStructName))
                            .WithRankSpecifiers([ArrayRankSpecifier([OmittedArraySizeExpression()])]))
                    .WithVariables([VariableDeclarator(Identifier(StepsFieldName))]))
            .WithModifiers([Private, Static, ReadOnly])
            .WithSemicolonToken(Semicolon);

    [Pure]
    private static MemberDeclarationSyntax CreateOverlapsField(GeneratorContext context) =>
        FieldDeclaration(
                VariableDeclaration(
                        ArrayType(CreateOverlapHandlerType(context))
                            .WithRankSpecifiers([ArrayRankSpecifier([OmittedArraySizeExpression()])]))
                    .WithVariables([VariableDeclarator(Identifier(OverlapsFieldName))]))
            .WithModifiers([Private, Static, ReadOnly])
            .WithSemicolonToken(Semicolon);

    [Pure]
    private static StatementSyntax CreateInitializeStepsField(GeneratorContext context)
    {
        var stepCreations = context.AllSteps.Select(step => CreateStep(context, step)).Append(CreateErrorStep());

        var elements = stepCreations.Select(ExpressionElement).ToArray();

        var value = CollectionExpression(SeparatedList<CollectionElementSyntax>(elements));

        var assignment = AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(StepsFieldName), value);

        return ExpressionStatement(assignment);
    }

    [Pure]
    private static StatementSyntax CreateInitializeOverlapsField(GeneratorContext context)
    {
        var overlaps = context.OverlapSteps
            .Select(step => (CollectionElementSyntax)ExpressionElement(CreateOverlap(context, step)))
            .Prepend(ExpressionElement(DefaultExpression(CreateOverlapHandlerType(context))));

        var value = CollectionExpression(SeparatedList(overlaps));

        var assignment = AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(OverlapsFieldName), value);

        return ExpressionStatement(assignment);
    }

    [Pure]
    private static ImplicitObjectCreationExpressionSyntax CreateErrorStep()
    {
        var handler = PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(ErrorMethodName));

        return CreateStepCreation(handler, 0, Action.None, LiteralExpression(SyntaxKind.DefaultLiteralExpression));
    }

    [Pure]
    private static ImplicitObjectCreationExpressionSyntax CreateStep(GeneratorContext context, Step step)
    {
        ExpressionSyntax handler = step.DoesNothing || step.ExecutesAsOverlapOnly
            ? LiteralExpression(SyntaxKind.DefaultLiteralExpression)
            : PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(GetStepMethodName(step)));

        var nextStep = step.NextOpcode switch
        {
            NextOpcodeMode.Read => 0,
            NextOpcodeMode.Overlapped when step.Sequence is PrefixJump => context.OpcodeRead.Steps[1].Index,
            NextOpcodeMode.Overlapped => GetOverlappedNextStep(context, step.Sequence),
            NextOpcodeMode.Custom => context.ErrorStepIndex,
            NextOpcodeMode.Loop => step.Sequence.FirstStep.Index,
            null when step.QueuesOverlapStep => context.OpcodeRead.FirstStep.Index,
            null => step.Index + 1,
            _ => throw new NotSupportedException($"The {nameof(NextOpcodeMode)} {step.NextOpcode} is not supported.")
        };

        ExpressionSyntax overlap = step.ExecutesAsOverlapOnly
            ? PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(GetOverlapMethodName(context, step)))
            : LiteralExpression(SyntaxKind.DefaultLiteralExpression);

        var action = step is { NextOpcode: NextOpcodeMode.Overlapped, Sequence: PrefixJump }
            ? context.OpcodeRead.FirstStep.RequiredAction
            : step.RequiredAction;

        return CreateStepCreation(handler, nextStep, action, overlap);
    }

    [Pure]
    private static int GetOverlappedNextStep(GeneratorContext context, StepSequence sequence) =>
        sequence.OverlappedSequenceName == null
            ? context.OpcodeRead.FirstStep.Index
            : context.GetSequence(sequence.OverlappedSequenceName).FirstStep.Index;

    [Pure]
    private static ImplicitObjectCreationExpressionSyntax CreateStepCreation(ExpressionSyntax handler, int nextStep, Action action, ExpressionSyntax overlap) =>
        ImplicitObjectCreationExpression()
            .WithArgumentList(
                ArgumentList([
                    Argument(handler),
                    Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(nextStep))),
                    Argument(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            // TODO: static using for ActionRequired.
                            IdentifierName(ActionRequiredEnumName),
                            IdentifierName(action.EnumName))),
                    Argument(overlap)
                ]));

    [Pure]
    private static ExpressionSyntax CreateOverlap(GeneratorContext context, Step step) =>
        step.DoesNothing
            ? LiteralExpression(SyntaxKind.DefaultLiteralExpression)
            : PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(GetOverlapMethodName(context, step)));

    [Pure]
    private static MemberDeclarationSyntax CreateOpcodeStepTableField(OpcodeStepTable opcodeStepTable)
    {
        var variableDeclarator = VariableDeclarator(Identifier(opcodeStepTable.FieldName));

        var variable = VariableDeclaration(PreDefinedDataMember.OpcodeStepTable.TypeSyntax)
            .WithVariables(SingletonSeparatedList(variableDeclarator));

        return FieldDeclaration(variable).AddModifiers(Private, Static, ReadOnly);
    }

    [Pure]
    private static MemberDeclarationSyntax CreateSequenceGroupStepTableField(SequenceGroup sequenceGroup)
    {
        var variableDeclarator = VariableDeclarator(Identifier(GetSequenceGroupStepTableFieldName(sequenceGroup)));

        var variable = VariableDeclaration(PreDefinedDataMember.OpcodeStepTable.TypeSyntax)
            .WithVariables(SingletonSeparatedList(variableDeclarator));

        return FieldDeclaration(variable).AddModifiers(Private, Static, ReadOnly);
    }

    [MustUseReturnValue]
    private static ConstructorDeclarationSyntax CreateStaticConstructor(GeneratorContext context)
    {
        var statements = new List<StatementSyntax>
        {
            CreateLittleEndianStatement(context),
            CreateInitializeStepsField(context),
            CreateInitializeOverlapsField(context)
        };

        // This is not totally generic. But it does support everything I need. Doesn't support:
        // - Duplicates with no prefix, i.e. different opcodes.
        // - Duplicates between an item in an opcode table and out.
        var duplicatesWithPrefix = context.Instructions
            .Where(i => i.OpcodeTable == null)
            .SelectMany(i => i.Duplicates)
            .GroupBy(d => context.Configuration.OpcodeStepTables.GetForPrefix(d.Prefix!.Value)) // Assumes no duplicates without a prefix.
            .Select(g => (g.Key, Items: g.Select(d => (d.Opcode, d.Step))));

        var duplicatesWithinOpcodeTable = context.Instructions
            .Where(i => i.OpcodeTable != null)
            .GroupBy(context.Configuration.OpcodeStepTables.GetForInstruction)
            .Select(g => (g.Key, Items: g.SelectMany(i => i.Duplicates.Select(d => (d.Opcode, d.Step)))));

        var duplicatesByOpcodeTable = duplicatesWithPrefix.Concat(duplicatesWithinOpcodeTable).ToDictionary(x => x.Key, x => x.Items.ToList());

        foreach (var group in context.Instructions.GroupBy(context.Configuration.OpcodeStepTables.GetForInstruction))
        {
            statements.Add(CreateOpcodeStepTableInitializationStatement(context, group.Key, group, duplicatesByOpcodeTable.TryGetValue(group.Key, out var duplicates) ? duplicates : []));
        }

        foreach (var sequenceGroup in context.SequenceGroups.Values.OrderBy(group => group.Name))
        {
            statements.Add(CreateSequenceGroupStepTableInitializationStatement(context, sequenceGroup));
        }

        // Static constructor
        return ConstructorDeclaration(GetEmulatorClassName(context))
            .WithModifiers(TokenList(Static))
            .WithBody(Block(statements));
    }

    [Pure]
    private static IfStatementSyntax CreateLittleEndianStatement(GeneratorContext context)
    {
        context.RequiredUsings.Add(typeof(BitConverter).Namespace!);
        context.RequiredUsings.Add(typeof(NotSupportedException).Namespace!);

        // throw new NotSupportedException("Only little endian systems are supported.");
        var throwStatement = ThrowStatement(
                ObjectCreationExpression(IdentifierName(nameof(NotSupportedException)))
                    .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList(
                                Argument(
                                    LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        Literal("Only little endian systems are supported.")))))));

        // #pragma warning disable CA1065
        var pragmaDisable = PragmaWarningDirectiveTrivia(
                Token(SyntaxKind.DisableKeyword),
                SeparatedList<ExpressionSyntax>()
                    .Add(IdentifierName("CA1065")),
                true);

        // #pragma warning enable CA1065
        var pragmaRestore = PragmaWarningDirectiveTrivia(
                Token(SyntaxKind.RestoreKeyword),
                SeparatedList<ExpressionSyntax>()
                    .Add(IdentifierName("CA1065")),
                true);

        // !BitConverter.IsLittleEndian
        var condition = PrefixUnaryExpression(
                SyntaxKind.LogicalNotExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(nameof(BitConverter)),
                    IdentifierName(nameof(BitConverter.IsLittleEndian))));

        // if (!BitConverter.IsLittleEndian)
        // {
        // #pragma warning disable CA1065
        //  throw new NotSupportedException("Only little endian systems are supported.");
        // #pragma warning restore CA1065
        // }
        return IfStatement(
                condition,
                Block(
                    List<StatementSyntax>()
                        .Add(throwStatement
                            .WithLeadingTrivia(Trivia(pragmaDisable))
                            .WithTrailingTrivia(Trivia(pragmaRestore)))));
    }

    [Pure]
    private static StatementSyntax CreateOpcodeStepTableInitializationStatement(GeneratorContext context, OpcodeStepTable opcodeStepTable, [InstantHandle] IEnumerable<Instruction> instructions, IReadOnlyList<(byte Opcode, Step Step)> duplicates)
    {
        var stepIndices = Enumerable.Repeat(context.ErrorStepIndex, 256).ToArray();

        if (opcodeStepTable == OpcodeStepTable.NoPrefix)
        {
            foreach (var prefixJump in context.PrefixJumps.Values)
            {
                stepIndices[prefixJump.Prefix] = prefixJump.FirstStep.Index;
            }
        }

        foreach (var instruction in instructions)
        {
            stepIndices[instruction.Opcode] = instruction.FirstStep.Index;
        }

        foreach (var duplicate in duplicates)
        {
            stepIndices[duplicate.Opcode] = duplicate.Step.Index;
        }

        var literals = stepIndices.Select(index => ExpressionElement(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(index))));

        var value = CollectionExpression(SeparatedList<CollectionElementSyntax>(literals));

        var assignment = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            IdentifierName(opcodeStepTable.FieldName),
            value);

        return ExpressionStatement(assignment);
    }

    [Pure]
    private static StatementSyntax CreateSequenceGroupStepTableInitializationStatement(GeneratorContext context, SequenceGroup sequenceGroup)
    {
        var stepIndices = Enumerable.Repeat(context.ErrorStepIndex, sequenceGroup.MaximumNumber + 1).ToArray();

        foreach (var member in sequenceGroup.Members)
        {
            stepIndices[member.Key] = member.Value.FirstStep.Index;
        }

        var literals = stepIndices.Select(index => ExpressionElement(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(index))));

        var value = CollectionExpression(SeparatedList<CollectionElementSyntax>(literals));

        var assignment = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            IdentifierName(GetSequenceGroupStepTableFieldName(sequenceGroup)),
            value);

        return ExpressionStatement(assignment);
    }
}