namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Documentation for one opcode of an instruction.
/// </summary>
/// <param name="Opcode">
/// The bytes of the opcode, e.g. <c>DD 46 d</c>, with the operand bytes named after the operands in the instruction.
/// </param>
/// <param name="Instruction">The instruction with the operands for the opcode, e.g. <c>LD B, (IX + d)</c>.</param>
/// <param name="Operands">The operands for the opcode's placeholders in the instruction, e.g. <c>R0</c> = <c>B</c>.</param>
/// <param name="TStates">
/// The T-states the opcode takes for each of the paths through it, shortest first. Conditional and repeating instructions have
/// one entry for when they finish early and one for when they do not; everything else has a single entry.
/// </param>
/// <param name="Undocumented">Whether the opcode is undocumented, i.e. not in the manufacturer's documentation for the CPU.</param>
public sealed record OpcodeDocumentation(
    string Opcode,
    string Instruction,
    IReadOnlyList<KeyValuePair<string, string>> Operands,
    IReadOnlyList<int> TStates,
    bool Undocumented)
{
    /// <summary>
    /// The prefix bytes of the opcode, e.g. <c>DD CB</c>, or an empty string if it has none. Opcodes with the same prefix share
    /// an opcode table.
    /// </summary>
    public string Prefix => string.Join(' ', HexBytes[..^1]);

    /// <summary>
    /// The opcode byte itself, i.e. the byte that selects the instruction within its opcode table.
    /// </summary>
    public byte OpcodeByte => Convert.ToByte(HexBytes[^1], 16);

    private string[] HexBytes => Opcode.Split(' ').Where(token => token.Length == 2 && token.All(Uri.IsHexDigit) && token.ToUpperInvariant() == token).ToArray();
}