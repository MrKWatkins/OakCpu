using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.Definitions;

/// <summary>
/// The names of the modes a flag puts the CPU in, e.g. <c>decimal</c> and <c>binary</c> for the 6502's D flag.
/// </summary>
/// <param name="Set">The name of the mode the CPU is in when the flag is set, without the word "mode".</param>
/// <param name="Reset">The name of the mode the CPU is in when the flag is reset, without the word "mode".</param>
public sealed record FlagMode(string Set, string Reset)
{
    [Pure]
    internal static FlagMode? Create(FlagModeYaml? yaml) => yaml == null ? null : new FlagMode(yaml.Set, yaml.Reset);
}