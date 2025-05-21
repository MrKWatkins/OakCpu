using VYaml.Annotations;

namespace MrKWatkins.OakCpu.SourceGenerator.Yaml;

[YamlObject]
public sealed partial class RegisterYaml
{
    private RegisterYaml()
    {
    }

    public string Name { get; set; } = null!;

    public DataType Type { get; set; }

    public bool Flags { get; set; }

    public string? Category { get; set; }

    public IReadOnlyList<string> Combines { get; set; } = [];

    public override string ToString() => $"{Name}: {Type}";
}