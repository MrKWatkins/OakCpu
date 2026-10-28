using System.Collections;
using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.Definitions;

public abstract class StepSequence : IEnumerable<Step>
{
    private protected StepSequence(
        string? name,
        IReadOnlyList<Step> steps,
        NextOpcodeMode nextOpcode,
        bool executeOverlapOnStart = false,
        string? overlappedSequenceName = null,
        IReadOnlyList<byte>? overrideStepPins = null)
    {
        if (steps.Count == 0)
        {
            throw new ArgumentException("Value cannot be empty.", nameof(steps));
        }

        if (overlappedSequenceName != null && nextOpcode != NextOpcodeMode.Overlapped)
        {
            throw new ArgumentException("An overlapped next sequence can only be specified for overlapped sequences.", nameof(overlappedSequenceName));
        }

        Name = name;
        Steps = steps;
        NextOpcode = nextOpcode;
        ExecuteOverlapOnStart = executeOverlapOnStart;
        OverlappedSequenceName = overlappedSequenceName;
        OverrideStepPins = overrideStepPins;
    }

    public string? Name { get; }

    public Step FirstStep => Steps[0];

    public IReadOnlyList<Step> Steps { get; }

    public NextOpcodeMode NextOpcode { get; }

    public bool ExecuteOverlapOnStart { get; }

    public string? OverlappedSequenceName { get; }

    /// <summary>
    /// Explicit pins for each step, overriding the pins inferred from the actions requested by the sequence's steps, or
    /// <c>null</c> if the sequence does not override them. Needed for machine cycles that are longer than the requested
    /// action's pin pattern, which hold their pins for the extra T-states.
    /// </summary>
    public IReadOnlyList<byte>? OverrideStepPins { get; }

    [Pure]
    public IEnumerator<Step> GetEnumerator() => Steps.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}