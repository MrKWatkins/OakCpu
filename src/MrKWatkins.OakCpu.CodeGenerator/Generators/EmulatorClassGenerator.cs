using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class EmulatorClassGenerator
{
    private protected EmulatorClassGenerator()
    {
    }

    [Pure]
    public CompilationUnitSyntax Generate(GeneratorInput input)
    {
        var usings = new HashSet<string>();

        var classDeclaration = PopulateClass(usings, input, SyntaxFactory.ClassDeclaration("Z80Emulator").AddModifiers(Public, Sealed, Partial));

        return SyntaxFactory
            .CompilationUnit()
            .AddUsings(usings.OrderBy(n => n).Select(CreateUsingStatement).ToArray())
            .AddMembers(
                input
                    .CreateRootNamespaceDeclaration()
                    .AddMembers(classDeclaration))
            .NormalizeWhitespace();
    }

    [Pure]
    protected abstract ClassDeclarationSyntax PopulateClass(HashSet<string> requiredUsings, GeneratorInput input, ClassDeclarationSyntax classDeclaration);

    [Pure]
    protected static SyntaxToken Private => SyntaxFactory.Token(SyntaxKind.PrivateKeyword);

    [Pure]
    protected static SyntaxToken Internal => SyntaxFactory.Token(SyntaxKind.InternalKeyword);

    [Pure]
    protected static SyntaxToken Public => SyntaxFactory.Token(SyntaxKind.PublicKeyword);

    [Pure]
    private static SyntaxToken Sealed => SyntaxFactory.Token(SyntaxKind.SealedKeyword);

    [Pure]
    private static SyntaxToken Partial => SyntaxFactory.Token(SyntaxKind.PartialKeyword);

    [Pure]
    private static UsingDirectiveSyntax CreateUsingStatement(string ns) => SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(ns));
}