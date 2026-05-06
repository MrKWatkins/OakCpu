using System.ComponentModel;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

internal static class StepFinalizer
{
    public static IReadOnlyList<Step> Finalize(IReadOnlyList<Step> steps)
    {
        AssignIndices(steps);

        var functionSteps = MapDuplicates(steps).ToList();
        AssignMethodIndices(functionSteps);
        return functionSteps;
    }

    private static void AssignIndices(IEnumerable<Step> steps)
    {
        var index = 0;
        foreach (var step in steps)
        {
            if (index > ushort.MaxValue)
            {
                throw new InvalidAsynchronousStateException("Too many steps; will need to change to int.");
            }

            step.AssignIndex((ushort)index++);
        }
    }

    private static void AssignMethodIndices(IEnumerable<Step> steps)
    {
        var index = 0;
        foreach (var step in steps)
        {
            if (step.DoesNothing)
            {
                step.AssignMethodIndex(null);
                foreach (var duplicate in step.Duplicates)
                {
                    duplicate.AssignMethodIndex(null);
                }
                continue;
            }

            var methodIndex = (ushort)index++;
            step.AssignMethodIndex(methodIndex);
            foreach (var duplicate in step.Duplicates)
            {
                duplicate.AssignMethodIndex(methodIndex);
            }
        }
    }

    private static IEnumerable<Step> MapDuplicates(IEnumerable<Step> steps)
    {
        foreach (var group in steps.GroupBy(step => step, StepDuplicateEqualityComparer.Instance))
        {
            var step = group.First();
            step.SetImplementation(step);
            foreach (var duplicate in group.Skip(1))
            {
                step.AddDuplicate(duplicate);
                duplicate.SetImplementation(step);
            }

            yield return step;
        }
    }
}