using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Optimization;

// Transforms tried that did not work:
// * Copying registers to local fields. The JIT already caches the field values.
// * Unsafe.BitCast<bool, byte>(A == constant) for is_zero rather than A == 0 ? 1 : 0 because it wasn't always generating a sete.
// * Unsafe.BitCast<bool, byte>(A == 0)) << 6 when setting Z to is_zero rather than A == 0 ? 64 : 0 as the latter generates more assembly instructions.
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