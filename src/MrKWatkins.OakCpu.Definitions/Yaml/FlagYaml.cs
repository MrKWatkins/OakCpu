using VYaml.Annotations;

namespace MrKWatkins.OakCpu.Definitions.Yaml;

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

    /// <summary>
    /// The modes the flag selects, if it selects a mode of operation rather than recording a result.
    /// </summary>
    public FlagModeYaml? Mode { get; private init; }

    public override string ToString() => Name;
}