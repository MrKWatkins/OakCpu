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

    /// <summary>
    /// The pins asserted during the T-states of the machine cycle the action belongs to, sampled at the rising edge of each
    /// T-state's clock. One entry per T-state, starting at the step that requests the action.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<string>>? Pins { get; private set; }

    public override string ToString() => Name;
}