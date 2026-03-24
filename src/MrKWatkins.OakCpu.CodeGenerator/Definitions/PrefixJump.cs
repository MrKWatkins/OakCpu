using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class PrefixJump : StepSequence
{
    private PrefixJump(byte prefix, IReadOnlyList<Step> steps)
        // Overlapped because this is an extra step we're using just to change the jump table, it shouldn't count as a T-state. So we need to overlap with the opcode read.
        : base(null, steps, NextOpcodeMode.Overlapped)
    {
        Prefix = prefix;
    }

    public byte Prefix { get; }

    [Pure]
    public static IReadOnlyDictionary<byte, PrefixJump> Create(ParserContext context, IReadOnlyList<Instruction> instructions)
    {
        var result = new Dictionary<byte, PrefixJump>();
        foreach (var prefix in instructions.Where(i => i.Prefix.HasValue).Select(i => i.Prefix!.Value).Distinct().OrderBy(p => p))
        {
            var steps = Step.Parse($"Read opcode after prefix 0x{prefix:X2}", context, [$"{PreDefinedFunction.SetOpcodeStepTable.Name}({prefix});"]);

            result.Add(prefix, new PrefixJump(prefix, steps));
        }

        return result;
    }
}
