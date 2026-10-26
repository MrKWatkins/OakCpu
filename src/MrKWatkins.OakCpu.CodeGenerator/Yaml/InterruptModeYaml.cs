using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class InterruptModeYaml
{
    private InterruptModeYaml()
    {
    }

    public byte Number { get; private init; }

    public string? Sequence { get; private init; }

    public IReadOnlyList<string?> Steps
    {
        get => field ?? [];
        private init;
    }

    public NextOpcodeMode NextOpcode { get; private init; }
}