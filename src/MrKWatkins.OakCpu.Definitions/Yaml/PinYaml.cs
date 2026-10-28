using VYaml.Annotations;

namespace MrKWatkins.OakCpu.Definitions.Yaml;

[YamlObject]
public sealed partial class PinYaml
{
    private PinYaml()
    {
    }

    public string Name { get; private init; } = null!;

    public string? Documentation { get; private init; }

    public override string ToString() => Name;
}