using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Instruction : StepSequence
{
    private Instruction(string group, string mnemonic, string? opcodeTable, byte? prefix, byte opcode, NextOpcodeMode nextOpcode, string? overlappedSequence, IReadOnlyList<Step> steps, IReadOnlyDictionary<string, Expression> flags, IReadOnlyList<(byte? Prefix, byte Opcode, Step Step)> duplicates)
        : base(null, steps, nextOpcode, overlappedSequenceName: overlappedSequence)
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
    public static IReadOnlyList<Instruction> Create(ParserContext context, IReadOnlyList<InstructionYaml> yamls) =>
        yamls.SelectMany(y => Create(context, y)).OrderBy(i => i.OpcodeTable).ThenBy(i => i.Prefix).ThenBy(i => i.Opcode).ToList();

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

            ResolveTypesInFlagsExpressions(steps, flags);

            yield return new Instruction(yaml.Group, mnemonic, yaml.OpcodeTable, opcodeYaml.PrefixByte, opcodeYaml.OpcodeByte, yaml.NextOpcode, yaml.OverlappedSequence, steps, flags, duplicates);
        }
    }

    private static void ResolveTypesInFlagsExpressions(IReadOnlyList<Step> steps, IReadOnlyDictionary<string, Expression> flags)
    {
        if (flags.Count == 0)
        {
            return;
        }

        foreach (var step in steps)
        {
            ResolveTypesInFlagsExpressions(step, flags);
        }
    }
    private static void ResolveTypesInFlagsExpressions(Step steps, IReadOnlyDictionary<string, Expression> flags)
    {
        var variables = new Dictionary<string, TemporaryVariable>();
        foreach (var node in steps.Statements.SelectMany(s => s.TraverseDepthFirst()))
        {
            if (node is IReferencesTemporaryVariable referencesTemporaryVariable)
            {
                variables[referencesTemporaryVariable.Variable.Name] = referencesTemporaryVariable.Variable;
            }
            else if (node is CallStatement call && call.Call.Function == PreDefinedFunction.Flags)
            {
                // When calling the flag all temps needed should have been defined. TODO: Should be doing something different to handle if/else branches correctly, i.e. a scoped walk.
                ResolveTypesInFlagsExpressions(variables, flags);
                return;
            }
        }
    }

    private static void ResolveTypesInFlagsExpressions(IReadOnlyDictionary<string, TemporaryVariable> temporaryVariables, IReadOnlyDictionary<string, Expression> flags)
    {
        foreach (var kvp in flags)
        {
            var flag = kvp.Key;

            foreach (var variableReference in kvp.Value.TraverseDepthFirst().OfType<IReferencesTemporaryVariable>())
            {
                if (!temporaryVariables.TryGetValue(variableReference.Variable.Name, out var variable))
                {
                    throw new InvalidOperationException($"The temporary variable {variableReference.Variable.Name} referenced by flag {flag} has not been defined.");
                }
                variableReference.Variable.Type = variable.Type;
            }
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
                value = value.Replace($"{registerVariable}H", register.HighRegister.Name, StringComparison.Ordinal);
            }
            if (register.LowRegister != null)
            {
                value = value.Replace($"{registerVariable}L", register.LowRegister.Name, StringComparison.Ordinal);
            }

            value = value.Replace(registerVariable, register.Name, StringComparison.Ordinal);
        }

        return value;
    }

    [Pure]
    private static string ReplaceCondition(ParserContext context, string value, string conditionVariable, string? replacement)
    {
        if (replacement != null)
        {
            var condition = context.Configuration.Conditions[replacement];

            value = value.Replace(conditionVariable, condition.Name, StringComparison.Ordinal);
        }

        return value;
    }

    [Pure]
    private static string ReplaceNumber(string value, string conditionVariable, byte? replacement)
    {
        if (replacement != null)
        {
            value = value.Replace(conditionVariable, $"0x{replacement.Value:X2}", StringComparison.Ordinal);
        }

        return value;
    }
}