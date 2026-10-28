namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Documentation for one opcode of an instruction.
/// </summary>
/// <param name="Encoding">
/// The bytes of the opcode, e.g. <c>DD 46 d</c>, with the operand bytes named after the operands in the mnemonic.
/// </param>
/// <param name="Mnemonic">The mnemonic with the operands for the opcode, e.g. <c>LD B, (IX + d)</c>.</param>
/// <param name="Operands">The operands for the opcode's placeholders in the instruction's mnemonic, e.g. <c>R0</c> = <c>B</c>.</param>
/// <param name="TStates">
/// The T-states the opcode takes for each of the paths through it, shortest first. Conditional and repeating instructions have
/// one entry for when they finish early and one for when they do not; everything else has a single entry.
/// </param>
/// <param name="Undocumented">Whether the opcode is undocumented, i.e. not in the manufacturer's documentation for the CPU.</param>
public sealed record OpcodeDocumentation(
    string Encoding,
    string Mnemonic,
    IReadOnlyList<KeyValuePair<string, string>> Operands,
    IReadOnlyList<int> TStates,
    bool Undocumented);