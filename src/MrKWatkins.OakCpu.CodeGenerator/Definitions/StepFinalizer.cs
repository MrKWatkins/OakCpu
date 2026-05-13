using System.ComponentModel;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

/// <summary>
/// Finalizes parsed <see cref="Step" /> instances into immutable layout data used by code generation.
/// </summary>
internal static class StepFinalizer
{
    /// <summary>
    /// Builds the finalized layout data for the supplied steps.
    /// </summary>
    /// <param name="steps">The parsed steps in their final execution order.</param>
    /// <param name="stepSequences">The owning sequence for each parsed step.</param>
    /// <returns>The finalized step layouts and the subset of steps that need generated handler methods.</returns>
    [Pure]
    internal static StepLayoutState Finalize(IReadOnlyList<Step> steps, IReadOnlyDictionary<Step, StepSequence> stepSequences)
    {
        var indices = AssignIndices(steps);
        var duplicateGroups = MapDuplicates(steps, stepSequences).ToList();
        var methodIndices = AssignMethodIndices(duplicateGroups, stepSequences);
        var layouts = CreateLayouts(stepSequences, indices, methodIndices, duplicateGroups);
        var functionSteps = duplicateGroups.Select(group => group.Implementation).ToList();
        return new StepLayoutState(layouts, functionSteps);
    }

    [Pure]
    private static IReadOnlyDictionary<Step, ushort> AssignIndices(IEnumerable<Step> steps)
    {
        var index = 0;
        var indices = new Dictionary<Step, ushort>();
        foreach (var step in steps)
        {
            if (index > ushort.MaxValue)
            {
                throw new InvalidAsynchronousStateException("Too many steps; will need to change to int.");
            }

            indices.Add(step, (ushort)index++);
        }

        return indices;
    }

    [Pure]
    private static IReadOnlyDictionary<Step, ushort?> AssignMethodIndices(IEnumerable<DuplicateGroup> groups, IReadOnlyDictionary<Step, StepSequence> stepSequences)
    {
        var index = 0;
        var methodIndices = new Dictionary<Step, ushort?>();
        foreach (var group in groups)
        {
            if (StepLayout.GetDoesNothing(group.Implementation, stepSequences[group.Implementation]))
            {
                foreach (var step in group.ImplementationAndDuplicates)
                {
                    methodIndices.Add(step, null);
                }
                continue;
            }

            var methodIndex = (ushort)index++;
            foreach (var step in group.ImplementationAndDuplicates)
            {
                methodIndices.Add(step, methodIndex);
            }
        }

        return methodIndices;
    }

    [Pure]
    private static IReadOnlyDictionary<Step, StepLayout> CreateLayouts(
        IReadOnlyDictionary<Step, StepSequence> stepSequences,
        IReadOnlyDictionary<Step, ushort> indices,
        IReadOnlyDictionary<Step, ushort?> methodIndices,
        IReadOnlyList<DuplicateGroup> duplicateGroups) =>
        duplicateGroups
            .SelectMany(group =>
            {
                var duplicates = group.ImplementationAndDuplicates.Skip(1).ToList();

                return group.ImplementationAndDuplicates.Select(step => new KeyValuePair<Step, StepLayout>(
                    step,
                    new StepLayout(
                        step,
                        stepSequences[step],
                        indices[step],
                        methodIndices[step],
                        group.Implementation,
                        step == group.Implementation ? duplicates : [],
                        group.ImplementationAndDuplicates)));
            })
            .ToDictionary();

    [Pure]
    private static IEnumerable<DuplicateGroup> MapDuplicates(IEnumerable<Step> steps, IReadOnlyDictionary<Step, StepSequence> stepSequences) =>
        steps.GroupBy(step => step, new StepDuplicateEqualityComparer(stepSequences))
            .Select(group => group.ToList())
            .Select(implementationAndDuplicates => new DuplicateGroup(implementationAndDuplicates[0], implementationAndDuplicates));

    private sealed record DuplicateGroup(Step Implementation, IReadOnlyList<Step> ImplementationAndDuplicates);
}