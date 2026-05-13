using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Field = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Field;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorStepsInitializationGenerator : EmulatorClassGenerator
{
    public static readonly EmulatorStepsInitializationGenerator Instance = new();

    private EmulatorStepsInitializationGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{Class.Name.Emulator(context)}.initialization";

    protected override ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration)
    {
        var members = new List<MemberDeclarationSyntax>
        {
            CreateSequenceStartConstant("OpcodeRead", context.OpcodeRead),
            CreateSequenceStartConstant("Halted", context.Interrupts.Halted)
        };
        members.AddRange(context.GetSequenceGroup(InterruptMode.SequenceGroupName).Members
            .OrderBy(mode => mode.Key)
            .Select(mode => CreateSequenceStartConstant($"IM{mode.Key}", mode.Value)));

        members.AddRange(
        [
            CreateStepsField(),
            CreateOverlapsField(context)
        ]);

        members.AddRange(TableGeneration.CreateStepTableFields(context, Field.Name.SequenceGroupStepTable));
        members.Add(CreateStaticConstructor(context));

        return classDeclaration.AddMembers(members.ToArray());
    }

    [Pure]
    private static MemberDeclarationSyntax CreateSequenceStartConstant(string name, StepSequence sequence) =>
        FieldDeclaration(
                VariableDeclaration(UShortType)
                    .WithVariables(
                    [
                        VariableDeclarator($"{name}Step0").WithInitializer(EqualsValueClause(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(sequence.FirstStep.Index))))
                    ]))
            .WithModifiers([Private, Token(SyntaxKind.ConstKeyword)]);

    [Pure]
    private static MemberDeclarationSyntax CreateStepsField() =>
        FieldDeclaration(
                VariableDeclaration(
                        ArrayType(IdentifierName(TypeName.StepStruct))
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
                    .WithVariables([VariableDeclarator(Identifier(Field.Name.Overlaps))]))
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

        var assignment = AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(Field.Name.Overlaps), value);

        return ExpressionStatement(assignment);
    }

    [Pure]
    private static ImplicitObjectCreationExpressionSyntax CreateErrorStep()
    {
        var handler = PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(Method.Name.Error));

        return CreateStepCreation(handler, 0, Action.None, LiteralExpression(SyntaxKind.DefaultLiteralExpression));
    }

    [Pure]
    private static ImplicitObjectCreationExpressionSyntax CreateStep(GeneratorContext context, Step step)
    {
        ExpressionSyntax handler = step.DoesNothing || step.ExecutesAsOverlapOnly
            ? LiteralExpression(SyntaxKind.DefaultLiteralExpression)
            : PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(Method.Name.Step(step)));

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
            ? PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(Method.Name.Overlap(context, step)))
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
                            IdentifierName(TypeName.ActionRequiredEnum),
                            IdentifierName(action.EnumName))),
                    Argument(overlap)
                ]));

    [Pure]
    private static ExpressionSyntax CreateOverlap(GeneratorContext context, Step step) =>
        step.DoesNothing
            ? LiteralExpression(SyntaxKind.DefaultLiteralExpression)
            : PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(Method.Name.Overlap(context, step)));

    [MustUseReturnValue]
    private static ConstructorDeclarationSyntax CreateStaticConstructor(GeneratorContext context)
    {
        var statements = new List<StatementSyntax>
        {
            TableGeneration.CreateLittleEndianStatement(context),
            CreateInitializeStepsField(context),
            CreateInitializeOverlapsField(context)
        };

        statements.AddRange(TableGeneration.CreateTableInitializationStatements(context, CreateOpcodeStepTableInitializationStatement, CreateSequenceGroupStepTableInitializationStatement));

        // Static constructor
        return ConstructorDeclaration(Class.Name.Emulator(context))
            .WithModifiers(TokenList(Static))
            .WithBody(Block(statements));
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
            IdentifierName(Field.Name.SequenceGroupStepTable(sequenceGroup)),
            value);

        return ExpressionStatement(assignment);
    }
}