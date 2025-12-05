namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

public sealed class OpcodeYamlNoPrefixFirstComparer : IComparer<OpcodeYaml>
{
    public static readonly OpcodeYamlNoPrefixFirstComparer Instance = new();

    private OpcodeYamlNoPrefixFirstComparer()
    {
    }

    public int Compare(OpcodeYaml? x, OpcodeYaml? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (x is null)
        {
            return -1;
        }

        if (y is null)
        {
            return 1;
        }

        if (x.PrefixByte == null)
        {
            if (y.PrefixByte == null)
            {
                return x.OpcodeByte.CompareTo(y.OpcodeByte);
            }

            return -1;
        }
        if (y.PrefixByte == null)
        {
            return 1;
        }

        var result = x.PrefixByte.Value.CompareTo(y.PrefixByte.Value);

        return result == 0 ? x.OpcodeByte.CompareTo(y.OpcodeByte) : result;
    }
}