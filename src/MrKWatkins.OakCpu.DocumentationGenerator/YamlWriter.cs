using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Writes the documentation for a CPU's instructions as YAML for other tools to consume. The keys match those of the
/// definitions where the two share a concept: <c>instruction</c>, <c>opcode</c>, <c>documentation</c>, <c>undocumented</c>
/// and the operand placeholders.
/// </summary>
public static partial class YamlWriter
{
    [GeneratedRegex(@"^[A-Za-z0-9][A-Za-z0-9 ,.()'+/_-]*$")]
    private static partial Regex PlainScalarRegex();

    [Pure]
    public static string Write(string cpu, IReadOnlyList<InstructionDocumentation> instructions)
    {
        var yaml = new StringBuilder();
        yaml.Append("cpu: ").Append(Scalar(cpu)).Append('\n');
        yaml.Append("instructions:\n");
        foreach (var instruction in instructions)
        {
            Write(yaml, instruction);
        }

        return yaml.ToString();
    }

    private static void Write(StringBuilder yaml, InstructionDocumentation documentation)
    {
        yaml.Append("  - instruction: ").Append(Scalar(documentation.Instruction)).Append('\n');
        yaml.Append("    defined_by: ").Append(Scalar(documentation.DefinedBy)).Append('\n');
        yaml.Append("    group: ").Append(Scalar(documentation.Group)).Append('\n');
        yaml.Append("    documentation: ").Append(Scalar(documentation.Documentation)).Append('\n');
        yaml.Append("    undocumented: ").Append(Bool(documentation.Undocumented)).Append('\n');
        yaml.Append("    page: ").Append(Scalar(documentation.Page)).Append('\n');

        yaml.Append("    flags:\n");
        foreach (var flag in documentation.Flags)
        {
            yaml.Append("      ").Append(flag.Flag.Name).Append(":\n");
            yaml.Append("        effect: ").Append(flag.Kind.ToString().ToLowerInvariant()).Append('\n');
            yaml.Append("        documentation: ").Append(Scalar(flag.Documentation)).Append('\n');
        }

        yaml.Append("    opcodes:\n");
        foreach (var opcode in documentation.Opcodes)
        {
            yaml.Append("      - opcode: ").Append(Scalar(opcode.Opcode)).Append('\n');
            yaml.Append("        instruction: ").Append(Scalar(opcode.Instruction)).Append('\n');
            foreach (var operand in opcode.Operands)
            {
                yaml.Append("        ").Append(operand.Key).Append(": ").Append(Scalar(operand.Value)).Append('\n');
            }
            yaml.Append("        t_states: [").Append(string.Join(", ", opcode.TStates.Select(t => t.ToString(NumberFormatInfo.InvariantInfo)))).Append("]\n");
            yaml.Append("        undocumented: ").Append(Bool(opcode.Undocumented)).Append('\n');
        }

        if (documentation.References.Count > 0)
        {
            yaml.Append("    references:\n");
            foreach (var reference in documentation.References)
            {
                yaml.Append("      - name: ").Append(Scalar(reference.Name)).Append('\n');
                yaml.Append("        url: ").Append(Scalar(reference.Url.ToString())).Append('\n');
            }
        }
    }

    [Pure]
    private static string Bool(bool value) => value ? "true" : "false";

    [Pure]
    private static string Scalar(string value) =>
        PlainScalarRegex().IsMatch(value) && !value.Contains(": ", StringComparison.Ordinal) && !value.Contains(" #", StringComparison.Ordinal) && !value.EndsWith(':')
            ? value
            : $"\"{value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal)}\"";
}