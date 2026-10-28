using VYaml.Annotations;

namespace MrKWatkins.OakCpu.Definitions.Yaml;

[YamlObject]
public sealed partial class ActionYaml
{
    private ActionYaml()
    {
    }

    public string Name { get; private init; } = null!;

    public string? Documentation { get; private init; }

    /// <summary>
    /// The pins asserted during the T-states of the machine cycle the action belongs to, sampled at the rising edge of each
    /// T-state's clock. One entry per T-state, starting at the step that requests the action.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<string>>? Pins { get; private init; }

    public override string ToString() => Name;
}