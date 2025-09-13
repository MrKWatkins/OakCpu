using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;

// TODO: Could potentially combine these?
internal sealed class I32BoolExpression : FlagAction
{
    private I32BoolExpression(Flag flag, Expression originalExpression)
        : base(flag)
    {
        OriginalExpression = originalExpression;
        Expression = originalExpression;
        Shift = flag.Index;
    }

    internal I32BoolExpression(I32BoolExpression original, Expression expression, int shift)
        : base(original.Flags)
    {
        OriginalExpression = original.OriginalExpression;
        Expression = expression;
        Shift = shift;
    }

    public Expression OriginalExpression { get; }

    public Expression Expression { get; }

    public int Shift { get; }

    internal override int Order => 2;

    internal override ExpressionSyntax GenerateExpression(StatementGeneratorContext context)
    {
        var expressionSyntax = ExpressionGenerator.GenerateExpressionSyntax(context, Expression);

        return Shift switch
        {
            > 0 => BinaryExpression(SyntaxKind.LeftShiftExpression, ParenthesizedExpression(expressionSyntax), SyntaxHelpers.GenerateNumericLiteralExpression(Shift)),
            < 0 => BinaryExpression(SyntaxKind.RightShiftExpression, ParenthesizedExpression(expressionSyntax), SyntaxHelpers.GenerateNumericLiteralExpression(-Shift)),
            _ => expressionSyntax
        };
    }

    internal override string GenerateComment() => $"// Set {FlagsNames(Flags)} if {OriginalExpression} is true.";

    [Pure]
    internal static FlagAction? CreateOrNull(Flag flag, Expression expression) =>
        expression.Type == DataType.I32Bool
            ? new I32BoolExpression(flag, expression)
            : null;
}