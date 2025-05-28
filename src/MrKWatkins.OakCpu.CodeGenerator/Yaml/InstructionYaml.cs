using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class InstructionYaml
{
    private IReadOnlyList<OpcodeYaml>? opcodes;
    private IReadOnlyList<IReadOnlyList<string>>? steps;

    private InstructionYaml()
    {
    }

    public string Group { get; private set; } = null!;

    public string Mnemonic { get; private set; } = null!;

    public IReadOnlyList<OpcodeYaml> Opcodes
    {
        get => opcodes ?? [];
        private set => opcodes = value;
    }

    public IReadOnlyList<IReadOnlyList<string>> Steps
    {
        get => steps ?? [];
        private set => steps = value;
    }

    public NextOpcodeMode NextOpcode { get; private set; }

    public override string ToString() => Mnemonic;
}