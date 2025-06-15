using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Optimization;

internal abstract class FlagOptimization
{
    private static readonly IReadOnlyList<FlagOptimization> All =
    [
        new CombineConstants(),
        new CombineCopyFroms(),
        new CombineI32BoolShifts()
    ];

    internal static IReadOnlyList<FlagAction> PerformAllOptimizations(StepContext context, IReadOnlyList<FlagAction> actions) =>
        All.Aggregate(actions, (current, optimization) => optimization.Optimize(context, current).ToList());

    [Pure]
    protected abstract IEnumerable<FlagAction> Optimize(StepContext context, IReadOnlyList<FlagAction> actions);
}

internal abstract class FlagOptimization<TAction> : FlagOptimization
    where TAction : FlagAction
{
    protected sealed override IEnumerable<FlagAction> Optimize(StepContext context, IReadOnlyList<FlagAction> actions)
    {
        foreach (var action in actions)
        {
            if (action is TAction typedAction)
            {
                yield return Optimize(context, typedAction);
            }
            else
            {
                yield return action;
            }
        }
    }

    [Pure]
    protected abstract TAction Optimize(StepContext context, TAction action);
}