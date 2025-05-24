using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Register
{
    private Register(string name, DataType type, bool flags, string? category, int fieldOffset)
    {
        Name = name;
        Type = type;
        Flags = flags;
        Category = category;
        FieldOffset = fieldOffset;
    }

    public string Name { get; }

    public string FieldName => (Category != null ? $"{Category}_{Name}" : Name).Replace("'", "");

    public DataType Type { get; }

    public bool Flags { get; }

    public string? Category { get; }

    public int FieldOffset { get; }

    public override string ToString() => $"{Name}: {Type}";

    [Pure]
    public static IReadOnlyList<Register> Create(IReadOnlyList<RegisterYaml> yamls)
    {
        var yamlsByName = yamls.ToDictionary(y => y.Name);

        var registers = new Dictionary<string, Register>();

        var fieldOffset = 0;
        foreach (var yaml in Order(yamls.Where(y => y.Combines.Count > 0)))
        {
            registers.Add(yaml.Name, new Register(yaml.Name, yaml.Type, yaml.Flags, yaml.Category, fieldOffset));
            foreach (var componentYaml in yaml.Combines.Reverse().Select(c => yamlsByName[c]))
            {
                registers.Add(componentYaml.Name, new Register(componentYaml.Name, componentYaml.Type, componentYaml.Flags, componentYaml.Category, fieldOffset));
                fieldOffset += componentYaml.Type.Size();
            }
        }

        foreach (var yaml in Order(yamls.Where(y => y.Combines.Count == 0 && !registers.ContainsKey(y.Name))))
        {
            registers.Add(yaml.Name, new Register(yaml.Name, yaml.Type, yaml.Flags, yaml.Category, fieldOffset));
            fieldOffset += yaml.Type.Size();
        }

        return registers.Values.OrderBy(r => r.FieldOffset).ToList();
    }

    [Pure]
    private static IOrderedEnumerable<RegisterYaml> Order(IEnumerable<RegisterYaml> yamls) => yamls.OrderBy(y => y.Type).ThenBy(y => y.Category).ThenBy(y => y.Name);
}