using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class Assignment : Expression
{
    internal Assignment(Expression target, Expression value)
    {
        if (target is not MemberAccess && target is not RegisterAccess)
        {
            throw new ArgumentException($"Target must be a {nameof(MemberAccess)} or {nameof(RegisterAccess)}, not a {target.GetType().Name}.", nameof(target));
        }
        Target = target;
        Value = value;
    }

    /// <summary>
    /// The target side of the assignment.
    /// </summary>
    public Expression Target { get; }

    /// <summary>
    /// The value to assign.
    /// </summary>
    public Expression Value { get; }

    public override IEnumerable<Expression> Children
    {
        get
        {
            yield return Target;
            yield return Value;
        }
    }

    public override void WriteExpression(StringBuilder expression)
    {
        Target.WriteExpression(expression);
        expression.Append(" = ");
        Value.WriteExpression(expression);
    }
}