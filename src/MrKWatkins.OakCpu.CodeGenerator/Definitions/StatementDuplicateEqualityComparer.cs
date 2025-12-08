using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

/// <summary>
/// Two statements are considered duplicates if they have the same string representations.
/// </summary>
internal sealed class StatementDuplicateEqualityComparer : IEqualityComparer<Statement>
{
    public static readonly StatementDuplicateEqualityComparer Instance = new();

    private StatementDuplicateEqualityComparer()
    {
    }

    public bool Equals(Statement? x, Statement? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return false;
        }

        if (y is null)
        {
            return false;
        }

        return x.ToString().Equals(y.ToString(), StringComparison.Ordinal);
    }

    public int GetHashCode(Statement obj) => obj.ToString().GetHashCode(StringComparison.Ordinal);
}