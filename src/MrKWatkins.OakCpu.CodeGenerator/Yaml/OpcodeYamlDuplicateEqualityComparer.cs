namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

/// <summary>
/// Checks if two OpcodeYamls, *for the same instruction*, are duplicates, i.e. they have all the same substitution parameters.
/// We *do not* consider prefixed opcodes to be duplicates of non-prefixed, because the prefixed versions need to reset the
/// opcode table.
/// </summary>
public sealed class OpcodeYamlDuplicateEqualityComparer : IEqualityComparer<OpcodeYaml>
{
    public static readonly OpcodeYamlDuplicateEqualityComparer Instance = new();

    private OpcodeYamlDuplicateEqualityComparer()
    {
    }

    public bool Equals(OpcodeYaml? x, OpcodeYaml? y)
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

        var xHasPrefix = x.PrefixByte != null;
        var yHasPrefix = y.PrefixByte != null;

        return xHasPrefix == yHasPrefix &&
               x.R0 == y.R0 &&
               x.R1 == y.R1 &&
               x.RP0 == y.RP0 &&
               x.RP1 == y.RP1 &&
               x.C0 == y.C0 &&
               x.N0 == y.N0;
    }

    public int GetHashCode(OpcodeYaml obj)
    {
        // HashCode.Combine is not available in .NET Standard 2.0.
        unchecked
        {
            var hasPrefix = obj.PrefixByte != null;
            var hashCode = hasPrefix.GetHashCode();
            hashCode = (hashCode * 397) ^ (obj.R0 != null ? obj.R0.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (obj.R1 != null ? obj.R1.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (obj.RP0 != null ? obj.RP0.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (obj.RP1 != null ? obj.RP1.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (obj.C0 != null ? obj.C0.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ obj.N0.GetHashCode();
            return hashCode;
        }
    }
}