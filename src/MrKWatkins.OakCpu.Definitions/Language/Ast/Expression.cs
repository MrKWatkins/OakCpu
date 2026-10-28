
namespace MrKWatkins.OakCpu.Definitions.Language.Ast;

public abstract class Expression : AstNode, IEquatable<Expression>
{
    public abstract DataType Type { get; }

    public bool Equals(Expression? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return ToString() == other.ToString();
    }

    public override bool Equals(object? obj) => Equals(obj as Expression);

    public override int GetHashCode() => ToString().GetHashCode(StringComparison.Ordinal);

    public static bool operator ==(Expression? left, Expression? right) => Equals(left, right);

    public static bool operator !=(Expression? left, Expression? right) => !Equals(left, right);
}