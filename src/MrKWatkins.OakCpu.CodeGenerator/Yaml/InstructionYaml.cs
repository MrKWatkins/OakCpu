using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class InstructionYaml
{
    private InstructionYaml()
    {
    }

    public string Name { get; private set; } = null!;

    public byte Opcode { get; private set; }

    public override string ToString() => $"0x{Opcode:X2}: {Name}";
}