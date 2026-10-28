using System.Globalization;
using System.Text;

namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Writes the contents pages for a CPU's instruction documentation: a list of the instructions by group, and the opcodes laid
/// out in a table per opcode table, with every entry linking to the instruction's page.
/// </summary>
public static class ContentsWriter
{
    /// <summary>
    /// Writes the instructions as a list, grouped by their groups.
    /// </summary>
    [Pure]
    public static string WriteIndex(Cpu cpu, IReadOnlyList<InstructionDocumentation> instructions)
    {
        var markdown = new StringBuilder();
        markdown.Append("# ").Append(cpu.DisplayName).Append(" Instructions\n");
        markdown.Append('\n');
        markdown.Append("The instructions by group. See [the opcode tables](").Append(DocumentationGenerator.OpcodesFileName).Append(") for the instructions by opcode.\n");

        foreach (var group in instructions.GroupBy(instruction => instruction.Group, StringComparer.Ordinal).OrderBy(group => group.Key, StringComparer.Ordinal))
        {
            markdown.Append('\n');
            markdown.Append("## ").Append(group.Key).Append('\n');
            markdown.Append('\n');
            foreach (var instruction in group.OrderBy(instruction => instruction.Instruction, StringComparer.Ordinal))
            {
                markdown.Append("- ").Append(Link(instruction.Symbolic, instruction.Page, instruction.Undocumented))
                    .Append(": ").Append(Operands.Substitute(instruction.Documentation, instruction.SymbolicOperands)).Append('\n');
            }
        }

        return markdown.ToString();
    }

    /// <summary>
    /// Writes the opcodes as a table per opcode table, i.e. one for the unprefixed opcodes and one for each prefix, with the
    /// high nibble of the opcode byte selecting the row and the low nibble the column. A CPU without prefixes, such as the 6502,
    /// gets a single table without a heading.
    /// </summary>
    [Pure]
    public static string WriteOpcodeTables(Cpu cpu, IReadOnlyList<InstructionDocumentation> instructions)
    {
        var markdown = new StringBuilder();
        markdown.Append("# ").Append(cpu.DisplayName).Append(" Opcodes\n");
        markdown.Append('\n');
        markdown.Append("The instructions by opcode, with the high nibble of the opcode byte down the side and the low nibble along the top. Undocumented opcodes are in italics. See [the instructions](")
            .Append(DocumentationGenerator.IndexFileName).Append(") for the instructions by group.\n");

        var opcodes = instructions
            .SelectMany(instruction => instruction.Opcodes.Select(opcode => (Instruction: instruction, Opcode: opcode)))
            .ToList();

        var duplicates = opcodes.GroupBy(x => (x.Opcode.Prefix, x.Opcode.OpcodeByte)).Where(group => group.Count() > 1).Select(group => group.Key).ToList();
        if (duplicates.Count > 0)
        {
            throw new InvalidOperationException($"Opcodes are documented more than once: {string.Join(", ", duplicates.Select(key => $"{key.Prefix} {key.OpcodeByte:X2}".Trim()))}.");
        }

        var prefixes = opcodes.Select(x => x.Opcode.Prefix).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToList();

        foreach (var table in opcodes.GroupBy(x => x.Opcode.Prefix, StringComparer.Ordinal).OrderBy(table => table.Key, StringComparer.Ordinal))
        {
            var cells = table.ToDictionary(x => x.Opcode.OpcodeByte, x => x);

            markdown.Append('\n');
            if (prefixes.Count > 1)
            {
                markdown.Append("## ").Append(PrefixTitle(table.Key)).Append('\n');
                markdown.Append('\n');
            }

            markdown.Append("| |");
            for (var low = 0; low < 16; low++)
            {
                markdown.Append(" x").Append(low.ToString("X1", NumberFormatInfo.InvariantInfo)).Append(" |");
            }
            markdown.Append('\n');
            markdown.Append("| - |").Append(string.Concat(Enumerable.Repeat(" - |", 16))).Append('\n');

            for (var high = 0; high < 16; high++)
            {
                markdown.Append("| **").Append(high.ToString("X1", NumberFormatInfo.InvariantInfo)).Append("x** |");
                for (var low = 0; low < 16; low++)
                {
                    markdown.Append(' ');
                    var opcodeByte = (byte)(high << 4 | low);
                    if (cells.TryGetValue(opcodeByte, out var cell))
                    {
                        markdown.Append(Link(cell.Opcode.Instruction, cell.Instruction.Page, cell.Opcode.Undocumented));
                    }
                    else if (PrefixTable(prefixes, table.Key, opcodeByte) is { } prefix)
                    {
                        markdown.Append(PrefixLink(prefix));
                    }
                    markdown.Append(" |");
                }
                markdown.Append('\n');
            }
        }

        return markdown.ToString();
    }

    /// <summary>
    /// Gets the opcode table a byte in a table switches to, if it is a prefix byte: a longer prefix built on the table's own,
    /// e.g. <c>CB</c> in the <c>DD</c> table, or another prefix the CPU restarts with, e.g. <c>ED</c> in the <c>DD</c> table.
    /// </summary>
    [Pure]
    private static string? PrefixTable(IReadOnlyList<string> prefixes, string prefix, byte opcodeByte)
    {
        var hex = opcodeByte.ToString("X2", NumberFormatInfo.InvariantInfo);
        var longer = prefix.Length == 0 ? hex : $"{prefix} {hex}";
        return prefixes.FirstOrDefault(p => p == longer) ?? prefixes.FirstOrDefault(p => p == hex);
    }

    [Pure]
    private static string PrefixLink(string prefix) => $"[{PrefixTitle(prefix)}](#{CpuDocumentation.Slug(PrefixTitle(prefix))})";

    [Pure]
    private static string PrefixTitle(string prefix) => prefix.Length == 0 ? "No prefix" : $"{prefix} prefix";

    [Pure]
    private static string Link(string text, string page, bool undocumented)
    {
        var link = $"[{text.Replace("|", "\\|", StringComparison.Ordinal)}]({page})";
        return undocumented ? $"*{link}*" : link;
    }
}