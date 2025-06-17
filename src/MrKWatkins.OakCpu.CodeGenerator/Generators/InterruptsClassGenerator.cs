using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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
        context.Interrupts.Properties
            .OrderBy(p => p)
            .Select(p => CreateInterruptProperty(context, p, context.Configuration.UserDefinedDataMembers[p]));

    [Pure]
    private static PropertyDeclarationSyntax CreateInterruptProperty(GeneratorContext context, string property, UserDefinedDataMember field)
    {
        var memberAccessExpression = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(EmulatorFieldName),
            IdentifierName(field.FieldName));

        var getExpression = memberAccessExpression;

        var setExpression = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            memberAccessExpression,
            IdentifierName("value"));

        return CreateGetSetProperty(context, field.TypeSyntax, property.ToUpperCamelCaseFromSnakeCase(), getExpression, setExpression);
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
            .WithBody(Block(statements.ToArray()));
    }
}