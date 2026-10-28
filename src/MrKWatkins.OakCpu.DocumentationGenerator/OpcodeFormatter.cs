using System.Globalization;
using System.Text.RegularExpressions;
using MrKWatkins.OakCpu.Definitions.Language.Ast;

namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Formats the bytes of an opcode, e.g. <c>DD 46 d</c>, naming the operand bytes after the operands in the instruction.
/// </summary>
public sealed partial class OpcodeFormatter
{
    private readonly IReadOnlyDictionary<string, Instruction> redirects;

    public OpcodeFormatter(CpuModel model)
    {
        redirects = model.Instructions
            .Where(TStateCalculator.IsRedirect)
            .Select(instruction => (Instruction: instruction, Table: RedirectTable(instruction)))
            .Where(x => x.Table != null)
            .ToDictionary(x => x.Table!, x => x.Instruction, StringComparer.Ordinal);
    }

    [GeneratedRegex(@"\$?\b(nn|n|d)\b")]
    private static partial Regex OperandTokenRegex();

    /// <summary>
    /// Gets the operand bytes named in a instruction, e.g. <c>d</c> and <c>n</c> for <c>LD (IX + d), n</c>, in order.
    /// </summary>
    [Pure]
    public static IReadOnlyList<string> OperandTokens(string instruction) => OperandTokenRegex().Matches(instruction).Select(match => match.Groups[1].Value).ToList();

    /// <summary>
    /// Formats the encoding of an opcode: its prefix and opcode bytes in hex with the operand bytes, named after the operands
    /// in the instruction, in the positions the instruction reads them.
    /// </summary>
    /// <param name="form">The instruction's instruction with placeholders, e.g. <c>LD (RP0 + d), n</c>.</param>
    /// <param name="instruction">The instruction for the opcode, which supplies the operand reads.</param>
    /// <param name="prefix">The opcode's prefix byte, if any. Duplicate opcodes share an instruction but have their own bytes.</param>
    /// <param name="opcode">The opcode byte.</param>
    [Pure]
    public string Format(string form, Instruction instruction, byte? prefix, byte opcode)
    {
        var tokens = OperandTokens(form);
        var expectedBytes = tokens.Sum(token => token == "nn" ? 2 : 1);

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
            redirectReads = CountOperandReads(redirect) - 1;
        }
        else if (prefix.HasValue)
        {
            bytes.Add(FormatByte(prefix.Value));
        }

        var instructionReads = CountOperandReads(instruction);
        if (redirectReads + instructionReads != expectedBytes)
        {
            throw new InvalidOperationException($"The instruction {form} names {expectedBytes} operand byte(s) but the instruction {instruction.Mnemonic} reads {redirectReads + instructionReads}.");
        }

        var tokenIndex = 0;
        for (var read = 0; read < redirectReads; tokenIndex++)
        {
            bytes.Add(tokens[tokenIndex]);
            read += tokens[tokenIndex] == "nn" ? 2 : 1;
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
    [Pure]
    private static int CountOperandReads(Instruction instruction) =>
        instruction.Steps.Count(step =>
            step.SpecifiedAction?.Name == "memory_read" &&
            step.Statements.Any(statement => statement is Assignment
            {
                Target: DataMemberAccess { DataMember: var member },
                Value: RegisterAccess { Register.ProgramCounter: true }
            } && member == PreDefinedDataMember.Address));
}