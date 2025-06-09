using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Cpu
{
    private Cpu(string name, IReadOnlyDictionary<string, Action> actions, IReadOnlyList<Step> opcodeRead)
    {
        Name = name;
        Actions = actions;
        OpcodeRead = opcodeRead;
    }

    public string Name { get; }

    public IReadOnlyDictionary<string, Action> Actions { get; }

    public IReadOnlyList<Step> OpcodeRead { get; }

    public override string ToString() => Name;

    public static Cpu Create(CpuYaml yaml, IReadOnlyDictionary<string, Register> registers, IReadOnlyDictionary<string, Flag> flags, IReadOnlyDictionary<string, Condition> conditions)
    {
        var actions = yaml.Actions.Select((action, index) => new Action(action, index + 1)).Prepend(Action.None).ToDictionary(a => a.Name, a => a);

        var context = new ParserContext(actions, registers, flags, conditions);

        var opcodeRead = Step.Parse("Opcode read", context, yaml.OpcodeRead);

        return new Cpu(yaml.Name, actions, opcodeRead);
    }
}