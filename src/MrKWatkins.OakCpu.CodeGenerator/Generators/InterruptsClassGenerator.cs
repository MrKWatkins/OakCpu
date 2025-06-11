using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class InterruptsClassGenerator : ClassGenerator
{
    public static readonly InterruptsClassGenerator Instance = new();

    private InterruptsClassGenerator()
    {
    }

    protected override IEnumerable<BaseTypeDeclarationSyntax> CreateTypes(HashSet<string> requiredUsings, GeneratorInput input)
    {
        yield return CreateClass(input);
    }

    private static ClassDeclarationSyntax CreateClass(GeneratorInput input)
    {
        var members = new List<MemberDeclarationSyntax>
        {
            CreateEmulatorField(input),
            CreateConstructor(input)
        };
        members.AddRange(CreateInterruptProperties(input));

        return ClassDeclaration(GetInterruptsClassName(input))
            .AddModifiers(Public, Sealed)
            .AddMembers(members.ToArray());
    }

    [Pure]
    private static IEnumerable<PropertyDeclarationSyntax> CreateInterruptProperties(GeneratorInput input) =>
        input.Interrupts.Properties
            .OrderBy(p => p)
            .Select(p => CreateInterruptProperty(p, input.Configuration.UserDefinedDataMembers[p]));

    [Pure]
    private static PropertyDeclarationSyntax CreateInterruptProperty(string property, UserDefinedDataMember field)
    {
        var memberAccessExpression = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(EmulatorFieldName),
            IdentifierName(field.MemberName));

        var getExpression = memberAccessExpression;

        var setExpression = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            memberAccessExpression,
            IdentifierName("value"));

        return CreateGetSetProperty(field.TypeSyntax, property.ToUpperCamelCaseFromSnakeCase(), getExpression, setExpression);
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(GeneratorInput input)
    {
        var statements = new List<StatementSyntax>
        {
            CreateAssignEmulatorFieldExpression()
        };

        return ConstructorDeclaration(GetInterruptsClassName(input))
            .WithModifiers(TokenList(Internal))
            .WithParameterList(
                ParameterList(
                    SingletonSeparatedList(
                        Parameter(Identifier(EmulatorFieldName))
                            .WithType(GetEmulatorClassIdentifier(input)))))
            .WithBody(Block(statements.ToArray()));
    }
}