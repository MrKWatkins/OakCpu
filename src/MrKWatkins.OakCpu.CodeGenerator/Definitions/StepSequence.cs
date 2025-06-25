using System.Collections;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public abstract class StepSequence : IEnumerable<Step>
{
    private protected StepSequence(IReadOnlyList<Step> steps, NextOpcodeMode nextOpcode)
    {
        if (steps.Count == 0)
        {
            throw new ArgumentException("Value cannot be empty.", nameof(steps));
        }

        Steps = steps;
        NextOpcode = nextOpcode;

        foreach (var step in steps)
        {
            step.Sequence = this;
        }
    }

    public Step FirstStep => Steps[0];

    public IReadOnlyList<Step> Steps { get; }

    public NextOpcodeMode NextOpcode { get; }

    public IEnumerator<Step> GetEnumerator() => Steps.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}