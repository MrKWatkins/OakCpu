using System.Globalization;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Instruction
{
    private Instruction(string group, string mnemonic, byte? prefix, byte opcode,bool nextOpcodeOverlapped, IReadOnlyList<Step> steps, IReadOnlyDictionary<string, Expression> flags)
    {
        if (steps.Count == 0)
        {
            throw new ArgumentException("Value cannot be empty.", nameof(steps));
        }
        Group = group;
        Mnemonic = mnemonic;
        Prefix = prefix;
        Opcode = opcode;
        NextOpcodeOverlapped = nextOpcodeOverlapped;
        Steps = steps;
        Flags = flags;

        foreach (var step in steps)
        {
            step.Instruction = this;
        }
    }

    public string Group { get; }

    public string Mnemonic { get; }

    public byte? Prefix { get; }

    public byte Opcode { get; }

    public bool NextOpcodeOverlapped { get; }

    public IReadOnlyList<Step> Steps { get; }

    public IReadOnlyDictionary<string, Expression> Flags { get; }

    public override string ToString() => $"0x{Opcode:X2}: {Mnemonic}";

    [Pure]
    public static IReadOnlyList<Instruction> Create(ParserContext context, IReadOnlyList<InstructionYaml> yamls)
    {
        var instructions = yamls.SelectMany(y => Create(context, y)).OrderBy(f => f.Prefix).ThenBy(f => f.Opcode).ToList();

        var prefixInstructions = CreatePrefixJumpInstructions(context, instructions);

        return prefixInstructions.Concat(instructions).ToList();
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
                .Select((expressions, index) => Step.Parse($"{opcodeYaml.Opcode}: {mnemonic} [{index}]", context, Substitute(context, opcodeYaml, expressions), index == yaml.Steps.Count - 1 ? lastStepFinalStatement : null))
                .ToList();

            var flags = yaml.Flags.ToDictionary(kvp => kvp.Key, kvp => Parser.ParseExpression(context, Substitute(context, opcodeYaml, kvp.Value)));

            var (prefix, opcode) = ParseOpcode(opcodeYaml.Opcode);

            yield return new Instruction(yaml.Group, mnemonic, prefix, opcode, yaml.NextOpcode == NextOpcodeMode.Overlapped, steps, flags);
        }
    }

    [Pure]
    private static IEnumerable<Instruction> CreatePrefixJumpInstructions(ParserContext context, IReadOnlyList<Instruction> instructions)
    {
        foreach (var prefix in instructions.Where(i => i.Prefix.HasValue).Select(i => i.Prefix!.Value).Distinct().OrderBy(p => p))
        {
            var steps = Step.Parse($"Move to read opcode after prefix 0x{prefix:X2}", context, [$"{DataMember.Prefix.Name} = {DataMember.Data.Name};"], OverlappedOpcodeRead.Instance);

            yield return new Instruction("Prefixes", $"Prefix 0x{prefix:X2}", null, prefix, true, steps, new Dictionary<string, Expression>());
        }
    }

    [Pure]
    private static string Substitute(ParserContext context, OpcodeYaml opcodeYaml, string? value)
    {
        if (value == null)
        {
            return "";
        }

        value = ReplaceRegister(context, value, "R0", opcodeYaml.R0);
        value = ReplaceRegister(context, value, "R1", opcodeYaml.R1);
        value = ReplaceRegister(context, value, "RP0", opcodeYaml.RP0);
        value = ReplaceRegister(context, value, "RP1", opcodeYaml.RP1);
        value = ReplaceCondition(context, value, "C0", opcodeYaml.C0);
        value = ReplaceNumber(value, "N0", opcodeYaml.N0);
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

    [Pure]
    private static string ReplaceCondition(ParserContext context, string value, string conditionVariable, string? replacement)
    {
        if (replacement != null)
        {
            var condition = context.Conditions[replacement];

            value = value.Replace(conditionVariable, condition.Name);
        }

        return value;
    }

    [Pure]
    private static string ReplaceNumber(string value, string conditionVariable, byte? replacement)
    {
        if (replacement != null)
        {
            value = value.Replace(conditionVariable, $"0x{replacement.Value:X2}");
        }

        return value;
    }

    [Pure]
    private static (byte? Prefix, byte Opcode) ParseOpcode(string value)
    {
        var values = value
            .Split([' '], StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseHex)
            .ToArray();

        return values.Length == 2 ? (values[0], values[1]) : (null, values[0]);
    }

    [Pure]
    private static byte ParseHex(string hex)
    {
        if (hex.StartsWith("0x"))
        {
            return byte.Parse(hex.Substring(2), NumberStyles.HexNumber);
        }
        throw new InvalidOperationException($"{hex} is not a hex number.");
    }
}