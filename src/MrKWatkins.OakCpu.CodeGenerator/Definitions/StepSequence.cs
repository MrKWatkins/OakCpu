using System.Collections;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public abstract class StepSequence : IEnumerable<Step>
{
    private protected StepSequence(string? name, IReadOnlyList<Step> steps, NextOpcodeMode nextOpcode, bool executeOverlapOnStart = false)
    {
        if (steps.Count == 0)
        {
            throw new ArgumentException("Value cannot be empty.", nameof(steps));
        }

        Name = name;
        Steps = steps;
        NextOpcode = nextOpcode;
        ExecuteOverlapOnStart = executeOverlapOnStart;

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

    public IEnumerator<Step> GetEnumerator() => Steps.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
