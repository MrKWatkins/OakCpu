using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.Definitions;

public sealed class Cpu
{
    private Cpu(string name, string displayName, Documentation documentation, NextOpcodeMode defaultNextOpcode, Cycles cycles, IReadOnlyList<Operand> operands, IReadOnlyList<Reference> references, IReadOnlyDictionary<string, string> temporaries, Cpu? @base)
    {
        Temporaries = temporaries;
        Name = name;
        DisplayName = displayName;
        Documentation = documentation;
        DefaultNextOpcode = defaultNextOpcode;
        Cycles = cycles;
        Operands = operands;
        References = references;
        Base = @base;
    }

    /// <summary>
    /// The name of the CPU, which is a valid identifier, e.g. <c>M6502</c>.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The name of the CPU for people, e.g. <c>6502</c>.
    /// </summary>
    public string DisplayName { get; }

    public Documentation Documentation { get; }

    public NextOpcodeMode DefaultNextOpcode { get; }

    /// <summary>
    /// How the CPU's clock cycles are counted and named.
    /// </summary>
    public Cycles Cycles { get; }

    /// <summary>
    /// The operands that follow an opcode in memory, as they are named in instructions.
    /// </summary>
    public IReadOnlyList<Operand> Operands { get; }

    /// <summary>
    /// Links to external documentation for the CPU's instructions.
    /// </summary>
    public IReadOnlyList<Reference> References { get; }

    /// <summary>
    /// How the temporaries the instruction steps use are named in documentation, keyed by the temporary's name without its
    /// <c>$</c>, e.g. <c>result</c> = <c>the result</c>.
    /// </summary>
    public IReadOnlyDictionary<string, string> Temporaries { get; }

    /// <summary>
    /// The CPU this CPU is a variant of, if any.
    /// </summary>
    public Cpu? Base { get; }

    /// <summary>
    /// Gets this CPU, or the CPU it is based on, by name.
    /// </summary>
    [Pure]
    public Cpu GetCpu(string name)
    {
        for (var cpu = this; cpu != null; cpu = cpu.Base)
        {
            if (cpu.Name == name)
            {
                return cpu;
            }
        }

        throw new InvalidOperationException($"{Name} is not, and is not based on, a CPU named {name}.");
    }

    public override string ToString() => Name;

    [Pure]
    public static Cpu Create(CpuYaml yaml) =>
        new(
            yaml.Name,
            yaml.EffectiveDisplayName,
            Documentation.CreateOptional(yaml.Documentation, $"CPU {yaml.Name}"),
            yaml.EffectiveDefaultNextOpcode,
            Cycles.Create(yaml.Cycles),
            Operand.Create(yaml.Operands),
            Reference.Create(yaml.References),
            yaml.Temporaries,
            yaml.BaseCpu != null ? Create(yaml.BaseCpu) : null);
}