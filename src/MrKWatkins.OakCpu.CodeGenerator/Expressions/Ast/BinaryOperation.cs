using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

/// <summary>
/// A binary operation. The expression has a left and right side and an operator.
/// </summary>
public sealed class BinaryOperation : Expression
{
    internal BinaryOperation(char @operator, Expression left, Expression right)
    {
        Operator = @operator;
        Left = left;
        Right = right;
    }

    /// <summary>
    /// The operator.
    /// </summary>
    public char Operator { get; }

    /// <summary>
    /// The <see cref="SyntaxKind" /> for an expression using the operator.
    /// </summary>
    public SyntaxKind ExpressionSyntaxKind => Operator switch
    {
        '+' => SyntaxKind.AddExpression,
        '-' => SyntaxKind.SubtractExpression,
        '&' => SyntaxKind.BitwiseAndExpression,
        '|' => SyntaxKind.BitwiseOrExpression,
        '^' => SyntaxKind.ExclusiveOrExpression,
        _ => throw new NotSupportedException($"The operator {Operator} is not supported.")
    };

    /// <summary>
    /// The relative precedence of the operator.
    /// </summary>
    public int OperatorPrecedence => Operator switch
    {
        '+' => 0,
        '-' => 0,
        '|' => 1,
        '^' => 2,
        '&' => 3,
        _ => throw new NotSupportedException($"The operator {Operator} is not supported.")
    };

    /// <summary>
    /// The left side of the operation.
    /// </summary>
    public Expression Left { get; }

    /// <summary>
    /// The right side of the operation.
    /// </summary>
    public Expression Right { get; }

    public override Type Type => typeof(int);

    public override TypeSyntax TypeSyntax => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));

    public override IEnumerable<Expression> Children
    {
        get
        {
            yield return Left;
            yield return Right;
        }
    }

    public override void WriteExpression(StringBuilder expression)
    {
        expression.Append('(');
        Left.WriteExpression(expression);
        expression.Append(' ');
        expression.Append(Operator);
        expression.Append(' ');
        Right.WriteExpression(expression);
        expression.Append(')');
    }
}