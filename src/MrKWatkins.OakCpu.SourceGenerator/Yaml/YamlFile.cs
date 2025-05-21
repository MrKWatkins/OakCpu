using VYaml.Annotations;

namespace MrKWatkins.OakCpu.SourceGenerator.Yaml;

[YamlObject]
public sealed partial class YamlFile
{
    private YamlFile()
    {
    }

    public IReadOnlyList<RegisterYaml> Registers { get; set; } = [];
}