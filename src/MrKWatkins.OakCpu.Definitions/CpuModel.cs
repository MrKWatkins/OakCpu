using MrKWatkins.OakCpu.Definitions.Language.Ast;
using MrKWatkins.OakCpu.Definitions.Language.Parsing;
using MrKWatkins.OakCpu.Definitions.Validation;
using MrKWatkins.OakCpu.Definitions.Yaml;
using VYaml.Serialization;
using Action = MrKWatkins.OakCpu.Definitions.Action;

namespace MrKWatkins.OakCpu.Definitions;

/// <summary>
/// The model of a CPU built from its YAML definitions: its configuration, sequences, instructions and the layout of their steps.
/// Generators build their output from this model.
/// </summary>
public sealed class CpuModel
{
    private CpuModel(
        Configuration configuration,
        Cpu cpu,
        Interrupts interrupts,
        IReadOnlyDictionary<string, StepSequence> sequences,
        IReadOnlyDictionary<string, SequenceGroup> sequenceGroups,
        StepSequence opcodeRead,
        IReadOnlyList<Statement> onInstructionStepsComplete,
        IReadOnlyList<Statement> onInstructionComplete,
        IReadOnlyList<Statement> onStepComplete,
        IReadOnlyList<Instruction> instructions,
        IReadOnlyDictionary<byte, PrefixJump> prefixJumps,
        IReadOnlyList<Step> allSteps,
        IReadOnlyDictionary<Step, StepLayout> stepLayouts,
        IReadOnlyList<Step> functionSteps)
    {
        Configuration = configuration;
        Cpu = cpu;
        Interrupts = interrupts;
        Sequences = sequences;
        SequenceGroups = sequenceGroups;
        OpcodeRead = opcodeRead;
        OnInstructionStepsComplete = onInstructionStepsComplete;
        OnInstructionComplete = onInstructionComplete;
        OnStepComplete = onStepComplete;
        Instructions = instructions;
        PrefixJumps = prefixJumps;
        AllSteps = allSteps;
        StepLayouts = stepLayouts;
        FunctionSteps = functionSteps;
    }

    public Configuration Configuration { get; }

    public Cpu Cpu { get; }

    public Interrupts Interrupts { get; }

    public IReadOnlyDictionary<string, StepSequence> Sequences { get; }

    public IReadOnlyDictionary<string, SequenceGroup> SequenceGroups { get; }

    public StepSequence OpcodeRead { get; }

    public IReadOnlyList<Statement> OnInstructionStepsComplete { get; }

    public IReadOnlyList<Statement> OnInstructionComplete { get; }

    public IReadOnlyList<Statement> OnStepComplete { get; }

    public IReadOnlyList<Instruction> Instructions { get; }

    public IReadOnlyDictionary<byte, PrefixJump> PrefixJumps { get; }

    public IReadOnlyList<Step> AllSteps { get; }

    public IReadOnlyDictionary<Step, StepLayout> StepLayouts { get; }

    public IReadOnlyList<Step> FunctionSteps { get; }

    [Pure]
    public StepLayout GetStepLayout(Step step) => StepLayouts.TryGetValue(step, out var layout) ? layout : throw new InvalidOperationException($"No finalized layout has been defined for step {step.Name}.");

    [Pure]
    public StepSequence GetSequence(string name) =>
        Sequences.TryGetValue(name, out var sequence) ? sequence : throw new InvalidOperationException($"No sequence named {name} exists.");

    [Pure]
    public SequenceGroup GetSequenceGroup(string name) =>
        SequenceGroups.TryGetValue(name, out var sequenceGroup) ? sequenceGroup : throw new InvalidOperationException($"No sequence group named {name} exists.");

    /// <summary>
    /// Loads the definitions for a CPU from its definitions directory, merging in the definitions of any CPUs it is based on,
    /// and builds the model.
    /// </summary>
    [Pure]
    public static async Task<CpuModel> LoadAsync(DirectoryInfo definitionsDirectory) => Create(await LoadYamlAsync(definitionsDirectory));

