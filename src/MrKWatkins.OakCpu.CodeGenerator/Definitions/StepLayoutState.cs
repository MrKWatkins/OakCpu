namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

/// <summary>
/// Holds the finalized step layout data built for a generator context.
/// </summary>
internal sealed record StepLayoutState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StepLayoutState" /> record.
    /// </summary>
    /// <param name="layouts">The finalized layout for each parsed step.</param>
    /// <param name="functionSteps">The unique implementation steps that require generated handler methods.</param>
    internal StepLayoutState(IReadOnlyDictionary<Step, StepLayout> layouts, IReadOnlyList<Step> functionSteps)
    {
        Layouts = layouts;
        FunctionSteps = functionSteps;
    }

    /// <summary>
    /// Gets the finalized layout for each parsed step.
    /// </summary>
    internal IReadOnlyDictionary<Step, StepLayout> Layouts { get; }

    /// <summary>
    /// Gets the unique implementation steps that require generated handler methods.
    /// </summary>
    internal IReadOnlyList<Step> FunctionSteps { get; }
}