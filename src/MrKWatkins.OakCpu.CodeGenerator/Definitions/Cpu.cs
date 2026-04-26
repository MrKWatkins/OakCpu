using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Cpu
{
    private Cpu(string name, Documentation documentation)
    {
        Name = name;
        Documentation = documentation;
    }

    public string Name { get; }

    public Documentation Documentation { get; }

    public override string ToString() => Name;

    [Pure]
    public static Cpu Create(CpuYaml yaml) => new(yaml.Name, Documentation.CreateOptional(yaml.Documentation, $"CPU {yaml.Name}"));
}