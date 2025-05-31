using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class Assignment : Statement
{
    internal Assignment(AstNode target, AstNode value)
    {
        if (target is not DataMemberAccess && target is not RegisterAccess)
        {
            throw new ArgumentException($"Target must be a {nameof(DataMemberAccess)} or {nameof(RegisterAccess)}, not a {target.GetType().Name}.", nameof(target));
        }
        Target = target as Expression ?? throw new ArgumentException($"Value must be a {nameof(Expression)}, not a {target.GetType().Name}.", nameof(target));
        Value = value as Expression ?? throw new ArgumentException($"Value must be a {nameof(Expression)}, not a {value.GetType().Name}.", nameof(value));
    }

    /// <summary>
    /// The target side of the assignment.
    /// </summary>
    public Expression Target { get; }

    /// <summary>
    /// The value to assign.
    /// </summary>
    public Expression Value { get; }

    public override void WriteExpression(StringBuilder expression)
    {
        Target.WriteExpression(expression);
        expression.Append(" = ");
        Value.WriteExpression(expression);
    }
}