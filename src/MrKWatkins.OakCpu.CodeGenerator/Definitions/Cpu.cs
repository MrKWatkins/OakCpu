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

    [MustUseReturnValue]
    public static Cpu Create(Configuration configuration, CpuYaml yaml)
    {
        configuration.Actions = yaml.Actions.Select((action, index) => new Action(action, index + 1)).Prepend(Action.None).ToDictionary(a => a.Name, a => a);
        configuration.UserDefinedDataMembers = UserDefinedDataMember.Create(yaml.Fields);


        return new Cpu(yaml.Name);
    }
}