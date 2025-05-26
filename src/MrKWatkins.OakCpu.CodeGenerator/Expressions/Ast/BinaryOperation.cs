using System.Text;

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
    /// The left side of the operation.
    /// </summary>
    public Expression Left { get; }

    /// <summary>
    /// The right side of the operation.
    /// </summary>
    public Expression Right { get; }

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