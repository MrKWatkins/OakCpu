using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class TypeGenerator : Generator
{
    protected const string EmulatorFieldName = SyntaxHelpers.EmulatorFieldName;

    private protected TypeGenerator()
    {
    }

    public static readonly IReadOnlyList<TypeGenerator> AllGenerators = [
        ActionRequiredGenerator.Instance,
        EmulatorInstanceDataMembersAndConstructorGenerator.Instance,
        EmulatorInterruptsGenerator.Instance,
        EmulatorResetGenerator.Instance,
        EmulatorSerializationGenerator.Instance,
        EmulatorStepsInitializationGenerator.Instance,
        EmulatorStepsGenerator.Instance,
        FlagsClassGenerator.Instance,
        InterruptsClassGenerator.Instance,
        StepStructGenerator.Instance,
        RegistersClassesGenerator.Instance];

    public string FileName => GetType().Name.Substring(0, GetType().Name.Length - "Generator".Length);

    [Pure]
    public string Generate(GeneratorContext context) =>
        // Filthy hackery to put some newlines and indents where we want because NormalizeWhitespace will remove any normal whitespace we add.
        Regex.Replace(GenerateCompilationUnit(context).ToFullString().Replace("// Newline", ""), "// Indent\r?\n\\s*", "    ");

    [Pure]
    public CompilationUnitSyntax GenerateCompilationUnit(GeneratorContext context)
    {
        context = context.WithRequiredUsings();

        var classDeclarations = CreateTypes(context).ToArray<MemberDeclarationSyntax>();

        return CompilationUnit()
            .AddUsings(context.RequiredUsings.OrderBy(n => n).Select(CreateUsingStatement).ToArray())
            .AddMembers(
                context
                    .CreateRootNamespaceDeclaration()
                    .AddMembers(classDeclarations))
            .NormalizeWhitespace();
    }

    [MustUseReturnValue]
    protected virtual IEnumerable<BaseTypeDeclarationSyntax> CreateTypes(GeneratorContext context)
    {
        yield return CreateType(context);
    }

    [MustUseReturnValue]
    protected virtual BaseTypeDeclarationSyntax CreateType(GeneratorContext context)
    {
        throw new NotImplementedException($"{nameof(CreateType)} is not implemented and {nameof(CreateTypes)} has not been overridden.");
    }

    [Pure]
    protected static PropertyDeclarationSyntax CreateGetOnlyProperty(GeneratorContext context, string typeName, string propertyName) =>
        PropertyDeclaration(IdentifierName(typeName), Identifier(propertyName))
            .WithModifiers(TokenList(Public))
            .WithAccessorList(
                AccessorList(
                [
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithAttributeLists([AttributeList([SyntaxHelpers.CreateMethodImplAttribute(context.RequiredUsings, MethodImplOptions.AggressiveInlining)]).WithLeadingTrivia(NewlineComment)])
                        .WithSemicolonToken(Semicolon)
                        .WithTrailingTrivia(NewlineComment, IndentComment)
                ])
                .WithLeadingTrivia(NewlineComment));

    [Pure]
    protected static PropertyDeclarationSyntax CreateGetSetProperty(GeneratorContext context, TypeSyntax type, string propertyName, ExpressionSyntax getExpression, ExpressionSyntax setExpression) =>
        PropertyDeclaration(type, Identifier(propertyName))
            .WithModifiers(TokenList(Public))
            .WithAccessorList(
                AccessorList(List(
                    [
                        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithExpressionBody(ArrowExpressionClause(getExpression))
                            .WithAttributeLists([AttributeList([SyntaxHelpers.CreateMethodImplAttribute(context.RequiredUsings, MethodImplOptions.AggressiveInlining)]).WithLeadingTrivia(NewlineComment)])
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),

                        AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithExpressionBody(ArrowExpressionClause(setExpression))
                            .WithAttributeLists([AttributeList([SyntaxHelpers.CreateMethodImplAttribute(context.RequiredUsings, MethodImplOptions.AggressiveInlining)]).WithLeadingTrivia(NewlineComment)])
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                            .WithTrailingTrivia(NewlineComment, IndentComment)
                    ]))
                    .WithLeadingTrivia(NewlineComment).WithTrailingTrivia(NewlineComment, NewlineComment));
    [Pure]
    protected static FieldDeclarationSyntax CreateEmulatorField(GeneratorContext context) =>
        FieldDeclaration(
                VariableDeclaration(GetEmulatorClassIdentifier(context))
                    .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(EmulatorFieldName)))))
            .WithModifiers(TokenList(Private, ReadOnly));

    [Pure]
    protected static ExpressionStatementSyntax CreateNewObjectAndAssignToProperty(string propertyName, string classToCreateName, params ExpressionSyntax[] constructorArguments) =>
        SyntaxHelpers.CreateNewObjectAndAssignToProperty(propertyName, classToCreateName, constructorArguments);

    [Pure]
    protected static ExpressionStatementSyntax CreateAssignEmulatorFieldExpression() =>
        SyntaxHelpers.CreateAssignEmulatorFieldExpression();

    [MustUseReturnValue]
    protected static AttributeSyntax CreateMethodImplAttribute(GeneratorContext context, MethodImplOptions options) =>
        SyntaxHelpers.CreateMethodImplAttribute(context.RequiredUsings, options);

    [MustUseReturnValue]
    protected static AttributeSyntax CreateMethodImplAttribute(GeneratorContext context, string options) =>
        SyntaxHelpers.CreateMethodImplAttribute(context.RequiredUsings, options);

    [Pure]
    protected static string GetEmulatorClassName(GeneratorContext context) => $"{context.Cpu.Name}Emulator";

    [Pure]
    protected static IdentifierNameSyntax GetEmulatorClassIdentifier(GeneratorContext context) => IdentifierName(GetEmulatorClassName(context));

    [Pure]
    protected static string GetRegistersClassName(GeneratorContext context, string? category = null) => $"{context.Cpu.Name}{category}Registers";

    [Pure]
    protected static string GetFlagsClassName(GeneratorContext context) => $"{context.Cpu.Name}Flags";

    [Pure]
    protected static string GetInterruptsClassName(GeneratorContext context) => $"{context.Cpu.Name}Interrupts";

    [Pure]
    private static UsingDirectiveSyntax CreateUsingStatement(string @namespace) => UsingDirective(IdentifierName(@namespace));
}