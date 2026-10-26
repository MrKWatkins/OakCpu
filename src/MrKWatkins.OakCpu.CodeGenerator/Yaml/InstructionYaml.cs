using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class InstructionYaml
{
    private static readonly IReadOnlyDictionary<string, string> EmptyFlags = new Dictionary<string, string>();

    private InstructionYaml()
    {
    }

    public string Group { get; private init; } = null!;

    public string Mnemonic { get; private init; } = null!;

    public string? OpcodeTable { get; private init; }

    public IReadOnlyList<OpcodeYaml> Opcodes
    {
        get => field ?? [];
        private init;
    }

    public IReadOnlyList<string?> Steps
    {
        get => field ?? [];
        private init;
    }

    public IReadOnlyDictionary<string, string> Flags
    {
        get => field ?? EmptyFlags;
        private init;
    }

    public NextOpcodeMode? NextOpcode { get; private init; }

    public string? OverlappedSequence { get; private init; }

    /// <summary>
    /// Explicit pins for each step, one entry per step, overriding the pins inferred from the actions the steps request. Needed for
    /// machine cycles longer than the requested action's pin pattern, which hold their pins for the extra T-states.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<string>>? OverrideStepPins { get; private init; }

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