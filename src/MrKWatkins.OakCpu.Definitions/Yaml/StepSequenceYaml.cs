using VYaml.Annotations;

namespace MrKWatkins.OakCpu.Definitions.Yaml;

[YamlObject]
public sealed partial class StepSequenceYaml
{
    private StepSequenceYaml()
    {
    }

    public string Name { get; private init; } = null!;

    public SequenceGroupYaml? Group { get; private init; }

    public bool ExecuteOverlapOnStart { get; private init; }

    public IReadOnlyList<string?> Steps
    {
        get => field ?? [];
        private init;
    }

    /// <summary>
    /// Explicit pins for each step, sampled at the rising edge of the step's clock. Overrides the pins inferred from the
    /// actions requested by the sequence's steps. Missing trailing entries default to no pins.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<string>>? OverrideStepPins { get; private init; }

    public NextOpcodeMode NextOpcode { get; private init; }

    [Pure]
    public static StepSequenceYaml Create(string name, IReadOnlyList<string?> steps, NextOpcodeMode nextOpcode, bool executeOverlapOnStart = false, SequenceGroupYaml? group = null) =>
        new()
        {
            Name = name,
            Group = group,
            ExecuteOverlapOnStart = executeOverlapOnStart,
            Steps = steps,
            NextOpcode = nextOpcode
        };
}