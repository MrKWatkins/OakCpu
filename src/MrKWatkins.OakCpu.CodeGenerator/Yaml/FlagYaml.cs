using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class FlagYaml
{
    private FlagYaml()
    {
    }

    public string Name { get; private init; } = null!;

    public int Index { get; private init; }

    public string? Documentation { get; private init; }

    public string? Condition { get; private init; }

    public string? NotCondition { get; private init; }

    public override string ToString() => Name;
}