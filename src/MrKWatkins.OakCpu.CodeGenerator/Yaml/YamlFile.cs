using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class YamlFile
{
    private IReadOnlyList<RegisterYaml>? registers;
    private IReadOnlyList<FlagYaml>? flags;
    private IReadOnlyList<InstructionYaml>? instructions;

    private YamlFile()
    {
    }

    public CpuYaml Cpu { get; private set; } = null!;

    public IReadOnlyList<RegisterYaml> Registers
    {
        get => registers ?? [];
        private set => registers = value;
    }

    public IReadOnlyList<FlagYaml> Flags
    {
        get => flags ?? [];
        private set => flags = value;
    }

    public IReadOnlyList<InstructionYaml> Instructions
    {
        get => instructions ?? [];
        private set => instructions = value;
    }

    [Pure]
    public static YamlFile Combine(params IEnumerable<YamlFile> files)
    {
        CpuYaml? cpu = null;
        var registers = new List<RegisterYaml>();
        var flags = new List<FlagYaml>();
        var instructions = new List<InstructionYaml>();
        foreach (var file in files)
        {
            cpu ??= file.Cpu;
            registers.AddRange(file.Registers);
            flags.AddRange(file.Flags);
            instructions.AddRange(file.Instructions);
        }
        return new YamlFile { Cpu = cpu ?? throw new InvalidOperationException("No cpu definition found."), Registers = registers, Flags = flags, Instructions = instructions };
    }
}