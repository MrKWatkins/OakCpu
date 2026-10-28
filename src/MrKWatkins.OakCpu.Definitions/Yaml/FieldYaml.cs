using VYaml.Annotations;

namespace MrKWatkins.OakCpu.Definitions.Yaml;

[YamlObject]
public sealed partial class FieldYaml
{
    private FieldYaml()
    {
    }

    public string Name { get; private init; } = null!;

    public DataType Type { get; private init; }

    public string? Documentation { get; private init; }

    public bool Getter { get; private init; }

    public bool Setter { get; private init; }

    public override string ToString() => $"{Name}: {Type}";
}