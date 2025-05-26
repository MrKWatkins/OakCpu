using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        EmulatorFieldsPropertiesAndConstructorGenerator.Instance,
        EmulatorStepGenerator.Instance,
        FlagsClassGenerator.Instance,
        RegistersClassesGenerator.Instance];

    public string FileName => GetType().Name.Substring(0, GetType().Name.Length - "Generator".Length);

    [Pure]
    public CompilationUnitSyntax Generate(GeneratorInput input)
    {
        var requiredUsings = new HashSet<string>();

        var classDeclarations = CreateTypes(requiredUsings, input).ToArray<MemberDeclarationSyntax>();

        return SyntaxFactory
            .CompilationUnit()
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
        SyntaxFactory
            .PropertyDeclaration(SyntaxFactory.IdentifierName(typeName), SyntaxFactory.Identifier(propertyName))
            .WithModifiers(SyntaxFactory.TokenList(Public))
            .WithAccessorList(
                SyntaxFactory.AccessorList(
                    SyntaxFactory.SingletonList(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Semicolon))));

    [Pure]
    protected static PropertyDeclarationSyntax CreateGetSetProperty(TypeSyntax type, string propertyName) =>
        SyntaxFactory
            .PropertyDeclaration(type, SyntaxFactory.Identifier(propertyName))
            .WithModifiers(SyntaxFactory.TokenList(Public))
            .WithAccessorList(
                SyntaxFactory.AccessorList(SyntaxFactory.List([
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Semicolon),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Semicolon)
                ])));

    [Pure]
    protected static PropertyDeclarationSyntax CreateGetSetProperty(TypeSyntax type, string propertyName, ExpressionSyntax getExpression, ExpressionSyntax setExpression) =>
        SyntaxFactory
            .PropertyDeclaration(type, SyntaxFactory.Identifier(propertyName))
            .WithModifiers(SyntaxFactory.TokenList(Public))
            .WithAccessorList(
                SyntaxFactory.AccessorList(SyntaxFactory.List(
                [
                    SyntaxFactory
                        .AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(getExpression))
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),

                    SyntaxFactory
                        .AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(setExpression))
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                ])));
    [Pure]
    protected static FieldDeclarationSyntax CreateEmulatorField(GeneratorInput input) =>
        SyntaxFactory
            .FieldDeclaration(
                SyntaxFactory
                    .VariableDeclaration(GetEmulatorClassIdentifier(input))
                    .WithVariables(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(EmulatorFieldName)))))
            .WithModifiers(SyntaxFactory.TokenList(Private, ReadOnly));

    [Pure]
    protected static ExpressionStatementSyntax CreateNewObjectAndAssignToProperty(string propertyName, string classToCreateName, params ExpressionSyntax[] constructorArguments) =>
        SyntaxFactory.ExpressionStatement(
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName(propertyName),
                SyntaxFactory
                    .ObjectCreationExpression(SyntaxFactory.IdentifierName(classToCreateName))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList(constructorArguments.Select(SyntaxFactory.Argument).ToArray())))));

    [Pure]
    protected static ExpressionStatementSyntax CreateAssignEmulatorFieldExpression() =>
        // this.emulator = emulator;
        SyntaxFactory.ExpressionStatement(
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.ThisExpression(),
                    SyntaxFactory.IdentifierName(EmulatorFieldName)),
                SyntaxFactory.IdentifierName(EmulatorFieldName)));

    [Pure]
    protected static string GetEmulatorClassName(GeneratorInput input) => $"{input.Cpu.Name}Emulator";

    [Pure]
    protected static IdentifierNameSyntax GetEmulatorClassIdentifier(GeneratorInput input) => SyntaxFactory.IdentifierName(GetEmulatorClassName(input));

    [Pure]
    protected static string GetRegistersClassName(GeneratorInput input, string? category = null) => $"{input.Cpu.Name}{category}Registers";

    [Pure]
    protected static IdentifierNameSyntax GetRegistersClassIdentifier(GeneratorInput input, string? category = null) => SyntaxFactory.IdentifierName(GetRegistersClassName(input, category));

    [Pure]
    protected static string GetFlagsClassName(GeneratorInput input) => $"{input.Cpu.Name}Flags";

    [Pure]
    protected static IdentifierNameSyntax GetFlagsClassIdentifier(GeneratorInput input) => SyntaxFactory.IdentifierName(GetFlagsClassName(input));

    [Pure]
    private static UsingDirectiveSyntax CreateUsingStatement(string @namespace) => SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(@namespace));
}