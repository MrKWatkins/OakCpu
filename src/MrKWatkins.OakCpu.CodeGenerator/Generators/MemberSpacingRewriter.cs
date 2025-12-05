using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

internal sealed class ExtraSpacingRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitCollectionExpression(CollectionExpressionSyntax node)
    {
        var result = base.VisitCollectionExpression(node)!;

        var elements = result.ChildNodes().OfType<ExpressionElementSyntax>().ToArray();
        if (elements.Length == 0)
        {
            return result;
        }

        // For literal expressions have 10 per line. Everything else is 5 per line.
        var elementsPerLine = elements.Length > 0 && elements[0].ChildNodes().OfType<LiteralExpressionSyntax>().Any() ? 10 : 5;

        // If we have only one line, then there is no need to reformat.
        if (elements.Length <= elementsPerLine)
        {
            return result;
        }

        // Put the '[' on a new line, indented to statement level.
        result = Indent(PrefixWithBlankLine(result), 3);

        // Put n items per line.
        var item = 0;
        result = result.ReplaceNodes(
            result.ChildNodes().OfType<ExpressionElementSyntax>(),
            (original, _) =>
            {
                item++;
                if (item % elementsPerLine == 1)
                {
                    return Indent(PrefixWithBlankLine(original), 4);
                }

                return original;
            });

        // Put the ']' on a new line, indented to statement level.
        var closeBracket = result.ChildTokens().Last();
        var newCloseBracket = closeBracket.WithLeadingTrivia(SyntaxFactory.LineFeed, CreateIndent(3));

        result = result.ReplaceToken(closeBracket, newCloseBracket);

        return result;
    }

    public override SyntaxNode? VisitConstructorDeclaration(ConstructorDeclarationSyntax node) => PrefixWithBlankLine(base.VisitConstructorDeclaration(node));

    public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node) => PrefixWithBlankLine(base.VisitMethodDeclaration(node));

    public override SyntaxNode? VisitFieldDeclaration(FieldDeclarationSyntax node) => PrefixWithBlankLine(base.VisitFieldDeclaration(node));

    public override SyntaxNode? VisitPropertyDeclaration(PropertyDeclarationSyntax node) => PrefixWithBlankLine(base.VisitPropertyDeclaration(node));

    public override SyntaxNode VisitAccessorDeclaration(AccessorDeclarationSyntax node)
    {
        var accessor = (AccessorDeclarationSyntax)base.VisitAccessorDeclaration(node)!;

        // If we have a setter with attributes, then we need a blank line or the attribute will be on the same line as the getter. Assumes no set-only properties!
        return accessor.IsKind(SyntaxKind.SetAccessorDeclaration) && accessor.AttributeLists.Any() ? Indent(PrefixWithBlankLine(accessor), 3) : accessor;
    }

    public override SyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node)
    {
        // Prefix variable declarations in methods if they have a comment.
        var result = base.VisitVariableDeclaration(node)!;
        return result.GetLeadingTrivia().Any(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia)) ? PrefixWithBlankLine(result) : result;
    }

    [Pure]
    [return: NotNullIfNotNull("node")]
    private static T? PrefixWithBlankLine<T>(T? node)
        where T : SyntaxNode
    {
        if (node is null)
        {
            return null;
        }

        var trivia = node.GetLeadingTrivia();
        var newTrivia = trivia.Insert(0, SyntaxFactory.LineFeed);
        return node.WithLeadingTrivia(newTrivia);
    }

    [Pure]
    [return: NotNullIfNotNull("node")]
    private static T? Indent<T>(T? node, int depth)
        where T : SyntaxNode
    {
        if (node is null)
        {
            return null;
        }

        var trivia = node.GetLeadingTrivia();
        var newTrivia = trivia.Append(CreateIndent(depth));
        return node.WithLeadingTrivia(newTrivia);
    }

    [Pure]
    private static SyntaxTrivia CreateIndent(int depth) => SyntaxFactory.Whitespace(string.Concat(Enumerable.Repeat("    ", depth)));
}