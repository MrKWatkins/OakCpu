using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;
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

    public static Cpu Create(CpuYaml yaml, IReadOnlyDictionary<string, Register> registers, IReadOnlyDictionary<string, Flag> flags, IReadOnlyDictionary<string, Condition> conditions)
    {
        var context = new ParserContext(new HashSet<string>(yaml.Actions), registers, flags, conditions);

        var opcodeRead = Step.Parse("Opcode read", context, yaml.OpcodeRead, OpcodeJump.Instance);

        return new Cpu(yaml.Name, yaml.Actions, opcodeRead);
    }
}