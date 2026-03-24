using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class GeneratorContext
{
    private GeneratorContext(
        string rootNamespace,
        Configuration configuration,
        Cpu cpu,
        Interrupts interrupts,
        IReadOnlyDictionary<string, StepSequence> sequences,
        IReadOnlyDictionary<string, SequenceGroup> sequenceGroups,
        StepSequence opcodeRead,
        IReadOnlyList<Statement> onInstructionComplete,
        IReadOnlyList<Instruction> instructions,
        IReadOnlyDictionary<byte, PrefixJump> prefixJumps,
        IReadOnlyList<Step> allSteps,
        IReadOnlyList<Step> functionSteps,
        IReadOnlyList<Step> overlapSteps,
        IReadOnlyDictionary<Step, Step> overlapImplementations,
        IReadOnlyDictionary<Step, IReadOnlyList<Step>> overlapImplementationAndDuplicates,
        IReadOnlyDictionary<Step, int> overlapMethodIndices,
        IReadOnlyDictionary<Step, ushort> overlapIndices)
    {
        RootNamespace = rootNamespace;
        Configuration = configuration;
        Cpu = cpu;
        Interrupts = interrupts;
        Sequences = sequences;
        SequenceGroups = sequenceGroups;
        OpcodeRead = opcodeRead;
        OnInstructionComplete = onInstructionComplete;
        Instructions = instructions;
        PrefixJumps = prefixJumps;
        AllSteps = allSteps;
        FunctionSteps = functionSteps;
        OverlapSteps = overlapSteps;
        this.overlapImplementations = overlapImplementations;
        this.overlapImplementationAndDuplicates = overlapImplementationAndDuplicates;
        this.overlapMethodIndices = overlapMethodIndices;
        this.overlapIndices = overlapIndices;
    }

    private readonly IReadOnlyDictionary<Step, Step> overlapImplementations;
    private readonly IReadOnlyDictionary<Step, IReadOnlyList<Step>> overlapImplementationAndDuplicates;
    private readonly IReadOnlyDictionary<Step, int> overlapMethodIndices;
    private readonly IReadOnlyDictionary<Step, ushort> overlapIndices;

    public string RootNamespace { get; }

    public Configuration Configuration { get; }

    public Cpu Cpu { get; }

    public Interrupts Interrupts { get; }

    public IReadOnlyDictionary<string, StepSequence> Sequences { get; }

    public IReadOnlyDictionary<string, SequenceGroup> SequenceGroups { get; }

    public StepSequence OpcodeRead { get; }

    public IReadOnlyList<Statement> OnInstructionComplete { get; }

    public IReadOnlyList<Instruction> Instructions { get; }

    public IReadOnlyDictionary<byte, PrefixJump> PrefixJumps { get; }

    public IReadOnlyList<Step> FunctionSteps { get; }

    public IReadOnlyList<Step> AllSteps { get; }

    public IReadOnlyList<Step> OverlapSteps { get; }

    public HashSet<string> RequiredUsings { get; } = new();

    public int ErrorStepIndex => AllSteps.Count;

    [Pure]
    public StepSequence GetSequence(string name) =>
        Sequences.TryGetValue(name, out var sequence) ? sequence : throw new InvalidOperationException($"No sequence named {name} exists.");

    [Pure]
    public SequenceGroup GetSequenceGroup(string name) =>
        SequenceGroups.TryGetValue(name, out var sequenceGroup) ? sequenceGroup : throw new InvalidOperationException($"No sequence group named {name} exists.");

    [Pure]
    public int GetOverlapMethodIndex(Step step) =>
        overlapMethodIndices.TryGetValue(GetOverlapImplementation(step), out var index) ? index : throw new InvalidOperationException($"No overlap has been defined for step {step.Name}.");

    [Pure]
    public ushort GetOverlapIndex(Step step) =>
        overlapIndices.TryGetValue(GetOverlapImplementation(step), out var index) ? index : throw new InvalidOperationException($"No overlap has been defined for step {step.Name}.");

    [Pure]
    public IReadOnlyList<Step> GetOverlapImplementationAndDuplicates(Step step) =>
        overlapImplementationAndDuplicates.TryGetValue(GetOverlapImplementation(step), out var steps)
            ? steps
            : throw new InvalidOperationException($"No overlap has been defined for step {step.Name}.");

    [Pure]
    public FileScopedNamespaceDeclarationSyntax CreateRootNamespaceDeclaration() => SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName(RootNamespace));

    [Pure]
    public static async Task<GeneratorContext> CreateAsync(string rootNamespace, DirectoryInfo definitionsDirectory)
    {
        var yamls = new List<YamlFile>();
        foreach (var file in definitionsDirectory.GetFiles("*.yaml", SearchOption.AllDirectories))
        {
            yamls.Add(await DeserializeYamlAsync<YamlFile>(file));
        }

        var combined = YamlFile.Combine(yamls);

        return Create(rootNamespace, combined);
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

    [Pure]
    internal static GeneratorContext Create(string rootNamespace, YamlFile yaml)
    {
        var configuration = CreateConfiguration(yaml);
        var context = new ParserContext(configuration);

        context = context.WithOnInstructionComplete(Parser.ParseStatements(context, yaml.OnInstructionComplete));

        var namedSequences = CreateNamedSequences(context, yaml.Sequences);
        var opcodeRead = namedSequences.TryGetValue("opcode_read", out var opcodeReadSequence)
            ? opcodeReadSequence
            : yaml.OpcodeRead.Any()
                ? NamedStepSequence.Create(context, "opcode_read", yaml.OpcodeRead, NextOpcodeMode.Custom, true)
                : throw new InvalidOperationException("No opcode_read sequence has been defined.");

        var instructions = Instruction.Create(context, yaml.Instructions);

        var prefixJumps = PrefixJump.Create(context, instructions);

        var interrupts = Interrupts.Create(context, yaml.Interrupts, namedSequences);
        var cpu = Cpu.Create(yaml.Cpu);

        var sequences = namedSequences.Values
            .Append(opcodeRead)
            .Concat(interrupts.AllSequences)
            .Distinct()
            .Where(sequence => sequence.Name != null)
            .ToDictionary(sequence => sequence.Name!, StringComparer.Ordinal);

        var sequenceGroups = CreateSequenceGroups(yaml, interrupts, sequences);

        var auxiliarySequences = namedSequences.Values
            .Where(sequence => !ReferenceEquals(sequence, opcodeRead))
            .Concat(interrupts.AllSequences)
            .Distinct()
            .ToList();

        // Steps need to keep their order within an instruction or all hell breaks loose.
        var allSteps = opcodeRead
            .Concat(prefixJumps.Values.SelectMany(p => p.Steps))
            .Concat(auxiliarySequences.SelectMany(sequence => sequence.Steps))
            .Concat(instructions.SelectMany(i => i.Steps))
            .ToList();

        Step.AssignIndices(allSteps);

        var functionSteps = Step.MapDuplicates(allSteps).ToList();

        Step.AssignMethodIndices(functionSteps);

        var overlapCandidates = allSteps
            .Where(step => step.ExecutesAsOverlapOnly)
            .ToList();

        var overlapGroups = CreateOverlapGroups(rootNamespace, configuration, cpu, interrupts, sequences, sequenceGroups, opcodeRead, context.OnInstructionComplete, instructions, prefixJumps, allSteps, functionSteps, overlapCandidates);

        var overlapSteps = overlapGroups
            .Select(group => group.First())
            .OrderBy(step => step.Index)
            .ToList();

        var overlapImplementations = overlapGroups
            .SelectMany(group => group.Select(step => (Step: step, Implementation: group.First())))
            .ToDictionary(x => x.Step, x => x.Implementation);

        var overlapImplementationAndDuplicates = overlapGroups
            .ToDictionary(group => group.First(), group => (IReadOnlyList<Step>)group.OrderBy(step => step.Index).ToList());

        var overlapMethodIndices = overlapSteps
            .Select((step, index) => (Step: step, Index: index))
            .ToDictionary(x => x.Step, x => x.Index);

        var overlapIndices = overlapSteps
            .Select((step, index) => (Step: step, Index: (ushort)(index + 1)))
            .ToDictionary(x => x.Step, x => x.Index);

        return new GeneratorContext(rootNamespace, configuration, cpu, interrupts, sequences, sequenceGroups, opcodeRead, context.OnInstructionComplete, instructions, prefixJumps, allSteps, functionSteps, overlapSteps, overlapImplementations, overlapImplementationAndDuplicates, overlapMethodIndices, overlapIndices);
    }

    [Pure]
    internal GeneratorContext WithRequiredUsings() => new(RootNamespace, Configuration, Cpu, Interrupts, Sequences, SequenceGroups, OpcodeRead, OnInstructionComplete, Instructions, PrefixJumps, AllSteps, FunctionSteps, OverlapSteps, overlapImplementations, overlapImplementationAndDuplicates, overlapMethodIndices, overlapIndices);

    [Pure]
    private Step GetOverlapImplementation(Step step) =>
        overlapImplementations.TryGetValue(step, out var implementation) ? implementation : throw new InvalidOperationException($"No overlap implementation has been defined for step {step.Name}.");

    [Pure]
    private static IReadOnlyList<IReadOnlyList<Step>> CreateOverlapGroups(
        string rootNamespace,
        Configuration configuration,
        Cpu cpu,
        Interrupts interrupts,
        IReadOnlyDictionary<string, StepSequence> sequences,
        IReadOnlyDictionary<string, SequenceGroup> sequenceGroups,
        StepSequence opcodeRead,
        IReadOnlyList<Statement> onInstructionComplete,
        IReadOnlyList<Instruction> instructions,
        IReadOnlyDictionary<byte, PrefixJump> prefixJumps,
        IReadOnlyList<Step> allSteps,
        IReadOnlyList<Step> functionSteps,
        IReadOnlyList<Step> overlapCandidates)
    {
        var temporaryContext = new GeneratorContext(rootNamespace, configuration, cpu, interrupts, sequences, sequenceGroups, opcodeRead, onInstructionComplete, instructions, prefixJumps, allSteps, functionSteps, [], new Dictionary<Step, Step>(), new Dictionary<Step, IReadOnlyList<Step>>(), new Dictionary<Step, int>(), new Dictionary<Step, ushort>());

        var overlapBodies = overlapCandidates.ToDictionary(
            step => step,
            step => (IReadOnlyList<string>)StatementGenerator.GenerateOverlapStatements(temporaryContext, step)
                .Select(statement => statement.NormalizeWhitespace().ToFullString())
                .ToList());

        var groups = new List<IReadOnlyList<Step>>();
        foreach (var step in overlapCandidates)
        {
            var existingGroup = groups.FirstOrDefault(group => overlapBodies[group[0]].SequenceEqual(overlapBodies[step], StringComparer.Ordinal));
            if (existingGroup is List<Step> mutableGroup)
            {
                mutableGroup.Add(step);
                continue;
            }

            groups.Add(new List<Step> { step });
        }

        return groups;
    }

    [Pure]
    private static IReadOnlyDictionary<string, StepSequence> CreateNamedSequences(ParserContext context, IReadOnlyList<StepSequenceYaml> yamls)
    {
        var duplicates = yamls
            .GroupBy(sequence => sequence.Name, StringComparer.Ordinal)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicates != null)
        {
            throw new InvalidOperationException($"The sequence {duplicates.Key} is defined multiple times.");
        }

        return yamls
            .Select(sequence => NamedStepSequence.Create(context, sequence))
            .ToDictionary(
                sequence => sequence.Name ?? throw new InvalidOperationException("Named sequences must have a name."),
                sequence => (StepSequence)sequence,
                StringComparer.Ordinal);
    }

    [Pure]
    private static Configuration CreateConfiguration(YamlFile yaml)
    {
        var actions = Action.Create(yaml.Cpu);
        var registers = Register.Create(yaml.Registers);
        var flags = Flag.Create(yaml.Flags);
        var opcodeStepTables = new OpcodeStepTables(yaml.Instructions);
        var userDefinedDataMembers = UserDefinedDataMember.Create(yaml.Cpu.Fields, Visibility.Private)
            .Concat(UserDefinedDataMember.Create(yaml.Interrupts.Properties, Visibility.Internal))
            .ToDictionary(u => u.Name);

        var configuration = new Configuration(actions, registers, flags, opcodeStepTables, userDefinedDataMembers);

        UserDefinedFunction.AddToConfiguration(configuration, yaml.Functions);

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
