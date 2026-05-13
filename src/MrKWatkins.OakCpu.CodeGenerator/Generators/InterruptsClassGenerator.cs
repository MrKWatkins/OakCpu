using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Field = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Field;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

/// <summary>
/// Generates the interrupts facade classes for both emulator variants.
/// </summary>
public sealed class InterruptsClassGenerator : TypeGenerator
{
    public static readonly InterruptsClassGenerator Instance = new();

    private InterruptsClassGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => Class.Name.Interrupts(context);

    [Pure]
    public override IReadOnlyList<GeneratedFile> GenerateFiles(GeneratorContext context) => GenerateOneFilePerType(context);

    protected override IEnumerable<BaseTypeDeclarationSyntax> CreateTypes(FileGeneratorContext context)
    {
        yield return CreateBaseClass(context);
        yield return CreateConcreteClass(context, Class.Name.StepInterrupts(context), Class.Identifier.Emulator(context), instructionEmulator: false);
        yield return CreateConcreteClass(context, Class.Name.InstructionInterrupts(context), Class.Identifier.InstructionEmulator(context), instructionEmulator: true);
    }

    [Pure]
    private static ClassDeclarationSyntax CreateBaseClass(FileGeneratorContext context)
    {
        var members = CreateInterruptProperties(context, createOverrideProperty: false).Cast<MemberDeclarationSyntax>().ToArray();

        return CreateFacadeBaseClass(Class.Name.Interrupts(context), $"Provides access to the {context.GeneratorContext.Cpu.Name} interrupt state.", members);
    }

    [Pure]
    private static ClassDeclarationSyntax CreateConcreteClass(FileGeneratorContext context, string className, TypeSyntax emulatorType, bool instructionEmulator)
    {
        var members = CreateInterruptProperties(context, createOverrideProperty: true, instructionEmulator).Cast<MemberDeclarationSyntax>().ToArray();
        return CreateFacadeConcreteClass(
            className,
            Class.Name.Interrupts(context),
            emulatorType,
            CreateFacadeConstructor(className, emulatorType),
            members);
    }

    [Pure]
    private static IEnumerable<PropertyDeclarationSyntax> CreateInterruptProperties(FileGeneratorContext context, bool createOverrideProperty, bool instructionEmulator = false) =>
        context.GeneratorContext.Interrupts.Properties.Values
            .OrderBy(property => property.PropertyName)
            .Select(property => CreateInterruptProperty(context, property, createOverrideProperty, instructionEmulator));

    [Pure]
    private static PropertyDeclarationSyntax CreateInterruptProperty(FileGeneratorContext context, UserDefinedDataMember property, bool createOverrideProperty, bool instructionEmulator)
    {
        if (!createOverrideProperty)
        {
            return WithXmlDocumentation(CreateAbstractGetSetProperty(property.TypeSyntax, property.PropertyName), property.Documentation);
        }

        var memberAccessExpression = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(EmulatorFieldName),
            IdentifierName(property.FieldName));

        if (instructionEmulator && string.Equals(property.FieldName, "halted", StringComparison.Ordinal))
        {
            return CreateInstructionEmulatorHaltedProperty(context, property, memberAccessExpression);
        }

        return CreateOverrideGetSetProperty(
            context,
            property.TypeSyntax,
            property.PropertyName,
            memberAccessExpression,
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                memberAccessExpression,
                IdentifierName("value")));
    }

    [Pure]
    private static PropertyDeclarationSyntax CreateInstructionEmulatorHaltedProperty(FileGeneratorContext context, UserDefinedDataMember property, ExpressionSyntax memberAccessExpression)
    {
        var nextSequenceStepExpression = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(EmulatorFieldName),
            IdentifierName(Field.Name.NextSequenceStep));
        var haltedStep = CastExpression(
            UShortType,
            LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(context.GeneratorContext.GetInstructionEmulatorSequenceIndex(context.GeneratorContext.Interrupts.Halted))));
        var noNextSequenceStep = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(Class.Name.InstructionEmulator(context)),
            IdentifierName(Field.Name.NoNextSequenceStep));

        return PropertyDeclaration(property.TypeSyntax, Identifier(property.PropertyName))
            .WithModifiers(TokenList(Public, Override))
            .WithAccessorList(
                AccessorList(
                [
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithExpressionBody(ArrowExpressionClause(memberAccessExpression))
                        .WithAttributeLists([AttributeList([CreateMethodImplAttribute(context.RequiredUsings, System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)])])
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithBody(
                            Block(
                                List<StatementSyntax>(
                                [
                                    ExpressionStatement(
                                        AssignmentExpression(
                                            SyntaxKind.SimpleAssignmentExpression,
                                            memberAccessExpression,
                                            IdentifierName("value"))),
                                    ExpressionStatement(
                                        AssignmentExpression(
                                            SyntaxKind.SimpleAssignmentExpression,
                                            nextSequenceStepExpression,
                                            ConditionalExpression(
                                                IdentifierName("value"),
                                                haltedStep,
                                                ConditionalExpression(
                                                    BinaryExpression(
                                                        SyntaxKind.EqualsExpression,
                                                        nextSequenceStepExpression,
                                                        haltedStep),
                                                    noNextSequenceStep,
                                                    nextSequenceStepExpression))))
                                ])))
                ]));
    }

}