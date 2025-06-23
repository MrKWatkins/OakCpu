using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class InterruptsYaml
{
    private IReadOnlyList<InterruptPropertyYaml>? properties;

    private InterruptsYaml()
    {
    }

    public IReadOnlyList<InterruptPropertyYaml> Properties
    {
        get => properties ?? [];
        private set => properties = value;
    }
}