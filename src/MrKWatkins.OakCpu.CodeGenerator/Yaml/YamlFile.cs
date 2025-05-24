using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class YamlFile
{
    private YamlFile()
    {
    }

    public IReadOnlyList<RegisterYaml> Registers { get; set; } = [];

    [Pure]
    public static YamlFile Combine(params IEnumerable<YamlFile> files)
    {
        var registers = new List<RegisterYaml>();
        foreach (var file in files)
        {
            registers.AddRange(file.Registers);
        }
        return new YamlFile { Registers = registers };
    }
}