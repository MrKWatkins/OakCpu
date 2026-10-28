using System.Globalization;
using System.Text;

namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Writes the documentation for an instruction as a Markdown page for people to read. The same information is written as data
/// for tools by <see cref="YamlWriter" />.
/// </summary>
public static class MarkdownWriter
{
    [Pure]
    public static string Write(InstructionDocumentation documentation)
    {
        var markdown = new StringBuilder();

        markdown.Append("# ").Append(documentation.Symbolic).Append('\n');
        markdown.Append('\n');
        markdown.Append(Operands.Substitute(documentation.Documentation, documentation.SymbolicOperands)).Append('\n');

        if (documentation.Undocumented)
        {
            markdown.Append('\n');
            markdown.Append("This instruction is undocumented.\n");
        }

        markdown.Append('\n');
        markdown.Append("## Flags\n");
        markdown.Append('\n');
        markdown.Append("| Flag | Effect |\n");
        markdown.Append("| ---- | ------ |\n");
        foreach (var flag in documentation.Flags)
        {
            markdown.Append("| ").Append(flag.Flag.Name).Append(" | ").Append(Operands.Substitute(flag.Documentation, documentation.SymbolicOperands)).Append(" |\n");
        }

        markdown.Append('\n');
        markdown.Append("## Opcodes\n");
        markdown.Append('\n');
        markdown.Append("| Instruction | Opcode | T-States | Notes |\n");
        markdown.Append("| ----------- | ------ | -------- | ----- |\n");
        foreach (var opcode in documentation.Opcodes)
        {
            markdown.Append("| `").Append(opcode.Instruction).Append("` | `").Append(opcode.Opcode).Append("` | ")
                .Append(string.Join(" or ", opcode.TStates.Select(t => t.ToString(NumberFormatInfo.InvariantInfo))))
                .Append(" | ").Append(opcode.Undocumented && !documentation.Undocumented ? "Undocumented." : "").Append(" |\n");
        }

        if (documentation.References.Count > 0)
        {
            markdown.Append('\n');
            markdown.Append("## References\n");
            markdown.Append('\n');
            foreach (var reference in documentation.References)
            {
                markdown.Append("- [").Append(reference.Name).Append("](").Append(reference.Url).Append(")\n");
            }
        }

        return markdown.ToString();
    }
}