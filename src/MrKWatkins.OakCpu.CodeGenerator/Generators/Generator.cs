using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class Generator
{
    protected const string ActionRequiredEnumName = "ActionRequired";
    protected const string StepVariableName = "step";

    private protected Generator()
    {
    }

    [Pure]
    protected static StatementSyntax InitializeVariableStatement(string variable, ExpressionSyntax value) =>
        LocalDeclarationStatement(VariableDeclaration(IdentifierName("var"))
            .WithVariables(SingletonSeparatedList(
                VariableDeclarator(Identifier(variable))
                    .WithInitializer(EqualsValueClause(value)))));

    [Pure]
    protected static PredefinedTypeSyntax Bool => PredefinedType(Token(SyntaxKind.BoolKeyword));

    [Pure]
    protected static PredefinedTypeSyntax Byte => PredefinedType(Token(SyntaxKind.ByteKeyword));

    [Pure]
    protected static SyntaxToken Field => Token(SyntaxKind.FieldKeyword);

    [Pure]
    protected static SyntaxToken Internal => Token(SyntaxKind.InternalKeyword);

    [Pure]
    protected static SyntaxToken Partial => Token(SyntaxKind.PartialKeyword);

    [Pure]
    protected static SyntaxToken Private => Token(SyntaxKind.PrivateKeyword);

    [Pure]
    protected static SyntaxToken Public => Token(SyntaxKind.PublicKeyword);

    [Pure]
    protected static SyntaxToken ReadOnly => Token(SyntaxKind.ReadOnlyKeyword);

    [Pure]
    protected static SyntaxToken Sealed => Token(SyntaxKind.SealedKeyword);

    [Pure]
    protected static SyntaxToken Semicolon => Token(SyntaxKind.SemicolonToken);

    [Pure]
    protected static SyntaxToken Static => Token(SyntaxKind.StaticKeyword);

    [Pure]
    protected static SyntaxToken GenerateBinaryLiteral(byte value) => Literal($"0b{Convert.ToString(value, 2).PadLeft(8, '0')}", value);

    [Pure]
    protected static LiteralExpressionSyntax GenerateBinaryLiteralExpression(byte value) => LiteralExpression(SyntaxKind.NumericLiteralExpression, GenerateBinaryLiteral(value));

    [Pure]
    protected static LiteralExpressionSyntax GenerateNumericLiteralExpression(int value) => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value));
}