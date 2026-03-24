using System.Collections;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public abstract class StepSequence : IEnumerable<Step>
{
    private protected StepSequence(string? name, IReadOnlyList<Step> steps, NextOpcodeMode nextOpcode, bool executeOverlapOnStart = false, string? overlappedSequenceName = null)
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

        foreach (var step in steps)
        {
            step.Sequence = this;
        }
    }

    public string? Name { get; }

    public Step FirstStep => Steps[0];

    public IReadOnlyList<Step> Steps { get; }

    public NextOpcodeMode NextOpcode { get; }

    public bool ExecuteOverlapOnStart { get; }

    public string? OverlappedSequenceName { get; }

    public IEnumerator<Step> GetEnumerator() => Steps.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}