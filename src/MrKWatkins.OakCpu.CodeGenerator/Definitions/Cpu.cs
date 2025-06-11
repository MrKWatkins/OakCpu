using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Cpu
{
    private Cpu(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public override string ToString() => Name;

    [Pure]
    public static Cpu Create(CpuYaml yaml) => new(yaml.Name);
}