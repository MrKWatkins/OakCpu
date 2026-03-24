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