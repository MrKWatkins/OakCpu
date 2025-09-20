using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Optimization;

internal sealed class RemoveUnnecessaryResets : FlagOptimization
{
    protected override IEnumerable<FlagAction> Optimize(StatementGeneratorContext context, IReadOnlyList<FlagAction> actions, List<string> extraComments)
    {
        // If we are just resetting flags, and we have copy_froms, then we can remove the reset and initialize directly from the first copy_from().
        // Must run *after* CombineConstants!
        var constant = actions.OfType<Constant>().FirstOrDefault();
        if (constant is null || constant.BitMask != 0 || actions.All(a => a is not CopyFrom))
        {
            return actions;
        }

        extraComments.Add(constant.GenerateComment());

        return actions.Where(a => a != constant);
    }
}