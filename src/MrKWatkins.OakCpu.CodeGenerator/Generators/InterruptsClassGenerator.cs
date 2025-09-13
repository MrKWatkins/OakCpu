using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class InterruptsClassGenerator : TypeGenerator
{
    public static readonly InterruptsClassGenerator Instance = new();

    private InterruptsClassGenerator()
    {
    }

    protected override IEnumerable<BaseTypeDeclarationSyntax> CreateTypes(GeneratorContext context)
    {
        yield return CreateClass(context);
    }

    private static ClassDeclarationSyntax CreateClass(GeneratorContext context)
    {
        var members = new List<MemberDeclarationSyntax>
        {
            CreateEmulatorField(context),
            CreateConstructor(context)
        };
        members.AddRange(CreateInterruptProperties(context));

        return ClassDeclaration(GetInterruptsClassName(context))
            .AddModifiers(Public, Sealed)
            .AddMembers(members.ToArray());
    }

    [Pure]
    private static IEnumerable<PropertyDeclarationSyntax> CreateInterruptProperties(GeneratorContext context) =>
        context.Interrupts.Properties.Values
            .OrderBy(p => p.PropertyName)
            .Select(p => CreateInterruptProperty(context, p));

    [Pure]
    private static PropertyDeclarationSyntax CreateInterruptProperty(GeneratorContext context, UserDefinedDataMember property)
    {
        var memberAccessExpression = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(EmulatorFieldName),
            IdentifierName(property.FieldName));

        var getExpression = memberAccessExpression;

        var setExpression = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            memberAccessExpression,
            IdentifierName("value"));

        return CreateGetSetProperty(context, property.TypeSyntax, property.PropertyName, getExpression, setExpression);
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(GeneratorContext context)
    {
        var statements = new List<StatementSyntax>
        {
            CreateAssignEmulatorFieldExpression()
        };

        return ConstructorDeclaration(GetInterruptsClassName(context))
            .WithModifiers(TokenList(Internal))
            .WithParameterList(
                ParameterList(
                    SingletonSeparatedList(
                        Parameter(Identifier(EmulatorFieldName))
                            .WithType(GetEmulatorClassIdentifier(context)))))
            .WithBody(Block(statements.ToArray()))
            .WithLeadingTrivia(NewlineComment);
    }
}