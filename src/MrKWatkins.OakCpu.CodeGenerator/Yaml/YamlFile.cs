using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class YamlFile
{
    private IReadOnlyList<RegisterYaml>? registers;
    private IReadOnlyList<FlagYaml>? flags;
    private IReadOnlyList<InstructionYaml>? instructions;
    private IReadOnlyList<string?>? opcodeRead;
    private IReadOnlyList<StepSequenceYaml>? sequences;
    private IReadOnlyList<FunctionYaml>? functions;

    private YamlFile()
    {
    }

    public CpuYaml Cpu { get; private set; } = null!;

    /// <summary>
    /// The interrupts, if this file defines any. Most files do not; the definitions for a CPU are combined into a single
    /// <see cref="YamlFile" /> before use, and variant CPUs inherit their interrupts from the CPU they are based on.
    /// </summary>
    public InterruptsYaml? Interrupts { get; private set; }

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

    public IReadOnlyList<string?> OpcodeRead
    {
        get => opcodeRead ?? Sequences.FirstOrDefault(sequence => sequence.Name == "opcode_read")?.Steps ?? [];
        private set => opcodeRead = value;
    }

    public IReadOnlyList<StepSequenceYaml> Sequences
    {
        get => sequences ?? [];
        private set => sequences = value;
    }

    public IReadOnlyList<FunctionYaml> Functions
    {
        get => functions ?? [];
        private set => functions = value;
    }

    public string? OnInstructionStepsComplete { get; private set; }

    public string? OnInstructionComplete { get; private set; }

    public string? OnStepComplete { get; private set; }

    [Pure]
    public static YamlFile Combine(params IEnumerable<YamlFile> files)
    {
        // TODO: Validation on all this.
        CpuYaml? cpu = null;
        InterruptsYaml? interrupts = null;
        string? onInstructionStepsComplete = null;
        string? onInstructionComplete = null;
        string? onStepComplete = null;
        var registers = new List<RegisterYaml>();
        var opcodeRead = new List<string?>();
        var sequences = new List<StepSequenceYaml>();
        var flags = new List<FlagYaml>();
        var instructions = new List<InstructionYaml>();
        var functions = new List<FunctionYaml>();
        foreach (var file in files)
        {
            cpu ??= file.Cpu;
            interrupts ??= file.Interrupts;
            onInstructionStepsComplete ??= file.OnInstructionStepsComplete;
            onInstructionComplete ??= file.OnInstructionComplete;
            onStepComplete ??= file.OnStepComplete;
            if (file.opcodeRead != null)
            {
                opcodeRead.AddRange(file.opcodeRead);
            }
            sequences.AddRange(file.Sequences);
            registers.AddRange(file.Registers);
            flags.AddRange(file.Flags);
            instructions.AddRange(file.Instructions);
            functions.AddRange(file.Functions);
        }
        var combinedCpu = cpu ?? throw new InvalidOperationException("No cpu definition found.");

        // Variants inherit their interrupts from the CPU they are based on, so they do not need to define any themselves.
        if (interrupts == null && combinedCpu.Base == null)
        {
            throw new InvalidOperationException("No interrupts definition found.");
        }

        return new YamlFile
        {
            Cpu = combinedCpu,
            Interrupts = interrupts,
            OnInstructionStepsComplete = onInstructionStepsComplete,
            OnInstructionComplete = onInstructionComplete,
            OnStepComplete = onStepComplete,
            OpcodeRead = opcodeRead,
            Sequences = sequences,
            Registers = registers,
            Flags = flags,
            Instructions = instructions,
            Functions = functions
        };
    }

    /// <summary>
    /// Merges the definitions for a variant CPU into the definitions for the CPU it is based on. Registers and flags are appended to
    /// those from the base CPU; sequences and functions with the same name as one from the base CPU replace it; instructions replace
    /// the individual opcodes they define, rather than entire instructions from the base CPU.
    /// </summary>
    [Pure]
    public static YamlFile Merge(YamlFile @base, YamlFile variant) =>
        new()
        {
            Cpu = CpuYaml.Merge(@base.Cpu, variant.Cpu),
            Interrupts = variant.Interrupts ?? @base.Interrupts,
            OnInstructionStepsComplete = variant.OnInstructionStepsComplete ?? @base.OnInstructionStepsComplete,
            OnInstructionComplete = variant.OnInstructionComplete ?? @base.OnInstructionComplete,
            OnStepComplete = variant.OnStepComplete ?? @base.OnStepComplete,
            OpcodeRead = variant.OpcodeRead.Count > 0 ? variant.OpcodeRead : @base.OpcodeRead,
            Sequences = MergeByName(@base.Sequences, variant.Sequences, sequence => sequence.Name),
            Registers = [.. @base.Registers, .. variant.Registers],
            Flags = [.. @base.Flags, .. variant.Flags],
            Instructions = MergeInstructions(@base.Instructions, variant.Instructions),
            Functions = MergeByName(@base.Functions, variant.Functions, function => function.Name)
        };

    [Pure]
    private static IReadOnlyList<InstructionYaml> MergeInstructions(IReadOnlyList<InstructionYaml> @base, IReadOnlyList<InstructionYaml> variant)
    {
        var variantOpcodes = variant
            .SelectMany(instruction => instruction.Opcodes.Select(opcode => (instruction.OpcodeTable, opcode.PrefixByte, opcode.OpcodeByte)))
            .ToHashSet();

        var instructions = new List<InstructionYaml>();
        foreach (var instruction in @base)
        {
            var opcodes = instruction.Opcodes
                .Where(opcode => !variantOpcodes.Contains((instruction.OpcodeTable, opcode.PrefixByte, opcode.OpcodeByte)))
                .ToList();

            if (opcodes.Count == instruction.Opcodes.Count)
            {
                instructions.Add(instruction);
            }
            else if (opcodes.Count > 0)
            {
                // The variant has overridden some, but not all, of the opcodes for the instruction.
                instructions.Add(instruction.WithOpcodes(opcodes));
            }

            // Otherwise the variant has overridden every opcode for the instruction, so it no longer exists.
        }

        instructions.AddRange(variant);

        return instructions;
    }

    [Pure]
    private static IReadOnlyList<TYaml> MergeByName<TYaml>(IReadOnlyList<TYaml> @base, IReadOnlyList<TYaml> variant, Func<TYaml, string> getName)
    {
        var variantNames = variant.Select(getName).ToHashSet(StringComparer.Ordinal);

        return [.. @base.Where(item => !variantNames.Contains(getName(item))), .. variant];
    }
}