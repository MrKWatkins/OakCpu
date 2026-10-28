using System.Text.RegularExpressions;
using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// The operand placeholders used in instructions, e.g. <c>R0</c>, and their substitution in documentation.
/// </summary>
public static partial class Operands
{
    /// <summary>
    /// The placeholders, longest first so that <c>RP0</c> is not matched as <c>R</c> followed by <c>P0</c>.
    /// </summary>
    public static IReadOnlyList<string> Placeholders { get; } = ["RP0", "RP1", "R0", "R1", "C0", "N0"];

    [GeneratedRegex(@"\{(RP0|RP1|R0|R1|C0|N0)\}")]
    private static partial Regex PlaceholderRegex();

    [GeneratedRegex(@"\b(RP0|RP1|R0|R1|C0|N0)\b")]
    private static partial Regex InstructionPlaceholderRegex();

    /// <summary>
    /// Gets the operands for an opcode, i.e. the value of each placeholder the opcode specifies, in the order the placeholders
    /// appear in the instruction.
    /// </summary>
    /// <param name="instruction">The instruction with placeholders.</param>
    /// <param name="opcode">The opcode.</param>
    /// <param name="hexNumbers">Whether to format <c>N0</c> in hex, e.g. for the page zero addresses of <c>RST</c>, rather than decimal.</param>
    [Pure]
    public static IReadOnlyList<KeyValuePair<string, string>> For(string instruction, OpcodeYaml opcode, bool hexNumbers = false) =>
        PlaceholdersIn(instruction)
            .Select(placeholder => new KeyValuePair<string, string>(placeholder, Get(opcode, placeholder, hexNumbers) ?? throw new InvalidOperationException($"The opcode {opcode} for {instruction} does not specify {placeholder}.")))
            .ToList();

    /// <summary>
    /// Gets the placeholders in a instruction, in order.
    /// </summary>
    [Pure]
    public static IReadOnlyList<string> PlaceholdersIn(string instruction) =>
        InstructionPlaceholderRegex().Matches(instruction).Select(match => match.Value).Distinct(StringComparer.Ordinal).ToList();

    /// <summary>
    /// Gets the placeholders used in braces in documentation text, in order.
    /// </summary>
    [Pure]
    public static IReadOnlyList<string> PlaceholdersInDocumentation(string documentation) =>
        PlaceholderRegex().Matches(documentation).Select(match => match.Groups[1].Value).Distinct(StringComparer.Ordinal).ToList();

    /// <summary>
    /// Gets the value an opcode specifies for a placeholder, or <c>null</c> if it does not specify it.
    /// </summary>
    [Pure]
    public static string? Get(OpcodeYaml opcode, string placeholder, bool hexNumbers = false) =>
        placeholder switch
        {
            "R0" => opcode.R0,
            "R1" => opcode.R1,
            "RP0" => opcode.RP0,
            "RP1" => opcode.RP1,
            "C0" => opcode.C0,
            "N0" => opcode.N0 is { } n0 ? hexNumbers ? $"0x{n0:X2}" : n0.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) : null,
            _ => throw new ArgumentException($"Unknown placeholder {placeholder}.", nameof(placeholder))
        };

    /// <summary>
    /// Replaces the placeholders in braces in documentation text, e.g. <c>{R0}</c>, with the given operands.
    /// </summary>
    [Pure]
    public static string Substitute(string documentation, IReadOnlyList<KeyValuePair<string, string>> operands)
    {
        var lookup = operands.ToDictionary(o => o.Key, o => o.Value, StringComparer.Ordinal);
        return PlaceholderRegex().Replace(
            documentation,
            match => lookup.TryGetValue(match.Groups[1].Value, out var value)
                ? value
                : throw new InvalidOperationException($"No operand for placeholder {match.Value} in \"{documentation}\"."));
    }

    /// <summary>
    /// Replaces the placeholders in a instruction with the given operands.
    /// </summary>
    [Pure]
    public static string SubstituteInstruction(string instruction, IReadOnlyList<KeyValuePair<string, string>> operands)
    {
        var lookup = operands.ToDictionary(o => o.Key, o => o.Value, StringComparer.Ordinal);
        return InstructionPlaceholderRegex().Replace(instruction, match => lookup.TryGetValue(match.Value, out var value) ? value : match.Value);
    }

    /// <summary>
    /// Gets the symbolic operands for the placeholders in a instruction, e.g. <c>r</c> for <c>R0</c>, in the style of the Zilog
    /// manual. <c>N0</c> is a bit number, <c>b</c>, unless the opcodes use values that cannot be bit numbers, in which case
    /// it is a page zero address, <c>p</c>.
    /// </summary>
    [Pure]
    public static IReadOnlyList<KeyValuePair<string, string>> Symbolic(string instruction, IReadOnlyList<OpcodeYaml> opcodes) =>
        PlaceholdersIn(instruction)
            .Select(placeholder => new KeyValuePair<string, string>(
                placeholder,
                placeholder switch
                {
                    "R0" => "r",
                    "R1" => "r'",
                    "RP0" => "rr",
                    "RP1" => "rr'",
                    "C0" => "cc",
                    "N0" => opcodes.All(o => o.N0 is null or < 8) ? "b" : "p",
                    _ => throw new InvalidOperationException($"Unknown placeholder {placeholder}.")
                }))
            .ToList();
}