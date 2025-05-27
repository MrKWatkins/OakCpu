using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class Generator
{
    protected const string ActionRequiredEnumName = "ActionRequired";
    protected const string ActionRequiredNone = "None";
    protected const string ActionVariableName = "action";
    protected const string StepVariableName = "step";

    private protected Generator()
    {
    }

    [Pure]
    protected static PredefinedTypeSyntax Bool => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword));

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
    protected static SyntaxToken Static => SyntaxFactory.Token(SyntaxKind.StaticKeyword);

    [Pure]
    protected static SyntaxToken GetBinaryLiteral(byte value) => SyntaxFactory.Literal($"0b{Convert.ToString(value, 2).PadLeft(8, '0')}", value);

    [Pure]
    protected static LiteralExpressionSyntax GetBinaryLiteralExpression(byte value) => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, GetBinaryLiteral(value));

    [Pure]
    protected static LiteralExpressionSyntax GetNumericLiteralExpression(int value) => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
}