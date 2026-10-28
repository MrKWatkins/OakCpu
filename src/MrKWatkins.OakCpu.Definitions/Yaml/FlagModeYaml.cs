using VYaml.Annotations;

namespace MrKWatkins.OakCpu.Definitions.Yaml;

/// <summary>
/// The names of the modes a flag puts the CPU in, e.g. <c>decimal</c> and <c>binary</c> for the 6502's D flag, so that
/// documentation can say "in decimal mode" rather than "when the D flag is set".
/// </summary>
[YamlObject]
public sealed partial class FlagModeYaml
{
    private FlagModeYaml()
    {
    }

    /// <summary>
    /// The name of the mode the CPU is in when the flag is set, without the word "mode".
    /// </summary>
    public string Set { get; private init; } = null!;

    /// <summary>
    /// The name of the mode the CPU is in when the flag is reset, without the word "mode".
    /// </summary>
    public string Reset { get; private init; } = null!;

    public override string ToString() => $"{Set}/{Reset}";
}