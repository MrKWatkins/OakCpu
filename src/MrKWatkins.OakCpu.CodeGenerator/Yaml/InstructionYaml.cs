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

    /// <summary>
    /// Explicit pins for each step, one entry per step, overriding the pins inferred from the actions the steps request. Needed for
    /// machine cycles longer than the requested action's pin pattern, which hold their pins for the extra T-states.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<string>>? OverrideStepPins { get; private set; }

    [Pure]
    public NextOpcodeMode GetEffectiveNextOpcode(NextOpcodeMode defaultNextOpcode) => NextOpcode ?? defaultNextOpcode;

    /// <summary>
    /// Creates a copy of this instruction with the specified opcodes. Used when a variant CPU overrides some, but not all, of the
    /// opcodes of an instruction from the CPU it is based on.
    /// </summary>
    [Pure]
    internal InstructionYaml WithOpcodes(IReadOnlyList<OpcodeYaml> newOpcodes) =>
        new()
        {
            Group = Group,
            Mnemonic = Mnemonic,
            OpcodeTable = OpcodeTable,
            Opcodes = newOpcodes,
            Steps = Steps,
            Flags = Flags,
            NextOpcode = NextOpcode,
            OverlappedSequence = OverlappedSequence,
            OverrideStepPins = OverrideStepPins
        };

    public override string ToString() => Mnemonic;
}