namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Which instructions a <see cref="Reference" /> covers.
/// </summary>
public enum ReferenceCoverage
{
    /// <summary>
    /// Only the documented instructions, e.g. the manufacturer's manual.
    /// </summary>
    Documented,

    /// <summary>
    /// Only the undocumented instructions.
    /// </summary>
    Undocumented,

    /// <summary>
    /// Every instruction, documented or not.
    /// </summary>
    All
}