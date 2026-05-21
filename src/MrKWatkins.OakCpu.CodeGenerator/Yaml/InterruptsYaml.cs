using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class InterruptsYaml
{
    private IReadOnlyList<FieldYaml>? properties;
    private IReadOnlyList<InterruptModeYaml>? modes;

    private InterruptsYaml()
    {
    }

    public string? Handle { get; private set; }

    public IReadOnlyList<FieldYaml> Properties
    {
        get => properties ?? [];
        private set => properties = value;
    }

    public IReadOnlyList<InterruptModeYaml> Modes
    {
        get => modes ?? [];
        private set => modes = value;
    }
}