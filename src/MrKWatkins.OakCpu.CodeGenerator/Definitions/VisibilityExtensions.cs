using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public static class VisibilityExtensions
{
    [Pure]
    public static SyntaxToken ToSyntax(this Visibility visibility) => visibility switch
    {
        Visibility.Private => SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
        Visibility.Internal => SyntaxFactory.Token(SyntaxKind.InternalKeyword),
        Visibility.Public => SyntaxFactory.Token(SyntaxKind.PublicKeyword),
        _ => throw new NotSupportedException($"The {nameof(Visibility)} {visibility} is not supported.")
    };
}