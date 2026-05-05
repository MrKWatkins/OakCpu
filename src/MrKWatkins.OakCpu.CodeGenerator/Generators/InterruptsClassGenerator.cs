using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.GeneratedNames;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.GeneratorSymbols;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class InterruptsClassGenerator : TypeGenerator
{
    public static readonly InterruptsClassGenerator Instance = new();

    private InterruptsClassGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => GetInterruptsClassName(context);

    [Pure]
    public override IReadOnlyList<GeneratedFile> GenerateFiles(GeneratorContext context) => GenerateOneFilePerType(context);

    protected override IEnumerable<BaseTypeDeclarationSyntax> CreateTypes(GeneratorContext context)
    {
        yield return CreateBaseClass(context);
        yield return CreateConcreteClass(context, GetStepInterruptsClassName(context), GetEmulatorClassIdentifier(context), instructionEmulator: false);
        yield return CreateConcreteClass(context, GetInstructionInterruptsClassName(context), GetInstructionEmulatorClassIdentifier(context), instructionEmulator: true);
    }

    [Pure]
    private static ClassDeclarationSyntax CreateBaseClass(GeneratorContext context)
    {
        var members = CreateInterruptProperties(context, createOverrideProperty: false).Cast<MemberDeclarationSyntax>().ToArray();

        return WithXmlDocumentation(
            ClassDeclaration(GetInterruptsClassName(context))
                .AddModifiers(Public, Abstract)
                .AddMembers(members),
            $"Provides access to the {context.Cpu.Name} interrupt state.");
    }

    [Pure]
    private static ClassDeclarationSyntax CreateConcreteClass(GeneratorContext context, string className, TypeSyntax emulatorType, bool instructionEmulator)
    {
        var members = new List<MemberDeclarationSyntax>
        {
            CreateEmulatorField(emulatorType),
            CreateConstructor(className, emulatorType)
        };
        members.AddRange(CreateInterruptProperties(context, createOverrideProperty: true, instructionEmulator));

        return ClassDeclaration(className)
            .AddModifiers(Internal, Sealed)
            .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(IdentifierName(GetInterruptsClassName(context))))))
            .AddMembers(members.ToArray());
    }

    [Pure]
    private static IEnumerable<PropertyDeclarationSyntax> CreateInterruptProperties(GeneratorContext context, bool createOverrideProperty, bool instructionEmulator = false) =>
        context.Interrupts.Properties.Values
            .OrderBy(property => property.PropertyName)
            .Select(property => CreateInterruptProperty(context, property, createOverrideProperty, instructionEmulator));

    [Pure]
    private static PropertyDeclarationSyntax CreateInterruptProperty(GeneratorContext context, UserDefinedDataMember property, bool createOverrideProperty, bool instructionEmulator)
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
    private static PropertyDeclarationSyntax CreateInstructionEmulatorHaltedProperty(GeneratorContext context, UserDefinedDataMember property, ExpressionSyntax memberAccessExpression)
    {
        var nextSequenceStepExpression = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(EmulatorFieldName),
            IdentifierName(NextSequenceStepFieldName));
        var haltedStep = CastExpression(
            UShortType,
            LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(context.GetInstructionEmulatorSequenceIndex(context.Interrupts.Halted))));
        var noNextSequenceStep = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(GetInstructionEmulatorClassName(context)),
            IdentifierName(NoNextSequenceStepFieldName));

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

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(string className, TypeSyntax emulatorType) =>
        ConstructorDeclaration(className)
            .WithModifiers(TokenList(Internal))
            .WithParameterList(
                ParameterList(
                    SingletonSeparatedList(
                        Parameter(Identifier(EmulatorFieldName))
                            .WithType(emulatorType))))
            .WithBody(Block(CreateAssignEmulatorFieldExpression()));
}