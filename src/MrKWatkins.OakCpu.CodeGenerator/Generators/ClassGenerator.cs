using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class ClassGenerator : Generator
{
    protected const string EmulatorFieldName = "emulator";

    private protected ClassGenerator()
    {
    }

    public static readonly IReadOnlyList<ClassGenerator> AllGenerators = [
        ActionRequiredGenerator.Instance,
        EmulatorStaticFieldsAndConstructorGenerator.Instance,
        EmulatorInstanceFieldsPropertiesAndConstructorGenerator.Instance,
        EmulatorStepGenerator.Instance,
        FlagsClassGenerator.Instance,
        RegistersClassesGenerator.Instance];

    public string FileName => GetType().Name.Substring(0, GetType().Name.Length - "Generator".Length);

    [Pure]
    public CompilationUnitSyntax Generate(GeneratorInput input)
    {
        var requiredUsings = new HashSet<string>();

        var classDeclarations = CreateTypes(requiredUsings, input).ToArray<MemberDeclarationSyntax>();

        return CompilationUnit()
            .AddUsings(requiredUsings.OrderBy(n => n).Select(CreateUsingStatement).ToArray())
            .AddMembers(
                input
                    .CreateRootNamespaceDeclaration()
                    .AddMembers(classDeclarations))
            .NormalizeWhitespace();
    }

    [MustUseReturnValue]
    protected virtual IEnumerable<BaseTypeDeclarationSyntax> CreateTypes(HashSet<string> requiredUsings, GeneratorInput input)
    {
        yield return CreateType(requiredUsings, input);
    }

    [MustUseReturnValue]
    protected virtual BaseTypeDeclarationSyntax CreateType(HashSet<string> requiredUsings, GeneratorInput input)
    {
        throw new NotImplementedException($"{nameof(CreateType)} is not implemented and {nameof(CreateTypes)} has not been overridden.");
    }

    [Pure]
    protected static PropertyDeclarationSyntax CreateGetOnlyProperty(string typeName, string propertyName) =>
        PropertyDeclaration(IdentifierName(typeName), Identifier(propertyName))
            .WithModifiers(TokenList(Public))
            .WithAccessorList(
                AccessorList(
                    SingletonList(AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Semicolon))));

    [Pure]
    protected static PropertyDeclarationSyntax CreateGetSetProperty(TypeSyntax type, string propertyName) =>
        PropertyDeclaration(type, Identifier(propertyName))
            .WithModifiers(TokenList(Public))
            .WithAccessorList(
                AccessorList(List([
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Semicolon),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Semicolon)
                ])));

    [Pure]
    protected static PropertyDeclarationSyntax CreateGetSetProperty(TypeSyntax type, string propertyName, ExpressionSyntax getExpression, ExpressionSyntax setExpression) =>
        PropertyDeclaration(type, Identifier(propertyName))
            .WithModifiers(TokenList(Public))
            .WithAccessorList(
                AccessorList(List(
                [
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithExpressionBody(ArrowExpressionClause(getExpression))
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),

                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithExpressionBody(ArrowExpressionClause(setExpression))
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                ])));
    [Pure]
    protected static FieldDeclarationSyntax CreateEmulatorField(GeneratorInput input) =>
        FieldDeclaration(
                VariableDeclaration(GetEmulatorClassIdentifier(input))
                    .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(EmulatorFieldName)))))
            .WithModifiers(TokenList(Private, ReadOnly));

    [Pure]
    protected static ExpressionStatementSyntax CreateNewObjectAndAssignToProperty(string propertyName, string classToCreateName, params ExpressionSyntax[] constructorArguments) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(propertyName),
                ObjectCreationExpression(IdentifierName(classToCreateName))
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList(constructorArguments.Select(Argument).ToArray())))));

    [Pure]
    protected static ExpressionStatementSyntax CreateAssignEmulatorFieldExpression() =>
        // this.emulator = emulator;
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    ThisExpression(),
                    IdentifierName(EmulatorFieldName)),
                IdentifierName(EmulatorFieldName)));

    [Pure]
    protected static string GetEmulatorClassName(GeneratorInput input) => $"{input.Cpu.Name}Emulator";

    [Pure]
    protected static IdentifierNameSyntax GetEmulatorClassIdentifier(GeneratorInput input) => IdentifierName(GetEmulatorClassName(input));

    [Pure]
    protected static string GetRegistersClassName(GeneratorInput input, string? category = null) => $"{input.Cpu.Name}{category}Registers";

    [Pure]
    protected static IdentifierNameSyntax GetRegistersClassIdentifier(GeneratorInput input, string? category = null) => IdentifierName(GetRegistersClassName(input, category));

    [Pure]
    protected static string GetFlagsClassName(GeneratorInput input) => $"{input.Cpu.Name}Flags";

    [Pure]
    protected static IdentifierNameSyntax GetFlagsClassIdentifier(GeneratorInput input) => IdentifierName(GetFlagsClassName(input));

    [Pure]
    private static UsingDirectiveSyntax CreateUsingStatement(string @namespace) => UsingDirective(IdentifierName(@namespace));
}