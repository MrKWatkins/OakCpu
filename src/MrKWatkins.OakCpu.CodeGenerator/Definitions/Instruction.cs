using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Instruction
{
    private Instruction(string group, string mnemonic, byte opcode, byte? prefix, IReadOnlyList<Step> steps, bool nextOpcodeOverlapped)
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
        NextOpcodeOverlapped = nextOpcodeOverlapped;
    }

    public string Group { get; }

    public string Mnemonic { get; }

    public byte Opcode { get; }

    public byte? Prefix { get; }

    public IReadOnlyList<Step> Steps { get; }

    public bool NextOpcodeOverlapped { get; }

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
            var mnemonic = Substitute(context, opcodeYaml, yaml.Mnemonic);

            Statement lastStepFinalStatement = yaml.NextOpcode switch
            {
                NextOpcodeMode.Normal => MoveToOpcodeRead.Instance,
                NextOpcodeMode.Overlapped => OverlappedOpcodeRead.Instance,
                _ => throw new NotSupportedException($"The {nameof(NextOpcodeMode)} {yaml.NextOpcode} is not supported.")
            };

            var steps = yaml.Steps
                .Select((expressions, index) => Step.Parse($"{mnemonic} [{index}]", context, Substitute(context, opcodeYaml, expressions), index == yaml.Steps.Count - 1 ? lastStepFinalStatement : null))
                .ToList();

            yield return new Instruction(yaml.Group, mnemonic, opcodeYaml.Opcode, opcodeYaml.Prefix, steps, yaml.NextOpcode == NextOpcodeMode.Overlapped);
        }
    }

    [Pure]
    private static IEnumerable<string> Substitute(ParserContext context, OpcodeYaml opcodeYaml, IEnumerable<string> values) => values.Select(v => Substitute(context, opcodeYaml, v));

    [Pure]
    private static string Substitute(ParserContext context, OpcodeYaml opcodeYaml, string value)
    {
        value = ReplaceRegister(context, value, "R0", opcodeYaml.R0);
        value = ReplaceRegister(context, value, "R1", opcodeYaml.R1);
        value = ReplaceRegister(context, value, "RP0", opcodeYaml.RP0);
        value = ReplaceRegister(context, value, "RP1", opcodeYaml.RP1);
        return value;
    }

    [Pure]
    private static string ReplaceRegister(ParserContext context, string value, string registerVariable, string? replacement)
    {
        if (replacement != null)
        {
            var register = context.Registers[replacement];

            if (register.HighRegister != null)
            {
                value = value.Replace($"{registerVariable}H", register.HighRegister.Name);
            }
            if (register.LowRegister != null)
            {
                value = value.Replace($"{registerVariable}L", register.LowRegister.Name);
            }

            value = value.Replace(registerVariable, register.Name);
        }

        return value;
    }
}