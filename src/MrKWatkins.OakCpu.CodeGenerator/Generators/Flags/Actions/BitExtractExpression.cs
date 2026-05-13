using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;

internal sealed class BitExtractExpression : FlagAction
{
    private readonly Expression expression;
    private readonly int extractedBitIndex;
    private readonly Expression originalExpression;

    internal BitExtractExpression(Flag flag, Expression originalExpression, Expression expression, int extractedBitIndex)
        : base(flag)
    {
        this.originalExpression = originalExpression;
        this.expression = expression;
        this.extractedBitIndex = extractedBitIndex;
    }

    internal override int Order => 2;

    internal override ExpressionSyntax GenerateExpression(StatementGeneratorContext context)
    {
        var generatedExpression = ExpressionGenerator.GenerateExpressionSyntax(context, expression);
        if (extractedBitIndex != 0)
        {
            generatedExpression = BinaryExpression(SyntaxKind.RightShiftExpression, ParenthesizedExpression(generatedExpression), GenerateNumericLiteralExpression(extractedBitIndex));
        }

        generatedExpression = BinaryExpression(SyntaxKind.BitwiseAndExpression, ParenthesizedExpression(generatedExpression), GenerateNumericLiteralExpression(0x01));

        var shift = Flags[0].Index;
        return shift != 0
            ? BinaryExpression(SyntaxKind.LeftShiftExpression, ParenthesizedExpression(generatedExpression), GenerateNumericLiteralExpression(shift))
            : generatedExpression;
    }

    internal override string GenerateComment() => $"// Set {FlagsNames(Flags)} if {originalExpression} is true.";
}