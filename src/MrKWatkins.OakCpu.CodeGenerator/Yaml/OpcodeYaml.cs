using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class OpcodeYaml
{
    public byte Opcode { get; private set; }

    public byte? Prefix { get; private set; }

    public string? R0 { get; private set; }

    public string? R1 { get; private set; }

    public override string ToString() => $"0x{Opcode:X2}";
}