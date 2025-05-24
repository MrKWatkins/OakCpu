using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class RegistersClassesGenerator : ClassGenerator
{
    private const string EmulatorFieldName = "emulator";
    public static readonly RegistersClassesGenerator Instance = new();

    private RegistersClassesGenerator()
    {
    }

    protected override IEnumerable<ClassDeclarationSyntax> CreateClasses(HashSet<string> requiredUsings, GeneratorInput input)
    {
        yield return CreateClass(input, null);

        foreach (var category in GetCategories(input))
        {
            yield return CreateClass(input, category);
        }
    }

    private static ClassDeclarationSyntax CreateClass(GeneratorInput input, string? category)
    {
        var members = new List<MemberDeclarationSyntax>
        {
            CreateEmulatorField(),
            CreateConstructor(input, category)
        };
        if (category == null)
        {
            members.AddRange(CreateCategoryProperties(input));
        }
        members.AddRange(CreateRegisterProperties(input, category));

        return SyntaxFactory
            .ClassDeclaration(GetRegistersClassName(category))
            .AddModifiers(Public, Sealed)
            .AddMembers(members.ToArray());
    }

    [Pure]
    private static IEnumerable<PropertyDeclarationSyntax> CreateCategoryProperties(GeneratorInput input) => GetCategories(input).Select(c => CreateGetOnlyProperty(GetRegistersClassName(c), c));

    [Pure]
    private static IEnumerable<PropertyDeclarationSyntax> CreateRegisterProperties(GeneratorInput input, string? category) => input.Registers.Where(r => r.Category == category).OrderBy(r => r.Name).Select(CreateRegisterProperty);

    [Pure]
    private static PropertyDeclarationSyntax CreateRegisterProperty(Register register) =>
        SyntaxFactory
            .PropertyDeclaration(register.Type.PredefinedType(), SyntaxFactory.Identifier(register.PropertyName))
            .WithModifiers(SyntaxFactory.TokenList(Public))
            .WithAccessorList(
                SyntaxFactory.AccessorList(SyntaxFactory.List(
                [
                    SyntaxFactory
                        .AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithExpressionBody(
                            SyntaxFactory.ArrowExpressionClause(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName(EmulatorFieldName),
                                    SyntaxFactory.IdentifierName(register.FieldName))))
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory
                        .AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithExpressionBody(
                            SyntaxFactory.ArrowExpressionClause(
                                SyntaxFactory.AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName(EmulatorFieldName),
                                        SyntaxFactory.IdentifierName(register.FieldName)),
                                    SyntaxFactory.IdentifierName("value"))))
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                ])));

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(GeneratorInput input, string? category)
    {
        var statements = new List<StatementSyntax>
        {
            // this.emulator = emulator;
            SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.ThisExpression(),
                        SyntaxFactory.IdentifierName(EmulatorFieldName)),
                    SyntaxFactory.IdentifierName(EmulatorFieldName)))
        };

        if (category == null)
        {
            foreach (var c in GetCategories(input))
            {
                // Category = new Z80CategoryRegisters(emulator);
                statements.Add(
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            SyntaxFactory.IdentifierName(c),
                            SyntaxFactory
                                .ObjectCreationExpression(SyntaxFactory.IdentifierName(GetRegistersClassName(c)))
                                .WithArgumentList(
                                    SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName(EmulatorFieldName))))))));
            }
        }

        return SyntaxFactory
            .ConstructorDeclaration(GetRegistersClassName(category))
            .WithModifiers(SyntaxFactory.TokenList(Internal))
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory
                            .Parameter(SyntaxFactory.Identifier(EmulatorFieldName))
                            .WithType(SyntaxFactory.IdentifierName(EmulatorClassName)))))
            .WithBody(SyntaxFactory.Block(statements.ToArray()));
    }

    [Pure]
    private static FieldDeclarationSyntax CreateEmulatorField() =>
        SyntaxFactory
            .FieldDeclaration(
                SyntaxFactory
                    .VariableDeclaration(SyntaxFactory.IdentifierName(EmulatorClassName))
                    .WithVariables(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(EmulatorFieldName)))))
            .WithModifiers(SyntaxFactory.TokenList(Private, ReadOnly));

    [Pure]
    private static IEnumerable<string> GetCategories(GeneratorInput input) => input.Registers.Where(r => r.Category != null).Select(r => r.Category!).Distinct().OrderBy(c => c);
}