    /// <summary>
    /// Loads the definitions for a CPU from its definitions directory, merging in the definitions of any CPUs it is based on.
    /// </summary>
    [Pure]
    public static async Task<YamlFile> LoadYamlAsync(DirectoryInfo definitionsDirectory)
    {
        var combined = await LoadDirectoryAsync(definitionsDirectory);

        // CPUs can be variants of other CPUs, inheriting their definitions. Definitions directories for the CPUs are siblings of each other.
        var cpus = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { definitionsDirectory.Name };
        var parentDirectory = definitionsDirectory.Parent ?? throw new InvalidOperationException($"Definitions directory {definitionsDirectory} has no parent directory.");
        while (combined.Cpu.Base is { } baseCpu)
        {
            if (!cpus.Add(baseCpu))
            {
                throw new InvalidOperationException($"CPU {combined.Cpu.Name} has a circular base CPU reference to {baseCpu}.");
            }

            var baseDirectory = new DirectoryInfo(Path.Combine(parentDirectory.FullName, baseCpu));
            if (!baseDirectory.Exists)
            {
                throw new InvalidOperationException($"Could not find the definitions directory {baseDirectory} for base CPU {baseCpu}.");
            }

            combined = YamlFile.Merge(await LoadDirectoryAsync(baseDirectory), combined);
        }

        return combined;
    }

    /// <summary>
    /// Loads and combines the YAML files in a definitions directory, without merging in the definitions of any CPUs it is based on.
    /// </summary>
    [Pure]
    public static async Task<YamlFile> LoadDirectoryAsync(DirectoryInfo definitionsDirectory)
    {
        var yamls = new List<YamlFile>();
        foreach (var file in definitionsDirectory.GetFiles("*.yaml", SearchOption.AllDirectories))
        {
            yamls.Add(await DeserializeYamlAsync<YamlFile>(file));
        }

        return YamlFile.Combine(yamls);
    }

    [Pure]
    private static async Task<TYaml> DeserializeYamlAsync<TYaml>(FileInfo file)
    {
        try
        {
            await using var stream = file.OpenRead();
            return await YamlSerializer.DeserializeAsync<TYaml>(stream, YamlOptions.Instance);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Could not load YAML file {file}.", exception);
        }
    }

    /// <summary>
    /// Validates the combined definitions for a CPU and builds the model.
    /// </summary>
    [Pure]
    public static CpuModel Create(YamlFile yaml)
    {
        DefinitionValidation.Validate(yaml);

        var configuration = CreateConfiguration(yaml);
        var parserContext = CreateParserContext(configuration, yaml.OnInstructionStepsComplete);
        var cpu = Cpu.Create(yaml.Cpu);
        var namedSequences = CreateNamedSequences(parserContext, yaml.Sequences);
        var opcodeRead = CreateOpcodeReadSequence(parserContext, namedSequences, yaml);
        var instructions = Instruction.Create(parserContext, cpu, yaml.Instructions);
        var prefixJumps = PrefixJump.Create(parserContext, instructions);
        var interrupts = Interrupts.Create(parserContext, yaml.Interrupts!, namedSequences);
        var sequences = CreateSequences(namedSequences, opcodeRead, interrupts);
        var sequenceGroups = CreateSequenceGroups(yaml, interrupts, sequences);
        var allSteps = CreateAllSteps(opcodeRead, prefixJumps, sequences, instructions);
        var stepLayouts = StepFinalizer.Finalize(allSteps, CreateStepSequences(opcodeRead, prefixJumps, sequences, instructions));

        return new CpuModel(
            configuration,
            cpu,
            interrupts,
            sequences,
            sequenceGroups,
            opcodeRead,
            parserContext.OnInstructionStepsComplete,
            ParseStatements(configuration, yaml.OnInstructionComplete),
            ParseStatements(configuration, yaml.OnStepComplete),
            instructions,
            prefixJumps,
            allSteps,
            stepLayouts.Layouts,
            stepLayouts.FunctionSteps);
    }

    [Pure]
    private static ParserContext CreateParserContext(Configuration configuration, string? onInstructionStepsComplete)
    {
        var context = new ParserContext(configuration);
        return context.WithOnInstructionStepsComplete(Parser.ParseStatements(context, onInstructionStepsComplete));
    }

    [Pure]
    private static IReadOnlyList<Statement> ParseStatements(Configuration configuration, string? statements)
    {
        var context = new ParserContext(configuration);
        return Parser.ParseStatements(context, statements);
    }

    [Pure]
    private static StepSequence CreateOpcodeReadSequence(ParserContext context, IReadOnlyDictionary<string, StepSequence> namedSequences, YamlFile yaml) =>
        namedSequences.TryGetValue("opcode_read", out var opcodeReadSequence)
            ? opcodeReadSequence
            : yaml.OpcodeRead.Any()
                ? NamedStepSequence.Create(context, "opcode_read", yaml.OpcodeRead, NextOpcodeMode.Custom, true)
                : throw new InvalidOperationException("No opcode_read sequence has been defined.");

