using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Cpu
{
    private Cpu(string name, IReadOnlyList<string> actions)
    {
        Name = name;
        Actions = actions;
    }

    public string Name { get; }

    public IReadOnlyList<string> Actions { get; }

    public override string ToString() => Name;

    [Pure]
    public static Cpu Create(CpuYaml yaml) => new(yaml.Name, yaml.Actions);
}