namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class InstructionFlagsDuplicateEqualityComparer : IEqualityComparer<Instruction>
{
    public static readonly InstructionFlagsDuplicateEqualityComparer Instance = new();

    private InstructionFlagsDuplicateEqualityComparer()
    {
    }

    public bool Equals(Instruction? x, Instruction? y)
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

        if (x.Flags.Count != y.Flags.Count)
        {
            return false;
        }

        foreach (var (flag, expression) in x.Flags)
        {
            if (!y.Flags.TryGetValue(flag, out var yExpression) || !expression.ToString().Equals(yExpression.ToString(), StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    public int GetHashCode(Instruction obj)
    {
        var hashCode = new HashCode();
        foreach (var (flag, expression) in obj.Flags)
        {
            hashCode.Add(flag);
            hashCode.Add(expression.ToString());
        }

        return hashCode.ToHashCode();
    }
}