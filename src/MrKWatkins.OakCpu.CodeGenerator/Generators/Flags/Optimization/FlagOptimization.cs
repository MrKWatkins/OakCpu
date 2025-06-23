using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Optimization;

internal abstract class FlagOptimization
{
    private static readonly IReadOnlyList<FlagOptimization> All =
    [
        new CombineConstants(),
        new CombineCopyFroms(),
        new CombineI32BoolShifts(),
        new RemoveUnnecessaryResets()
    ];

    internal static IReadOnlyList<FlagAction> PerformAllOptimizations(StatementGeneratorContext context, IReadOnlyList<FlagAction> actions, List<string> extraComments) =>
        All.Aggregate(actions, (current, optimization) => optimization.Optimize(context, current, extraComments).ToList());

    [Pure]
    protected abstract IEnumerable<FlagAction> Optimize(StatementGeneratorContext context, IReadOnlyList<FlagAction> actions, List<string> extraComments);
}

internal abstract class FlagOptimization<TAction> : FlagOptimization
    where TAction : FlagAction
{
    protected sealed override IEnumerable<FlagAction> Optimize(StatementGeneratorContext context, IReadOnlyList<FlagAction> actions, List<string> extraComments)
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
    protected abstract TAction Optimize(StatementGeneratorContext context, TAction action);
}