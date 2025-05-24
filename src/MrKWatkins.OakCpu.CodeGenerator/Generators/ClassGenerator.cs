using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class ClassGenerator
{
    protected const string EmulatorClassName = "Z80Emulator";

    private protected ClassGenerator()
    {
    }

    public static readonly IReadOnlyList<ClassGenerator> AllGenerators = [EmulatorFieldsPropertiesAndConstructorGenerator.Instance, RegistersClassesGenerator.Instance];

    public string FileName => GetType().Name.Substring(0, GetType().Name.Length - "Generator".Length);

    [Pure]
    public CompilationUnitSyntax Generate(GeneratorInput input)
    {
        var requiredUsings = new HashSet<string>();

        var classDeclarations = CreateClasses(requiredUsings, input).ToArray<MemberDeclarationSyntax>();

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
    protected virtual IEnumerable<ClassDeclarationSyntax> CreateClasses(HashSet<string> requiredUsings, GeneratorInput input)
    {
        yield return CreateClass(requiredUsings, input);
    }

    [MustUseReturnValue]
    protected virtual ClassDeclarationSyntax CreateClass(HashSet<string> requiredUsings, GeneratorInput input)
    {
        throw new NotImplementedException($"{nameof(CreateClass)} is not implemented and {nameof(CreateClasses)} has not been overridden.");
    }

    [Pure]
    protected static PropertyDeclarationSyntax CreateGetOnlyProperty(string typeName, string propertyName) => CreateGetOnlyProperty(SyntaxFactory.IdentifierName(typeName), propertyName);

    [Pure]
    protected static PropertyDeclarationSyntax CreateGetOnlyProperty(TypeSyntax type, string propertyName) =>
        SyntaxFactory
            .PropertyDeclaration(type, SyntaxFactory.Identifier(propertyName))
            .WithModifiers(SyntaxFactory.TokenList(Public))
            .WithAccessorList(
                SyntaxFactory.AccessorList(SyntaxFactory.SingletonList(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Semicolon))));


    [Pure]
    protected static SyntaxToken Field => SyntaxFactory.Token(SyntaxKind.FieldKeyword);

    [Pure]
    protected static SyntaxToken Internal => SyntaxFactory.Token(SyntaxKind.InternalKeyword);

    [Pure]
    protected static SyntaxToken Partial => SyntaxFactory.Token(SyntaxKind.PartialKeyword);

    [Pure]
    protected static SyntaxToken Private => SyntaxFactory.Token(SyntaxKind.PrivateKeyword);

    [Pure]
    protected static SyntaxToken Public => SyntaxFactory.Token(SyntaxKind.PublicKeyword);

    [Pure]
    protected static SyntaxToken ReadOnly => SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword);

    [Pure]
    protected static SyntaxToken Sealed => SyntaxFactory.Token(SyntaxKind.SealedKeyword);

    [Pure]
    protected static SyntaxToken Semicolon => SyntaxFactory.Token(SyntaxKind.SemicolonToken);

    [Pure]
    protected static string GetRegistersClassName(string? category = null) => $"Z80{category}Registers";

    [Pure]
    private static UsingDirectiveSyntax CreateUsingStatement(string ns) => SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(ns));
}