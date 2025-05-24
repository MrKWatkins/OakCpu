using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class ClassGenerator
{
    private protected ClassGenerator()
    {
    }

    [Pure]
    public CompilationUnitSyntax Generate(GeneratorInput input)
    {
        var requiredUsings = new HashSet<string>();

        var classDeclaration = CreateClass(requiredUsings, input);

        return SyntaxFactory
            .CompilationUnit()
            .AddUsings(requiredUsings.OrderBy(n => n).Select(CreateUsingStatement).ToArray())
            .AddMembers(
                input
                    .CreateRootNamespaceDeclaration()
                    .AddMembers(classDeclaration))
            .NormalizeWhitespace();
    }

    protected abstract ClassDeclarationSyntax CreateClass(HashSet<string> requiredUsings, GeneratorInput input);

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
    private static UsingDirectiveSyntax CreateUsingStatement(string ns) => SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(ns));
}