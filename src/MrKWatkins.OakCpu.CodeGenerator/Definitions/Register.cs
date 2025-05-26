using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Register
{
    internal Register(string name, DataType type, bool flags, bool programCounter, string? category, int fieldOffset)
    {
        Name = name;
        Type = type;
        Flags = flags;
        ProgramCounter = programCounter;
        Category = category;
        FieldOffset = fieldOffset;
    }

    public string Name { get; }

    public string FieldName => Category != null ? $"{Category}_{PropertyName}" : PropertyName;

    public string PropertyName => Name.Replace("'", "");

    public DataType Type { get; }

    public bool Flags { get; }

    public bool ProgramCounter { get; }

    public string? Category { get; }

    public int FieldOffset { get; }

    public override string ToString() => $"{Name}: {Type}";

    [Pure]
    public static IReadOnlyList<Register> Create(IReadOnlyList<RegisterYaml> yamls)
    {
        var registers = new List<Register>();

        var fieldOffset = 0;
        foreach (var yaml in Order(yamls.Where(y => y.High != null)))
        {
            registers.Add(new Register(yaml.Name, yaml.Type, yaml.Flags, yaml.ProgramCounter, yaml.Category, fieldOffset));

            // Little endian; low byte is at the lowest address.
            registers.Add(new Register(yaml.Low!.Name, yaml.Low.Type, yaml.Low.Flags, yaml.Low.ProgramCounter, yaml.Low.Category, fieldOffset));
            fieldOffset += yaml.Low.Type.Size();

            registers.Add(new Register(yaml.High!.Name, yaml.High.Type, yaml.High.Flags, yaml.High.ProgramCounter, yaml.High.Category, fieldOffset));
            fieldOffset += yaml.High.Type.Size();
        }

        foreach (var yaml in Order(yamls.Where(y => y.High == null)))
        {
            registers.Add(new Register(yaml.Name, yaml.Type, yaml.Flags, yaml.ProgramCounter, yaml.Category, fieldOffset));
            fieldOffset += yaml.Type.Size();
        }

        return registers.OrderBy(r => r.FieldOffset).ToList();
    }

    [Pure]
    private static IOrderedEnumerable<RegisterYaml> Order(IEnumerable<RegisterYaml> yamls) =>
        // Put U16s first, so they are all on a two-byte boundary.
        yamls.OrderByDescending(y => y.Type).ThenBy(y => y.Category).ThenBy(y => y.Name);
}