using System.Collections;
using System.Collections.Frozen;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class OpcodeStepTables : IEnumerable<OpcodeStepTable>
{
    internal OpcodeStepTables([InstantHandle] IEnumerable<InstructionYaml> instructionYamls)
    {
        var prefixes = new Dictionary<byte, OpcodeStepTable>();
        var custom = new Dictionary<string, OpcodeStepTable>();

        foreach (var instructionYaml in instructionYamls)
        {
            if (instructionYaml.OpcodeTable != null)
            {
                custom.TryAdd(instructionYaml.OpcodeTable, OpcodeStepTable.CreateCustom(instructionYaml.OpcodeTable));
                continue;
            }

            foreach (var opcodeYaml in instructionYaml.Opcodes)
            {
                var prefix = opcodeYaml.PrefixByte;
                if (prefix != null && !prefixes.ContainsKey(prefix.Value))
                {
                    prefixes.Add(prefix.Value, OpcodeStepTable.CreatePrefix(prefix.Value));
                }
            }
        }

        Prefixes = prefixes.ToFrozenDictionary();
        Custom = custom.ToFrozenDictionary();
    }

#pragma warning disable CA1822
    public OpcodeStepTable NoPrefix => OpcodeStepTable.NoPrefix;
#pragma warning restore CA1822

    public IReadOnlyDictionary<byte, OpcodeStepTable> Prefixes { get; }

    public IReadOnlyDictionary<string, OpcodeStepTable> Custom { get; }

    /// <summary>
    /// Gets a value indicating whether the CPU has any prefixed or custom opcode tables beyond the default no-prefix
    /// table. CPUs without prefixes, such as the 6502, only ever use the no-prefix table.
    /// </summary>
    public bool HasPrefixes => Prefixes.Count > 0 || Custom.Count > 0;

    [Pure]
    public OpcodeStepTable GetForPrefix(byte prefix) => Prefixes[prefix];

    [Pure]
    public OpcodeStepTable GetForInstruction(Instruction instruction)
    {
        if (instruction.Prefix != null)
        {
            return GetForPrefix(instruction.Prefix.Value);
        }
        return instruction.OpcodeTable != null ? Custom[instruction.OpcodeTable] : NoPrefix;
    }

    public IEnumerator<OpcodeStepTable> GetEnumerator()
    {
        yield return NoPrefix;
        foreach (var prefix in Prefixes.Values.OrderBy(p => p.Prefix))
        {
            yield return prefix;
        }
        foreach (var custom in Custom.Values.OrderBy(p => p.CustomName))
        {
            yield return custom;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}