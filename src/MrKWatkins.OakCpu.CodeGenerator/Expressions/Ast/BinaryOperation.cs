using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

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

    public override Type Type => typeof(int);

    public override TypeSyntax TypeSyntax => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));

    public override void WriteExpression(StringBuilder expression)
    {
        if (Left is BinaryOperation leftBinary && leftBinary.Operator.Precedence < Operator.Precedence)
        {
            expression.Append('(');
            Left.WriteExpression(expression);
            expression.Append(')');
        }
        else
        {
            Left.WriteExpression(expression);
        }
        expression.Append(' ');
        expression.Append(Operator);
        expression.Append(' ');
        if (Right is BinaryOperation rightBinary && rightBinary.Operator.Precedence < Operator.Precedence)
        {
            expression.Append('(');
            Right.WriteExpression(expression);
            expression.Append(')');
        }
        else
        {
            Right.WriteExpression(expression);
        }
    }
}