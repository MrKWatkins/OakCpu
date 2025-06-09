using System.Collections;
using System.Collections.Frozen;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator;

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
                if (!custom.ContainsKey(instructionYaml.OpcodeTable))
                {
                    custom.Add(instructionYaml.OpcodeTable, OpcodeStepTable.CreateCustom(instructionYaml.OpcodeTable));
                }
                continue;
            }

            foreach (var opcodeYaml in instructionYaml.Opcodes)
            {
                var (prefix, _) = opcodeYaml.GetBytes();
                if (prefix != null)
                {
                    if (!prefixes.ContainsKey(prefix.Value))
                    {
                        prefixes.Add(prefix.Value, OpcodeStepTable.CreatePrefix(prefix.Value));
                    }
                }
            }
        }

        Prefixes = prefixes.ToFrozenDictionary();
        Custom = custom.ToFrozenDictionary();
    }

    public OpcodeStepTable NoPrefix => OpcodeStepTable.NoPrefix;

    public IReadOnlyDictionary<byte, OpcodeStepTable> Prefixes { get; }

    public IReadOnlyDictionary<string, OpcodeStepTable> Custom { get; }

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
        foreach (var prefix in Prefixes.Values)
        {
            yield return prefix;
        }
        foreach (var custom in Custom.Values)
        {
            yield return custom;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}