using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class InterruptsYaml
{
    private IReadOnlyList<FieldYaml>? properties;

    private InterruptsYaml()
    {
    }

    public IReadOnlyList<FieldYaml> Properties
    {
        get => properties ?? [];
        private set => properties = value;
    }
}