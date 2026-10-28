namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// How an instruction affects a flag.
/// </summary>
public enum FlagEffectKind
{
    /// <summary>
    /// The instruction leaves the flag alone.
    /// </summary>
    Unaffected,

    /// <summary>
    /// The instruction always sets the flag.
    /// </summary>
    Set,

    /// <summary>
    /// The instruction always resets the flag.
    /// </summary>
    Reset,

    /// <summary>
    /// The instruction sets or resets the flag depending on its operation.
    /// </summary>
    Changed
}