using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Optimization;

internal sealed class CombineCopyFroms : CombineOptimization<CopyFrom>
{
    private protected override IEnumerable<CopyFrom> Combine(StatementGeneratorContext context, IReadOnlyList<CopyFrom> copyFroms)
    {
        var grouped = copyFroms.GroupBy(c => c.Argument).ToList();
        return grouped.Count == copyFroms.Count ? copyFroms : Combine(grouped);
    }

    [Pure]
    private static IEnumerable<CopyFrom> Combine([InstantHandle] IEnumerable<IGrouping<Expression, CopyFrom>> grouped)
    {
        foreach (var group in grouped)
        {
            if (group.Count() == 1)
            {
                yield return group.First();
                continue;
            }

            byte bitMask = 0;
            var flags = new List<Flag>();
            foreach (var copyFrom in group)
            {
                bitMask |= copyFrom.BitMask;
                flags.AddRange(copyFrom.Flags);
            }

            yield return new CopyFrom(flags, bitMask, group.Key);
        }
    }
}