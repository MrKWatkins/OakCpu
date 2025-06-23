using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public abstract class StepSequence
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

    public IReadOnlyList<Step> Steps { get; }

    public NextOpcodeMode NextOpcode { get; }
}