    [Pure]
    private static IReadOnlyDictionary<string, StepSequence> CreateSequences(IReadOnlyDictionary<string, StepSequence> namedSequences, StepSequence opcodeRead, Interrupts interrupts) =>
        namedSequences.Values
            .Append(opcodeRead)
            .Concat(interrupts.AllSequences)
            .Distinct()
            .Where(sequence => sequence.Name != null)
            .ToDictionary(sequence => sequence.Name!, StringComparer.Ordinal);

    [Pure]
    private static IReadOnlyList<Step> CreateAllSteps(
        StepSequence opcodeRead,
        IReadOnlyDictionary<byte, PrefixJump> prefixJumps,
        IReadOnlyDictionary<string, StepSequence> sequences,
        IReadOnlyList<Instruction> instructions) =>
        // Steps need to keep their order within an instruction or all hell breaks loose.
        opcodeRead
            .Concat(prefixJumps.Values.SelectMany(prefixJump => prefixJump.Steps))
            .Concat(sequences.Values.Where(sequence => !ReferenceEquals(sequence, opcodeRead)).SelectMany(sequence => sequence.Steps))
            .Concat(instructions.SelectMany(instruction => instruction.Steps))
            .ToList();

    [Pure]
    private static IReadOnlyDictionary<Step, StepSequence> CreateStepSequences(
        StepSequence opcodeRead,
        IReadOnlyDictionary<byte, PrefixJump> prefixJumps,
        IReadOnlyDictionary<string, StepSequence> sequences,
        IReadOnlyList<Instruction> instructions) =>
        Enumerable
            .Repeat(opcodeRead, 1)
            .Concat(prefixJumps.Values)
            .Concat(sequences.Values.Where(sequence => !ReferenceEquals(sequence, opcodeRead)))
            .Concat(instructions)
            .SelectMany(sequence => sequence.Steps.Select(step => (Step: step, Sequence: sequence)))
            .ToDictionary(x => x.Step, x => x.Sequence);

    [Pure]
    private static IReadOnlyDictionary<string, StepSequence> CreateNamedSequences(ParserContext context, IReadOnlyList<StepSequenceYaml> yamls) =>
        yamls
            .Select(sequence => NamedStepSequence.Create(context, sequence))
            .ToDictionary(
                sequence => sequence.Name ?? throw new InvalidOperationException("Named sequences must have a name."),
                sequence => (StepSequence)sequence,
                StringComparer.Ordinal);

    [Pure]
    private static Configuration CreateConfiguration(YamlFile yaml)
    {
        var pins = Pin.Create(yaml.Cpu);
        var actions = Action.Create(yaml.Cpu, pins);
        var registers = Register.Create(yaml.Registers);
        var flags = Flag.Create(yaml.Flags);
        var opcodeStepTables = new OpcodeStepTables(yaml.Instructions);
        var userDefinedDataMembers = UserDefinedDataMember.Create(yaml.Cpu.Fields, Visibility.Private)
            .Concat(UserDefinedDataMember.Create(yaml.Interrupts!.Properties, Visibility.Internal))
            .ToDictionary(u => u.Name);

        var userDefinedFunctions = UserDefinedFunction.CreateDeclarations(yaml.Functions);
        var configuration = new Configuration(actions, registers, flags, opcodeStepTables, userDefinedDataMembers, userDefinedFunctions, pins);
        UserDefinedFunction.ParseExpressions(configuration, userDefinedFunctions, yaml.Functions);
        return configuration;
    }

    [Pure]
    private static IReadOnlyDictionary<string, SequenceGroup> CreateSequenceGroups(YamlFile yaml, Interrupts interrupts, IReadOnlyDictionary<string, StepSequence> sequences)
    {
        var groupedSequences = yaml.Sequences
            .Where(sequence => sequence.Group != null)
            .Select(
                sequence => (
                    GroupName: sequence.Group!.Name,
                    sequence.Group.Number,
                    Sequence: sequences.TryGetValue(sequence.Name, out var stepSequence)
                        ? stepSequence
                        : throw new InvalidOperationException($"No sequence named {sequence.Name} exists for sequence group {sequence.Group.Name}.")
                ))
            .ToList();

        var topLevelGroups = groupedSequences
            .Select(member => (member.GroupName, member.Number))
            .ToHashSet();

        var legacyInterruptModes = interrupts.Modes
            .Where(mode => !topLevelGroups.Contains((InterruptMode.SequenceGroupName, mode.Number)))
            .Select(mode => (InterruptMode.SequenceGroupName, mode.Number, mode.Sequence));

        return SequenceGroup.Create(groupedSequences.Concat(legacyInterruptModes));
    }
}