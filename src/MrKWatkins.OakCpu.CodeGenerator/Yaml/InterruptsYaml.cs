using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class InterruptsYaml
{
    private InterruptsYaml()
    {
    }

    public string? Handle { get; private init; }

    public IReadOnlyList<FieldYaml> Properties
    {
        get => field ?? [];
        private init;
    }

    public IReadOnlyList<InterruptModeYaml> Modes
    {
        get => field ?? [];
        private init;
    }
}