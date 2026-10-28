using VYaml.Annotations;

namespace MrKWatkins.OakCpu.Definitions.Yaml;

[YamlObject]
public sealed partial class CpuYaml
{
    private static readonly IReadOnlyDictionary<string, string> EmptyTemporaries = new Dictionary<string, string>();

    private CpuYaml()
    {
    }

    public string Name { get; private init; } = null!;

    /// <summary>
    /// The name of the definitions directory for the CPU this CPU is a variant of, if any. Definitions from the base CPU are inherited;
    /// this CPU's definitions are additions to, or overrides of, them.
    /// </summary>
    public string? Base { get; private init; }

    /// <summary>
    /// The name of the CPU for people, e.g. <c>6502</c> for <c>M6502</c>, whose name must be a valid identifier. Defaults to
    /// <see cref="Name" />.
    /// </summary>
    public string? DisplayName { get; private init; }

    [YamlIgnore]
    public string EffectiveDisplayName => DisplayName ?? Name;

    public string? Documentation { get; private init; }

    /// <summary>
    /// How the CPU's clock cycles are counted and named. Defaults to every step being a cycle.
    /// </summary>
    public CyclesYaml? Cycles { get; private init; }

    /// <summary>
    /// The operands that follow an opcode in memory, as they are named in instructions.
    /// </summary>
    public IReadOnlyList<OperandYaml> Operands
    {
        get => field ?? [];
        private init;
    }

    /// <summary>
    /// Links to external documentation for the CPU's instructions.
    /// </summary>
    public IReadOnlyList<ReferenceYaml> References
    {
        get => field ?? [];
        private init;
    }

    /// <summary>
    /// How the temporaries the instruction steps use are named in documentation, keyed by the temporary's name without its
    /// <c>$</c>, e.g. <c>result</c> = <c>the result</c>. Temporaries that are not named are described by what they were
    /// assigned.
    /// </summary>
    public IReadOnlyDictionary<string, string> Temporaries
    {
        get => field ?? EmptyTemporaries;
        private init;
    }

    /// <summary>
    /// The CPU this CPU is a variant of, if any. Only set by <see cref="Merge" />.
    /// </summary>
    [YamlIgnore]
    public CpuYaml? BaseCpu { get; private init; }

    public IReadOnlyList<ActionYaml> Actions
    {
        get => field ?? [];
        private init;
    }

    public IReadOnlyList<PinYaml> Pins
    {
        get => field ?? [];
        private init;
    }

    public IReadOnlyList<FieldYaml> Fields
    {
        get => field ?? [];
        private init;
    }

    public IReadOnlyList<string?> OpcodeRead
    {
        get => field ?? [];
        private init;
    }

    public NextOpcodeMode? DefaultNextOpcode { get; private init; }

    [YamlIgnore]
    public NextOpcodeMode EffectiveDefaultNextOpcode => DefaultNextOpcode ?? NextOpcodeMode.Read;

    /// <summary>
    /// Merges a variant CPU into the CPU it is based on. Pins, actions and fields from the variant are appended to those from the base
    /// so that the values generated for the base CPU's members are unchanged. The variant keeps its own name and display name, and
    /// inherits the cycles, operands and references it does not define, and the temporaries it does not rename.
    /// </summary>
    [Pure]
    internal static CpuYaml Merge(CpuYaml @base, CpuYaml variant) =>
        new()
        {
            Name = variant.Name,
            DisplayName = variant.DisplayName,
            Base = @base.Base,
            Documentation = variant.Documentation ?? @base.Documentation,
            Cycles = variant.Cycles ?? @base.Cycles,
            Operands = variant.Operands.Count > 0 ? variant.Operands : @base.Operands,
            References = variant.References.Count > 0 ? variant.References : @base.References,
            Temporaries = @base.Temporaries.Concat(variant.Temporaries).GroupBy(kvp => kvp.Key, StringComparer.Ordinal).ToDictionary(group => group.Key, group => group.Last().Value, StringComparer.Ordinal),
            BaseCpu = @base,
            Actions = [.. @base.Actions, .. variant.Actions],
            Pins = [.. @base.Pins, .. variant.Pins],
            Fields = [.. @base.Fields, .. variant.Fields],
            OpcodeRead = variant.OpcodeRead.Count > 0 ? variant.OpcodeRead : @base.OpcodeRead,
            DefaultNextOpcode = variant.DefaultNextOpcode ?? @base.DefaultNextOpcode
        };

    public override string ToString() => Name;
}