using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
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

    protected override ClassDeclarationSyntax PopulateClass(FileGeneratorContext context, ClassDeclarationSyntax classDeclaration)
    {
        var generatorContext = context.GeneratorContext;
        var members = new List<MemberDeclarationSyntax>
        {
            CreateSequenceStartConstant(generatorContext, "OpcodeRead", generatorContext.OpcodeRead)
        };
        if (generatorContext.Interrupts.Halted is not null)
        {
            members.Add(CreateSequenceStartConstant(generatorContext, "Halted", generatorContext.Interrupts.Halted));
        }
        members.AddRange(generatorContext.GetSequenceGroup(InterruptMode.SequenceGroupName).Members
            .OrderBy(mode => mode.Key)
            .Select(mode => CreateSequenceStartConstant(generatorContext, $"IM{mode.Key}", mode.Value)));

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
    private static MemberDeclarationSyntax CreateSequenceStartConstant(GeneratorContext context, string name, StepSequence sequence) =>
        FieldDeclaration(
                VariableDeclaration(UShortType)
                    .WithVariables(
                    [
                        VariableDeclarator($"{name}Step0").WithInitializer(EqualsValueClause(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(context.GetStepLayout(sequence.FirstStep).Index))))
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
    private static MemberDeclarationSyntax CreateOverlapsField(FileGeneratorContext context) =>
        FieldDeclaration(
                VariableDeclaration(
                        ArrayType(CreateOverlapHandlerType(context))
                            .WithRankSpecifiers([ArrayRankSpecifier([OmittedArraySizeExpression()])]))
                    .WithVariables([VariableDeclarator(Identifier(Field.Name.Overlaps))]))
            .WithModifiers([Private, Static, ReadOnly])
            .WithSemicolonToken(Semicolon);

    [Pure]
    private static StatementSyntax CreateInitializeStepsField(FileGeneratorContext context)
    {
        var stepCreations = context.GeneratorContext.AllSteps.Select(step => CreateStep(context, step)).Append(CreateErrorStep());

        var elements = stepCreations.Select(ExpressionElement).ToArray();

        var value = CollectionExpression(SeparatedList<CollectionElementSyntax>(elements));

        var assignment = AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(StepsFieldName), value);

        return ExpressionStatement(assignment);
    }

    [Pure]
    private static StatementSyntax CreateInitializeOverlapsField(FileGeneratorContext context)
    {
        var overlaps = context.GeneratorContext.OverlapSteps
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
    private static ImplicitObjectCreationExpressionSyntax CreateStep(FileGeneratorContext context, Step step)
    {
        var generatorContext = context.GeneratorContext;
        var stepLayout = generatorContext.GetStepLayout(step);

        ExpressionSyntax handler = stepLayout.DoesNothing || stepLayout.ExecutesAsOverlapOnly
            ? LiteralExpression(SyntaxKind.DefaultLiteralExpression)
            : PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(Method.Name.Step(generatorContext, step)));

        var nextStep = StepMetadata.GetNextStep(generatorContext, step);

        ExpressionSyntax overlap = stepLayout.ExecutesAsOverlapOnly
            ? PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(Method.Name.Overlap(context, step)))
            : LiteralExpression(SyntaxKind.DefaultLiteralExpression);

        var action = StepMetadata.GetAction(generatorContext, step);

        return CreateStepCreation(handler, nextStep, action, overlap);
    }

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
    private static ExpressionSyntax CreateOverlap(FileGeneratorContext context, Step step) =>
        context.GeneratorContext.GetStepLayout(step).DoesNothing
            ? LiteralExpression(SyntaxKind.DefaultLiteralExpression)
            : PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(Method.Name.Overlap(context, step)));

    [MustUseReturnValue]
    private static ConstructorDeclarationSyntax CreateStaticConstructor(FileGeneratorContext context)
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
    private static StatementSyntax CreateOpcodeStepTableInitializationStatement(FileGeneratorContext context, OpcodeStepTable opcodeStepTable, [InstantHandle] IEnumerable<Instruction> instructions, IReadOnlyList<(byte Opcode, Step Step)> duplicates)
    {
        var stepIndices = Enumerable.Repeat(context.GeneratorContext.ErrorStepIndex, 256).ToArray();

        if (opcodeStepTable == OpcodeStepTable.NoPrefix)
        {
            foreach (var prefixJump in context.GeneratorContext.PrefixJumps.Values)
            {
                stepIndices[prefixJump.Prefix] = context.GeneratorContext.GetStepLayout(prefixJump.FirstStep).Index;
            }
        }

        foreach (var instruction in instructions)
        {
            stepIndices[instruction.Opcode] = context.GeneratorContext.GetStepLayout(instruction.FirstStep).Index;
        }

        foreach (var duplicate in duplicates)
        {
            stepIndices[duplicate.Opcode] = context.GeneratorContext.GetStepLayout(duplicate.Step).Index;
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
    private static StatementSyntax CreateSequenceGroupStepTableInitializationStatement(FileGeneratorContext context, SequenceGroup sequenceGroup)
    {
        var stepIndices = Enumerable.Repeat(context.GeneratorContext.ErrorStepIndex, sequenceGroup.MaximumNumber + 1).ToArray();

        foreach (var member in sequenceGroup.Members)
        {
            stepIndices[member.Key] = context.GeneratorContext.GetStepLayout(member.Value.FirstStep).Index;
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