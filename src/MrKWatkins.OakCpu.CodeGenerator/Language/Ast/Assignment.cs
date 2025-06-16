using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class Assignment : Statement
{
    internal Assignment(AstNode target, AstNode value)
    {
        if (target is not Access)
        {
            throw new ArgumentException($"Target must be an {nameof(Access)}, not a {target.GetType().Name}.", nameof(target));
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

    public override IEnumerable<AstNode> Children
    {
        get
        {
            yield return Target;
            yield return Value;
        }
    }

    public override void WriteStringRepresentation(StringBuilder stringRepresentation)
    {
        Target.WriteStringRepresentation(stringRepresentation);
        stringRepresentation.Append(" = ");
        Value.WriteStringRepresentation(stringRepresentation);
    }
}