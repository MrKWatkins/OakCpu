using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Writes the documentation for an instruction as Markdown with YAML front matter. The front matter carries everything in the
/// document as data, so that other tools can consume it without parsing the Markdown.
/// </summary>
public static partial class MarkdownWriter
{
    [GeneratedRegex(@"^[A-Za-z0-9][A-Za-z0-9 ,.()'+/_-]*$")]
    private static partial Regex PlainScalarRegex();

    [Pure]
    public static string Write(InstructionDocumentation documentation)
    {
        var markdown = new StringBuilder();
        WriteFrontMatter(markdown, documentation);
        WriteBody(markdown, documentation);
        return markdown.ToString();
    }

    private static void WriteFrontMatter(StringBuilder markdown, InstructionDocumentation documentation)
    {
        markdown.Append("---\n");
        markdown.Append("cpu: ").Append(Scalar(documentation.Cpu)).Append('\n');
        markdown.Append("defined_by: ").Append(Scalar(documentation.DefinedBy)).Append('\n');
        markdown.Append("form: ").Append(Scalar(documentation.Form)).Append('\n');
        markdown.Append("mnemonic: ").Append(Scalar(documentation.Mnemonic)).Append('\n');
        markdown.Append("group: ").Append(Scalar(documentation.Group)).Append('\n');
        markdown.Append("description: ").Append(Scalar(documentation.Description)).Append('\n');
        markdown.Append("undocumented: ").Append(Bool(documentation.Undocumented)).Append('\n');

        markdown.Append("flags:\n");
        foreach (var flag in documentation.Flags)
        {
            markdown.Append("  - flag: ").Append(Scalar(flag.Flag.Name)).Append('\n');
            markdown.Append("    effect: ").Append(flag.Kind.ToString().ToLowerInvariant()).Append('\n');
            markdown.Append("    description: ").Append(Scalar(flag.Description)).Append('\n');
        }

        markdown.Append("opcodes:\n");
        foreach (var opcode in documentation.Opcodes)
        {
            markdown.Append("  - encoding: ").Append(Scalar(opcode.Encoding)).Append('\n');
            markdown.Append("    mnemonic: ").Append(Scalar(opcode.Mnemonic)).Append('\n');
            if (opcode.Operands.Count > 0)
            {
                markdown.Append("    operands: { ").Append(string.Join(", ", opcode.Operands.Select(o => $"{o.Key}: {Scalar(o.Value)}"))).Append(" }\n");
            }
            markdown.Append("    tstates: [").Append(string.Join(", ", opcode.TStates.Select(t => t.ToString(NumberFormatInfo.InvariantInfo)))).Append("]\n");
            markdown.Append("    undocumented: ").Append(Bool(opcode.Undocumented)).Append('\n');
        }

        if (documentation.References.Count > 0)
        {
            markdown.Append("references:\n");
            foreach (var reference in documentation.References)
            {
                markdown.Append("  - name: ").Append(Scalar(reference.Name)).Append('\n');
                markdown.Append("    url: ").Append(Scalar(reference.Url.ToString())).Append('\n');
            }
        }

        markdown.Append("---\n");
    }

    private static void WriteBody(StringBuilder markdown, InstructionDocumentation documentation)
    {
        var symbolic = documentation.Opcodes.Count > 0
            ? documentation.Opcodes[0].Operands.Select(o => new KeyValuePair<string, string>(o.Key, SymbolicOperand(documentation, o.Key))).ToList()
            : [];

        markdown.Append('\n');
        markdown.Append("# ").Append(documentation.Mnemonic).Append('\n');
        markdown.Append('\n');
        markdown.Append(Operands.Substitute(documentation.Description, symbolic)).Append('\n');

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
            markdown.Append("| ").Append(flag.Flag.Name).Append(" | ").Append(Operands.Substitute(flag.Description, symbolic)).Append(" |\n");
        }

        markdown.Append('\n');
        markdown.Append("## Opcodes\n");
        markdown.Append('\n');
        markdown.Append("| Opcode | Encoding | T-states | Notes |\n");
        markdown.Append("| ------ | -------- | -------- | ----- |\n");
        foreach (var opcode in documentation.Opcodes)
        {
            markdown.Append("| `").Append(opcode.Mnemonic).Append("` | `").Append(opcode.Encoding).Append("` | ")
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
    }

    [Pure]
    private static string SymbolicOperand(InstructionDocumentation documentation, string placeholder)
    {
        // The symbolic mnemonic was built from the same placeholders, so read the symbol back from it.
        var formParts = documentation.Form.Split(' ');
        var mnemonicParts = documentation.Mnemonic.Split(' ');
        for (var index = 0; index < formParts.Length; index++)
        {
            var position = formParts[index].IndexOf(placeholder, StringComparison.Ordinal);
            if (position >= 0 && index < mnemonicParts.Length)
            {
                var symbol = mnemonicParts[index];
                var prefix = formParts[index][..position];
                var suffix = formParts[index][(position + placeholder.Length)..];
                if (symbol.StartsWith(prefix, StringComparison.Ordinal) && symbol.EndsWith(suffix, StringComparison.Ordinal) && symbol.Length >= prefix.Length + suffix.Length)
                {
                    return symbol[prefix.Length..^suffix.Length];
                }
            }
        }

        throw new InvalidOperationException($"Could not find the symbol for {placeholder} in {documentation.Mnemonic}.");
    }

    [Pure]
    private static string Bool(bool value) => value ? "true" : "false";

    [Pure]
    private static string Scalar(string value) =>
        PlainScalarRegex().IsMatch(value) && !value.Contains(": ", StringComparison.Ordinal) && !value.Contains(" #", StringComparison.Ordinal) && !value.EndsWith(':')
            ? value
            : $"\"{value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal)}\"";
}