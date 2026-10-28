namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// The effect an instruction has on a flag.
/// </summary>
/// <param name="Flag">The flag.</param>
/// <param name="Kind">How the instruction affects the flag.</param>
/// <param name="Description">
/// A description of the effect. Operand placeholders from the instruction's mnemonic appear in braces, e.g. <c>{R0}</c>.
/// </param>
public sealed record FlagEffect(Flag Flag, FlagEffectKind Kind, string Description)
{
    public static FlagEffect Unaffected(Flag flag) => new(flag, FlagEffectKind.Unaffected, "Unaffected.");
}