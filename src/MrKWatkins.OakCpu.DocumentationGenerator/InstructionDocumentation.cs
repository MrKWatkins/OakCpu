namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Documentation for an instruction, i.e. one form of a mnemonic such as <c>LD R0, (RP0 + d)</c> covering every opcode that form
/// has.
/// </summary>
/// <param name="Cpu">The name of the CPU the documentation is for.</param>
/// <param name="DefinedBy">The name of the CPU that defines the instruction; a base CPU if the instruction is inherited.</param>
/// <param name="Form">The instruction's mnemonic with its operand placeholders, e.g. <c>LD R0, (RP0 + d)</c>.</param>
/// <param name="Mnemonic">The instruction's mnemonic with symbolic operands, e.g. <c>LD r, (rr + d)</c>.</param>
/// <param name="Group">The group of instructions the instruction belongs to.</param>
/// <param name="Description">
/// A description of what the instruction does. Operand placeholders appear in braces, e.g. <c>{R0}</c>, and are replaced with
/// the operand for an opcode by <see cref="Operands.Substitute" />.
/// </param>
/// <param name="Undocumented">Whether every opcode of the instruction is undocumented.</param>
/// <param name="Flags">The effect of the instruction on each of the CPU's flags, most significant first.</param>
/// <param name="Opcodes">The opcodes for the instruction.</param>
/// <param name="References">Links to external documentation for the instruction.</param>
/// <param name="Path">The relative path of the documentation file, e.g. <c>8-bit-load/ld-r0-rp0-d.md</c>.</param>
/// <param name="FlagFallbacks">
/// The flag expressions that could not be described in prose and so appear as expressions in <paramref name="Flags" />.
/// </param>
public sealed record InstructionDocumentation(
    string Cpu,
    string DefinedBy,
    string Form,
    string Mnemonic,
    string Group,
    string Description,
    bool Undocumented,
    IReadOnlyList<FlagEffect> Flags,
    IReadOnlyList<OpcodeDocumentation> Opcodes,
    IReadOnlyList<Reference> References,
    string Path,
    IReadOnlyList<string> FlagFallbacks);