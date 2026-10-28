using VYaml.Annotations;

namespace MrKWatkins.OakCpu.Definitions.Yaml;

/// <summary>
/// How a CPU's clock cycles are counted and named.
/// </summary>
[YamlObject]
public sealed partial class CyclesYaml
{
    private CyclesYaml()
    {
    }

    /// <summary>
    /// What the cycles are called, e.g. <c>T-States</c> on the Z80.
    /// </summary>
    public string? Name { get; private init; }

    [YamlIgnore]
    public string EffectiveName => Name ?? "Cycles";

    /// <summary>
    /// Which steps take a cycle.
    /// </summary>
    public CycleCount Count { get; private init; }

    /// <summary>
    /// The fewest cycles any instruction takes, including its opcode fetch. The 6502 always reads the byte after an opcode,
    /// so an instruction with no steps of its own still takes two.
    /// </summary>
    public int? Minimum { get; private init; }

    [YamlIgnore]
    public int EffectiveMinimum => Minimum ?? 1;

    public override string ToString() => EffectiveName;
}