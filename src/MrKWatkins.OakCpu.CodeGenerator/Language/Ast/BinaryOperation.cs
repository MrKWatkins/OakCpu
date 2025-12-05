using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

/// <summary>
/// A binary operation. The expression has a left and right side and an operator.
/// </summary>
public sealed class BinaryOperation : Expression
{
    internal BinaryOperation(Operator @operator, AstNode left, AstNode right)
    {
        Operator = @operator;
        Left = left as Expression ?? throw new ArgumentException($"Value must be a {nameof(Expression)}, not a {left.GetType().Name}.", nameof(left));
        Right = right as Expression ?? throw new ArgumentException($"Value must be a {nameof(Expression)}, not a {right.GetType().Name}.", nameof(right));
    }

    /// <summary>
    /// The operator.
    /// </summary>
    public Operator Operator { get; }

    /// <summary>
    /// The left side of the operation.
    /// </summary>
    public Expression Left { get; }

    /// <summary>
    /// The right side of the operation.
    /// </summary>
    public Expression Right { get; }

    public override DataType Type => Operator.Type;

    public override IEnumerable<AstNode> Children
    {
        get
        {
            yield return Left;
            yield return Right;
        }
    }

    public override void WriteStringRepresentation(StringBuilder stringRepresentation)
    {
        if (Left is BinaryOperation leftBinary && leftBinary.Operator.Precedence < Operator.Precedence)
        {
            stringRepresentation.Append('(');
            Left.WriteStringRepresentation(stringRepresentation);
            stringRepresentation.Append(')');
        }
        else
        {
            Left.WriteStringRepresentation(stringRepresentation);
        }
        stringRepresentation.Append(' ');
        stringRepresentation.Append(Operator);
        stringRepresentation.Append(' ');
        if (Right is BinaryOperation rightBinary && rightBinary.Operator.Precedence < Operator.Precedence)
        {
            stringRepresentation.Append('(');
            Right.WriteStringRepresentation(stringRepresentation);
            stringRepresentation.Append(')');
        }
        else
        {
            Right.WriteStringRepresentation(stringRepresentation);
        }
    }
}