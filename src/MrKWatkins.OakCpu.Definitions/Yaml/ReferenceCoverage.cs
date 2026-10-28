namespace MrKWatkins.OakCpu.Definitions.Yaml;

/// <summary>
/// Which instructions a reference covers.
/// </summary>
public enum ReferenceCoverage
{
    /// <summary>
    /// Every instruction, documented or not.
    /// </summary>
    All,

    /// <summary>
    /// Only the documented instructions, e.g. the manufacturer's manual.
    /// </summary>
    Documented,

    /// <summary>
    /// Only the undocumented instructions.
    /// </summary>
    Undocumented
}