using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class FlagYaml
{
    private FlagYaml()
    {
    }

    public string Name { get; private set; } = null!;

    public int Index { get; private set; }

    public string? Condition { get; private set; }

    public string? NotCondition { get; private set; }

    public override string ToString() => Name;
}