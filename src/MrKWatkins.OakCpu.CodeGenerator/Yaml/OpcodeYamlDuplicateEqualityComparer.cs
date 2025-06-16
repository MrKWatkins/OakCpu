namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

/// <summary>
/// Checks if two OpcodeYamls, *for the same instruction*, are duplicates, i.e. they have all the same substitution parameters.
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
        return x.R0 == y.R0 &&
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
            var hashCode = obj.R0 != null ? obj.R0.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (obj.R1 != null ? obj.R1.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (obj.RP0 != null ? obj.RP0.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (obj.RP1 != null ? obj.RP1.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (obj.C0 != null ? obj.C0.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ obj.N0.GetHashCode();
            return hashCode;
        }
    }
}