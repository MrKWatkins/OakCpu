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
}