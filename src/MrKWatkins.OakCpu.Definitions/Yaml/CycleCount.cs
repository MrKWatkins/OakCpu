namespace MrKWatkins.OakCpu.Definitions.Yaml;

/// <summary>
/// How the steps of a sequence relate to the CPU's clock cycles.
/// </summary>
public enum CycleCount
{
    /// <summary>
    /// Every step is a clock cycle, e.g. a T-state on the Z80, whether or not it requests an action.
    /// </summary>
    Steps,

    /// <summary>
    /// Only steps that request an action are clock cycles; the rest happen within the cycle of the action before them. The
    /// 6502 accesses the bus on every cycle, so its steps that only compute take no time.
    /// </summary>
    Actions
}