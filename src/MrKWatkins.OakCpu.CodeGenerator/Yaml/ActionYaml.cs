using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class ActionYaml
{
    private ActionYaml()
    {
    }

    public string Name { get; private set; } = null!;

    public string? Documentation { get; private set; }

    public override string ToString() => Name;
}