using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Instruction : StepSequence
{
    private Instruction(string group, string mnemonic, string? opcodeTable, byte? prefix, byte opcode, NextOpcodeMode nextOpcode, IReadOnlyList<Step> steps, IReadOnlyDictionary<string, Expression> flags, IReadOnlyList<(byte? Prefix, byte Opcode, Step Step)> duplicates)
        : base(steps, nextOpcode)
    {
        Group = group;
        Mnemonic = mnemonic;
        OpcodeTable = opcodeTable;
        Prefix = prefix;
        Opcode = opcode;
        Flags = flags;
        Duplicates = duplicates;
    }

    public string Group { get; }

    public string Mnemonic { get; }

    public string? OpcodeTable { get; }

    public byte? Prefix { get; }

    public byte Opcode { get; }

    public IReadOnlyDictionary<string, Expression> Flags { get; }

    public IReadOnlyList<(byte? Prefix, byte Opcode, Step Step)> Duplicates { get; }

    public bool UpdatesFlags => Flags.Any();

    [Pure]
    public static IReadOnlyList<Instruction> Create(ParserContext context, IReadOnlyList<InstructionYaml> yamls)
    {
        VerifyNoDuplicateOpcodes(yamls);

        var instructions = yamls.SelectMany(y => Create(context, y)).OrderBy(i => i.OpcodeTable).ThenBy(i => i.Prefix).ThenBy(i => i.Opcode).ToList();

        var prefixInstructions = CreatePrefixJumpInstructions(context, instructions);

        return prefixInstructions.Concat(instructions).ToList();
    }

    [Pure]
    private static IEnumerable<Instruction> Create(ParserContext context, InstructionYaml yaml)
    {
        var tablePrefix = yaml.OpcodeTable != null ? $"{yaml.OpcodeTable} " : "";

        foreach (var group in yaml.Opcodes.GroupBy(o => o, OpcodeYamlDuplicateEqualityComparer.Instance))
        {
            var opcodeAndDuplicates = group.ToList();
            opcodeAndDuplicates.Sort(OpcodeYamlNoPrefixFirstComparer.Instance);

            var opcodeYaml = opcodeAndDuplicates[0];

            var mnemonic = Substitute(context, opcodeYaml, yaml.Mnemonic);

            var steps = yaml.Steps
                .Select((expressions, index) =>
                {
                    var step = Substitute(context, opcodeYaml, expressions);

                    var requiresCompleteInstruction = yaml.NextOpcode != NextOpcodeMode.Custom && index == yaml.Steps.Count - 1;

                    return Step.Parse($"{tablePrefix}{opcodeYaml.Opcode}: {mnemonic} [{index}]", context, step, requiresCompleteInstruction);
                })
                .ToList();

            var duplicates = opcodeAndDuplicates.Skip(1).Select(y => (y.PrefixByte, y.OpcodeByte, steps[0])).ToList();

            var flags = yaml.Flags.ToDictionary(kvp => kvp.Key, kvp => Parser.ParseExpression(context, Substitute(context, opcodeYaml, kvp.Value)));

            yield return new Instruction(yaml.Group, mnemonic, yaml.OpcodeTable, opcodeYaml.PrefixByte, opcodeYaml.OpcodeByte, yaml.NextOpcode, steps, flags, duplicates);
        }
    }

    [Pure]
    private static IEnumerable<Instruction> CreatePrefixJumpInstructions(ParserContext context, IReadOnlyList<Instruction> instructions)
    {
        foreach (var prefix in instructions.Where(i => i.Prefix.HasValue).Select(i => i.Prefix!.Value).Distinct().OrderBy(p => p))
        {
            var steps = Step.Parse($"Read opcode after prefix 0x{prefix:X2}", context, [$"{PreDefinedFunction.SetOpcodeStepTable.Name}({prefix});"]);

            yield return new Instruction("Prefixes", $"Prefix 0x{prefix:X2}", null, null, prefix, NextOpcodeMode.Overlapped, steps, new Dictionary<string, Expression>(), []);
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
            var register = context.Configuration.Registers[replacement];

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
            var condition = context.Configuration.Conditions[replacement];

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

    private static void VerifyNoDuplicateOpcodes(IReadOnlyList<InstructionYaml> yamls)
    {
        var firstDuplicates = yamls.SelectMany(y => y.Opcodes.Select(o => (y.OpcodeTable, o.PrefixByte, o.OpcodeByte))).GroupBy(x => x).FirstOrDefault(g => g.Count() > 1);
        if (firstDuplicates != null)
        {
            var duplicates = firstDuplicates.Key;
            var groupText = duplicates.OpcodeTable != null ? $"in opcode table {duplicates.OpcodeTable} " : "";

            throw new InvalidOperationException(
                $"The opcodes {groupText}are defined multiple times: {(duplicates.PrefixByte.HasValue ? $"0x{duplicates.PrefixByte.Value:X2} " : "")}0x{duplicates.OpcodeByte:X2}");
        }
    }
}