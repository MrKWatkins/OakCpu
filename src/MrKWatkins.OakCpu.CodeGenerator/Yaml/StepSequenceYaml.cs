using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class StepSequenceYaml
{
    private IReadOnlyList<string?>? steps;

    private StepSequenceYaml()
    {
    }

    public string Name { get; private set; } = null!;

    public SequenceGroupYaml? Group { get; private set; }

    public bool ExecuteOverlapOnStart { get; private set; }

    public IReadOnlyList<string?> Steps
    {
        get => steps ?? [];
        private set => steps = value;
    }

    /// <summary>
    /// Explicit pins for each step, sampled at the rising edge of the step's clock. Overrides the pins inferred from the
    /// actions requested by the sequence's steps. Missing trailing entries default to no pins.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<string>>? OverrideStepPins { get; private set; }

    public NextOpcodeMode NextOpcode { get; private set; }

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