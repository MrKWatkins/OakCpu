using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;

internal sealed class BitExtractExpression : FlagAction
{
    internal BitExtractExpression(Flag flag, Expression originalExpression, Expression expression, int extractedBitIndex)
        : base(flag)
    {
        OriginalExpression = originalExpression;
        Expression = expression;
        ExtractedBitIndex = extractedBitIndex;
    }

    public Expression OriginalExpression { get; }

    public Expression Expression { get; }

    public int ExtractedBitIndex { get; }

    internal override int Order => 2;

    internal override ExpressionSyntax GenerateExpression(StatementGeneratorContext context)
    {
        var expression = ExpressionGenerator.GenerateExpressionSyntax(context, Expression);
        if (ExtractedBitIndex != 0)
        {
            expression = BinaryExpression(SyntaxKind.RightShiftExpression, ParenthesizedExpression(expression), GenerateNumericLiteralExpression(ExtractedBitIndex));
        }

        expression = BinaryExpression(SyntaxKind.BitwiseAndExpression, ParenthesizedExpression(expression), GenerateNumericLiteralExpression(0x01));

        var shift = Flags[0].Index;
        return shift != 0
            ? BinaryExpression(SyntaxKind.LeftShiftExpression, ParenthesizedExpression(expression), GenerateNumericLiteralExpression(shift))
            : expression;
    }

    internal override string GenerateComment() => $"// Set {FlagsNames(Flags)} if {OriginalExpression} is true.";
}