using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Optimization;

internal sealed class CombineConstants : CombineOptimization<Constant>
{
    private protected override IEnumerable<Constant> Combine(StepContext context, IReadOnlyList<Constant> constants)
    {
        if (constants.Count == 0)
        {
            yield return constants[0];
            yield break;
        }

        byte bitMask = 0;
        var flags = new List<Flag>(constants.Count);
        foreach (var constant in constants)
        {
            bitMask |= constant.BitMask;
            flags.AddRange(constant.Flags);
        }
        yield return new Constant(flags, bitMask);
    }
}