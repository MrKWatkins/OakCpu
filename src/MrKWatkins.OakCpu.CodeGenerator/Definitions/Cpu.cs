using MrKWatkins.OakCpu.CodeGenerator.Expressions.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Cpu
{
    private Cpu(string name, IReadOnlyList<string> actions, IReadOnlyList<Step> opcodeRead)
    {
        Name = name;
        Actions = actions;
        OpcodeRead = opcodeRead;
    }

    public string Name { get; }

    public IReadOnlyList<string> Actions { get; }

    public IReadOnlyList<Step> OpcodeRead { get; }

    public override string ToString() => Name;

    public static Cpu Create(CpuYaml yaml, Dictionary<string, Register> registersByName, Dictionary<string, Flag> flagsByName)
    {
        var context = new ParserContext(new HashSet<string>(yaml.Actions), registersByName, flagsByName);

        var opcodeRead = yaml.OpcodeRead.Select(s => Step.Parse(context, s)).ToList();

        return new Cpu(yaml.Name, yaml.Actions, opcodeRead);
    }
}