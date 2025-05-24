using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Instruction
{
    private Instruction(string name, byte opcode)
    {
        Name = name;
        Opcode = opcode;
    }

    public string Name { get; }

    public byte Opcode { get; }

    public override string ToString() => $"0x{Opcode:X2}: {Name}";

    [Pure]
    public static IReadOnlyList<Instruction> Create(IReadOnlyList<InstructionYaml> yamls) => yamls.Select(y => new Instruction(y.Name, y.Opcode)).OrderBy(f => f.Opcode).ToList();
}