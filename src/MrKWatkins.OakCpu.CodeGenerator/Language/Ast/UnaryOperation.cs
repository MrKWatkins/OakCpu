using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

/// <summary>
/// A unary operation.
/// </summary>
public sealed class UnaryOperation : Expression
{
    internal UnaryOperation(Operator @operator, AstNode expression)
    {
        Operator = @operator;
        Expression = expression as Expression ?? throw new ArgumentException($"Value must be a {nameof(Expression)}, not a {expression.GetType().Name}.", nameof(expression));
    }

    /// <summary>
    /// The operator.
    /// </summary>
    public Operator Operator { get; }

    public Expression Expression { get; private set; }

    public override DataType Type => DataType.I32;

    public override IEnumerable<AstNode> Children
    {
        get
        {
            yield return Expression;
        }
    }

    public override void ReplaceChild(AstNode original, AstNode replacement)
    {
        if (ReferenceEquals(Expression, original))
        {
            Expression = replacement as Expression ?? throw new ArgumentException($"Value must be a {nameof(Expression)}, not a {replacement.GetType().Name}.", nameof(replacement));
        }
        else
        {
            throw new ArgumentException("Value is not a child of this node.", nameof(original));
        }
    }

    public override void WriteStringRepresentation(StringBuilder stringRepresentation)
    {
        stringRepresentation.Append(Operator);
        stringRepresentation.Append('(');
        Expression.WriteStringRepresentation(stringRepresentation);
        stringRepresentation.Append(')');
    }
}