using System.Globalization;
using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed partial class OpcodeYaml
{
    public string Opcode { get; private set; } = null!;

    public string? R0 { get; private set; }

    public string? R1 { get; private set; }

    public string? RP0 { get; private set; }

    public string? RP1 { get; private set; }

    public string? C0 { get; private set; }

    public byte? N0 { get; private set; }

    public override string ToString() => Opcode;

    [Pure]
    public (byte? Prefix, byte Opcode) GetBytes()
    {
        var values = Opcode
            .Split([' '], StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseHex)
            .ToArray();

        return values.Length == 2 ? (values[0], values[1]) : (null, values[0]);
    }

    [Pure]
    private static byte ParseHex(string hex)
    {
        if (hex.StartsWith("0x"))
        {
            return byte.Parse(hex.Substring(2), NumberStyles.HexNumber);
        }
        throw new InvalidOperationException($"{hex} is not a hex number.");
    }
}