using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class CpuYaml
{
    private CpuYaml()
    {
    }

    public string Name { get; private set; } = null!;

    public override string ToString() => Name;
}