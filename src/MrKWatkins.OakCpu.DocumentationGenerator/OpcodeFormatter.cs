using System.Globalization;
using System.Text.RegularExpressions;
using MrKWatkins.OakCpu.Definitions.Language.Ast;

namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Formats the bytes of an opcode, e.g. <c>DD 46 d</c>, naming the operand bytes after the operands in the instruction.
/// </summary>
public sealed class OpcodeFormatter
{
    private readonly IReadOnlyDictionary<string, Operand> operands;
    private readonly Regex operandTokenRegex;
    private readonly IReadOnlyDictionary<string, Instruction> redirects;

    public OpcodeFormatter(CpuModel model)
    {
        operands = model.Cpu.Operands.ToDictionary(operand => operand.Name, StringComparer.Ordinal);

        // Longest names first so that nn is not matched as n.
        var names = model.Cpu.Operands.OrderByDescending(operand => operand.Name.Length).Select(operand => Regex.Escape(operand.Name));
        operandTokenRegex = new Regex($@"\$?\b({string.Join('|', names)})\b", RegexOptions.CultureInvariant);
        redirects = model.Instructions
            .Where(CycleCalculator.IsRedirect)
            .Select(instruction => (Instruction: instruction, Table: RedirectTable(instruction)))
            .Where(x => x.Table != null)
            .ToDictionary(x => x.Table!, x => x.Instruction, StringComparer.Ordinal);
    }

    /// <summary>
    /// Gets the operand bytes named in an instruction, e.g. <c>d</c> and <c>n</c> for <c>LD (IX + d), n</c>, in order. The
    /// operands are those the CPU defines, e.g. <c>n</c>, <c>nn</c> and <c>d</c> for the Z80.
    /// </summary>
    [Pure]
    public IReadOnlyList<string> OperandTokens(string instruction) => operandTokenRegex.Matches(instruction).Select(match => match.Groups[1].Value).ToList();

    /// <summary>
    /// The number of bytes an operand token names.
    /// </summary>
    [Pure]
    public int TokenBytes(string token) => operands[token].Bytes;

    /// <summary>
    /// Formats the encoding of an opcode: its prefix and opcode bytes in hex with the operand bytes, named after the operands
    /// in the instruction, in the positions the instruction reads them.
    /// </summary>
    /// <param name="form">The instruction with placeholders, e.g. <c>LD (RP0 + d), n</c>.</param>
    /// <param name="instruction">The instruction for the opcode, which supplies the operand reads.</param>
    /// <param name="prefix">The opcode's prefix byte, if any. Duplicate opcodes share an instruction but have their own bytes.</param>
    /// <param name="opcode">The opcode byte.</param>
    [Pure]
    public string Format(string form, Instruction instruction, byte? prefix, byte opcode)
    {
        var tokens = OperandTokens(form);
        var expectedBytes = tokens.Sum(TokenBytes);

        var bytes = new List<string>();
        var redirectReads = 0;
        if (instruction.OpcodeTable != null)
        {
            if (!redirects.TryGetValue(instruction.OpcodeTable, out var redirect))
            {
                throw new InvalidOperationException($"No redirect instruction was found for opcode table {instruction.OpcodeTable}.");
            }

            if (redirect.Prefix.HasValue)
            {
                bytes.Add(FormatByte(redirect.Prefix.Value));
            }

            bytes.Add(FormatByte(redirect.Opcode));

            // The redirect reads the opcode byte itself as well as any operands that come before it.
            redirectReads = CountOperandReads(redirect, true) - 1;
        }
        else if (prefix.HasValue)
        {
            bytes.Add(FormatByte(prefix.Value));
        }

        // The 6502 also reads at the program counter without incrementing it, both for dummy reads and for an operand that JSR
        // reads after pushing the return address, and BRK skips a byte it does not name, so the reads only bound the operands.
        var reads = redirectReads + CountOperandReads(instruction, false);
        if (reads < expectedBytes)
        {
            throw new InvalidOperationException($"The instruction {form} names {expectedBytes} operand byte(s) but {instruction.Mnemonic} only reads {reads} from the program counter.");
        }

        var tokenIndex = 0;
        for (var read = 0; read < redirectReads; tokenIndex++)
        {
            bytes.Add(tokens[tokenIndex]);
            read += TokenBytes(tokens[tokenIndex]);
        }

        bytes.Add(FormatByte(opcode));
        bytes.AddRange(tokens.Skip(tokenIndex));

        return string.Join(' ', bytes);
    }

    [Pure]
    private static string FormatByte(byte value) => value.ToString("X2", NumberFormatInfo.InvariantInfo);

    [Pure]
    private static string? RedirectTable(Instruction instruction) =>
        instruction.Steps
            .SelectMany(step => step.Statements)
            .SelectMany(statement => statement.TraverseDepthFirst())
            .OfType<Call>()
            .Where(call => call.Function == PreDefinedFunction.SetOpcodeStepTable)
            .Select(call => (call.Arguments.FirstOrDefault() as OpcodeStepTableAccess)?.OpcodeStepTable.CustomName)
            .FirstOrDefault(table => table != null);

    /// <summary>
    /// Counts the bytes an instruction reads from the program counter, i.e. its operand bytes.
    /// </summary>
    /// <param name="instruction">The instruction.</param>
    /// <param name="consumed">Whether to count only reads that increment the program counter.</param>
    [Pure]
    private static int CountOperandReads(Instruction instruction, bool consumed) =>
        instruction.Steps.Count(step =>
            step.SpecifiedAction?.Name == "memory_read" &&
            step.Statements.Any(statement => statement is Assignment
            {
                Target: DataMemberAccess { DataMember: var member },
                Value: RegisterAccess { Register.ProgramCounter: true }
            } && member == PreDefinedDataMember.Address) &&
            (!consumed || step.Statements.Any(statement => statement is Assignment
            {
                Target: RegisterAccess { Register.ProgramCounter: true },
                Value: BinaryOperation { Left: RegisterAccess { Register.ProgramCounter: true }, Right: Number { Value: 1 } } binary
            } && binary.Operator == Operator.Add)));
}