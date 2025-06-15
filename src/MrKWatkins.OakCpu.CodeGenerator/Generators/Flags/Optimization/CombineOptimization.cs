using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Optimization;

internal abstract class CombineOptimization<TAction> : FlagOptimization
    where TAction : FlagAction
{
    protected sealed override IEnumerable<FlagAction> Optimize(StepContext context, IReadOnlyList<FlagAction> actions)
    {
        var actionsToCombine = new List<TAction>();
        foreach (var action in actions)
        {
            if (action is TAction typedAction)
            {
                actionsToCombine.Add(typedAction);
            }
            else
            {
                yield return action;
            }
        }

        if (actionsToCombine.Any())
        {
            foreach (var combined in Combine(context, actionsToCombine))
            {
                yield return combined;
            }
        }
    }

    [Pure]
    private protected abstract IEnumerable<TAction> Combine(StepContext context, IReadOnlyList<TAction> actions);
}