using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class InstructionYaml
{
    private static readonly IReadOnlyDictionary<string, string> EmptyFlags = new Dictionary<string, string>();
    private IReadOnlyList<OpcodeYaml>? opcodes;
    private IReadOnlyList<string?>? steps;
    private IReadOnlyDictionary<string, string>? flags;

    private InstructionYaml()
    {
    }

    public string Group { get; private set; } = null!;

    public string Mnemonic { get; private set; } = null!;

    public string? OpcodeTable { get; private set; }

    public IReadOnlyList<OpcodeYaml> Opcodes
    {
        get => opcodes ?? [];
        private set => opcodes = value;
    }

    public IReadOnlyList<string?> Steps
    {
        get => steps ?? [];
        private set => steps = value;
    }

    public IReadOnlyDictionary<string, string> Flags
    {
        get => flags ?? EmptyFlags;
        private set => flags = value;
    }

    public NextOpcodeMode? NextOpcode { get; private set; }

    public string? OverlappedSequence { get; private set; }

    [Pure]
    public NextOpcodeMode GetEffectiveNextOpcode(NextOpcodeMode defaultNextOpcode) => NextOpcode ?? defaultNextOpcode;

    public override string ToString() => Mnemonic;
}