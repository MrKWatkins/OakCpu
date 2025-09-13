using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator;

/// <summary>
/// Provides static properties for commonly used types and tokens in code generation.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class CommonSyntax
{
    /// <summary>
    /// Gets the bool predefined type syntax.
    /// </summary>
    [Pure]
    public static PredefinedTypeSyntax Bool => PredefinedType(Token(SyntaxKind.BoolKeyword));

    /// <summary>
    /// Gets the byte predefined type syntax.
    /// </summary>
    [Pure]
    public static PredefinedTypeSyntax Byte => PredefinedType(Token(SyntaxKind.ByteKeyword));

    /// <summary>
    /// Gets the int predefined type syntax.
    /// </summary>
    [Pure]
    public static PredefinedTypeSyntax Int => PredefinedType(Token(SyntaxKind.IntKeyword));

    /// <summary>
    /// Gets the ushort predefined type syntax.
    /// </summary>
    [Pure]
    public static PredefinedTypeSyntax UShort => PredefinedType(Token(SyntaxKind.UShortKeyword));

    /// <summary>
    /// Gets the void type syntax.
    /// </summary>
    [Pure]
    public static TypeSyntax Void => PredefinedType(Token(SyntaxKind.VoidKeyword));

    /// <summary>
    /// Gets the field keyword token.
    /// </summary>
    [Pure]
    public static SyntaxToken Field => Token(SyntaxKind.FieldKeyword);

    /// <summary>
    /// Gets the internal keyword token.
    /// </summary>
    [Pure]
    public static SyntaxToken Internal => Token(SyntaxKind.InternalKeyword);

    /// <summary>
    /// Gets the partial keyword token.
    /// </summary>
    [Pure]
    public static SyntaxToken Partial => Token(SyntaxKind.PartialKeyword);

    /// <summary>
    /// Gets the private keyword token.
    /// </summary>
    [Pure]
    public static SyntaxToken Private => Token(SyntaxKind.PrivateKeyword);

    /// <summary>
    /// Gets the public keyword token.
    /// </summary>
    [Pure]
    public static SyntaxToken Public => Token(SyntaxKind.PublicKeyword);

    /// <summary>
    /// Gets the readonly keyword token.
    /// </summary>
    [Pure]
    public static SyntaxToken ReadOnly => Token(SyntaxKind.ReadOnlyKeyword);

    /// <summary>
    /// Gets the ref keyword token.
    /// </summary>
    [Pure]
    public static SyntaxToken Ref => Token(SyntaxKind.RefKeyword);

    /// <summary>
    /// Gets the sealed keyword token.
    /// </summary>
    [Pure]
    public static SyntaxToken Sealed => Token(SyntaxKind.SealedKeyword);

    /// <summary>
    /// Gets the semicolon token.
    /// </summary>
    [Pure]
    public static SyntaxToken Semicolon => Token(SyntaxKind.SemicolonToken);

    /// <summary>
    /// Gets the static keyword token.
    /// </summary>
    [Pure]
    public static SyntaxToken Static => Token(SyntaxKind.StaticKeyword);

    /// <summary>
    /// Gets the unsafe keyword token.
    /// </summary>
    [Pure]
    public static SyntaxToken Unsafe => Token(SyntaxKind.UnsafeKeyword);
}