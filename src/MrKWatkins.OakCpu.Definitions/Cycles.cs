using System.Globalization;
using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.Definitions;

/// <summary>
/// How a CPU's clock cycles are counted and named.
/// </summary>
/// <param name="Name">What the cycles are called, e.g. <c>T-States</c> on the Z80.</param>
/// <param name="Count">Which steps take a cycle.</param>
/// <param name="Minimum">The fewest cycles any instruction takes, including its opcode fetch.</param>
public sealed record Cycles(string Name, CycleCount Count, int Minimum)
{
    /// <summary>
    /// The name as a key for data files, e.g. <c>t_states</c>.
    /// </summary>
    public string Key => Name.ToLower(CultureInfo.InvariantCulture).Replace('-', '_').Replace(' ', '_');

    [Pure]
    internal static Cycles Create(CyclesYaml? yaml) => yaml == null ? new Cycles("Cycles", CycleCount.Steps, 1) : new Cycles(yaml.EffectiveName, yaml.Count, yaml.EffectiveMinimum);
}