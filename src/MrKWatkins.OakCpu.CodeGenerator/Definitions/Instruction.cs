using MrKWatkins.OakCpu.CodeGenerator.Expressions.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Instruction
{
    private Instruction(string group, string mnemonic, byte opcode, byte? prefix, IReadOnlyList<Step> steps)
    {
        if (steps.Count == 0)
        {
            throw new ArgumentException("Value cannot be empty.", nameof(steps));
        }
        Group = group;
        Mnemonic = mnemonic;
        Opcode = opcode;
        Prefix = prefix;
        Steps = steps;
    }

    public string Group { get; }

    public string Mnemonic { get; }

    public byte Opcode { get; }

    public byte? Prefix { get; }

    public IReadOnlyList<Step> Steps { get; }

    public override string ToString() => $"0x{Opcode:X2}: {Mnemonic}";

    [Pure]
    public static IReadOnlyList<Instruction> Create(IReadOnlyList<string> actions, IReadOnlyDictionary<string, Register> registersByName, IReadOnlyList<InstructionYaml> yamls)
    {
        var context = new ParserContext(new HashSet<string>(actions), registersByName);

        return yamls.SelectMany(y => Create(context, y)).OrderBy(f => f.Prefix).ThenBy(f => f.Opcode).ToList();
    }

    [Pure]
    private static IEnumerable<Instruction> Create(ParserContext context, InstructionYaml yaml)
    {
        foreach (var opcodeYaml in yaml.Opcodes)
        {
            var mnemonic = Substitute(opcodeYaml, yaml.Mnemonic);

            var steps = yaml.Steps.Select(expressions => Step.Parse(context, Substitute(opcodeYaml, expressions))).ToList();

            yield return new Instruction(yaml.Group, mnemonic, opcodeYaml.Opcode, opcodeYaml.Prefix, steps);
        }
    }

    [Pure]
    private static IEnumerable<string> Substitute(OpcodeYaml opcodeYaml, IEnumerable<string> values) => values.Select(v => Substitute(opcodeYaml, v));

    [Pure]
    private static string Substitute(OpcodeYaml opcodeYaml, string value)
    {
        if (opcodeYaml.R0 != null)
        {
            value = value.Replace("R0", opcodeYaml.R0);
        }
        if (opcodeYaml.R1 != null)
        {
            value = value.Replace("R1", opcodeYaml.R1);
        }
        return value;
    }
}