using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class InterruptModeYaml
{
    private IReadOnlyList<string?>? steps;

    private InterruptModeYaml()
    {
    }

    public byte Number { get; private set; }

    public string? Sequence { get; private set; }

    public IReadOnlyList<string?> Steps
    {
        get => steps ?? [];
        private set => steps = value;
    }

    public NextOpcodeMode NextOpcode { get; private set; }
}
