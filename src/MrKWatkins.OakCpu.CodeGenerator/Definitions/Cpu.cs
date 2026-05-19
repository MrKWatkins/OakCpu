using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Cpu
{
    private Cpu(string name, Documentation documentation, NextOpcodeMode defaultNextOpcode)
    {
        Name = name;
        Documentation = documentation;
        DefaultNextOpcode = defaultNextOpcode;
    }

    public string Name { get; }

    public Documentation Documentation { get; }

    public NextOpcodeMode DefaultNextOpcode { get; }

    public override string ToString() => Name;

    [Pure]
    public static Cpu Create(CpuYaml yaml) => new(yaml.Name, Documentation.CreateOptional(yaml.Documentation, $"CPU {yaml.Name}"), yaml.EffectiveDefaultNextOpcode);
}