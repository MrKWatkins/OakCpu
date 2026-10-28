using VYaml.Annotations;

namespace MrKWatkins.OakCpu.Definitions.Yaml;

[YamlObject]
public sealed partial class InstructionYaml
{
    private static readonly IReadOnlyDictionary<string, string> EmptyFlags = new Dictionary<string, string>();

    private InstructionYaml()
    {
    }

    public string Group { get; private init; } = null!;

    /// <summary>
    /// The instruction with placeholders for its operands, e.g. <c>LD R0, (RP0 + d)</c>.
    /// </summary>
    public string Instruction { get; private init; } = null!;

    /// <summary>
    /// A description of what the instruction does for documentation. Placeholders in the instruction, e.g. <c>R0</c>, can be
    /// used in braces, e.g. <c>{R0}</c>, and are replaced with the operand for each opcode.
    /// </summary>
    public string? Documentation { get; private init; }

    /// <summary>
    /// Whether every opcode of the instruction is undocumented, i.e. not in the manufacturer's documentation for the CPU.
    /// Individual opcodes can be marked instead with <see cref="OpcodeYaml.Undocumented" />.
    /// </summary>
    public bool Undocumented { get; private init; }

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

    /// <summary>
    /// Descriptions of the instruction's effect on flags for documentation, by flag, for flags whose <see cref="Flags" />
    /// expressions are too involved to describe automatically.
    /// </summary>
    public IReadOnlyDictionary<string, string> FlagDocumentation
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
            Instruction = Instruction,
            Documentation = Documentation,
            Undocumented = Undocumented,
            OpcodeTable = OpcodeTable,
            Opcodes = newOpcodes,
            Steps = Steps,
            Flags = Flags,
            FlagDocumentation = FlagDocumentation,
            NextOpcode = NextOpcode,
            OverlappedSequence = OverlappedSequence,
            OverrideStepPins = OverrideStepPins
        };

    public override string ToString() => Instruction;
}