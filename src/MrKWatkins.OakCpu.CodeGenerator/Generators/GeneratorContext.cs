using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Validation;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class GeneratorContext
{
    private readonly ModelState model;
    private readonly OverlapState overlap;
    private readonly InstructionEmulatorDispatchInfo instructionEmulatorDispatchInfo;

    private GeneratorContext(string rootNamespace, ModelState model, OverlapState overlap)
    {
        RootNamespace = rootNamespace;
        this.model = model;
        this.overlap = overlap;
        instructionEmulatorDispatchInfo = CreateInstructionEmulatorDispatchInfo();
    }

    public string RootNamespace { get; }

    public Configuration Configuration => model.Configuration;

    public Cpu Cpu => model.Cpu;

    public Interrupts Interrupts => model.Interrupts;

    public IReadOnlyDictionary<string, StepSequence> Sequences => model.Sequences;

    public IReadOnlyDictionary<string, SequenceGroup> SequenceGroups => model.SequenceGroups;

    public StepSequence OpcodeRead => model.OpcodeRead;

    public IReadOnlyList<Statement> OnInstructionComplete => model.OnInstructionComplete;

    public IReadOnlyList<Instruction> Instructions => model.Instructions;

    public IReadOnlyDictionary<byte, PrefixJump> PrefixJumps => model.PrefixJumps;

    public IReadOnlyList<Step> FunctionSteps => model.FunctionSteps;

    public IReadOnlyList<Step> AllSteps => model.AllSteps;

    public IReadOnlyList<Step> OverlapSteps => overlap.OverlapSteps;

    public int ErrorStepIndex => AllSteps.Count;

    public IReadOnlyList<StepSequence> InstructionEmulatorSequences => instructionEmulatorDispatchInfo.Sequences;

    public int InstructionEmulatorDispatchCount => instructionEmulatorDispatchInfo.ErrorIndex + 1;

    public ushort InstructionEmulatorErrorIndex => instructionEmulatorDispatchInfo.ErrorIndex;

    public ushort GetInstructionEmulatorSequenceIndex(StepSequence sequence) => instructionEmulatorDispatchInfo.SequenceIndices[sequence];

    [Pure]
    public StepLayout GetStepLayout(Step step) => model.StepLayouts.TryGetValue(step, out var layout) ? layout : throw new InvalidOperationException($"No finalized layout has been defined for step {step.Name}.");

    [Pure]
    internal int GetImplicitInstructionCompleteStatementCount(Step step)
    {
        if (OnInstructionComplete.Count == 0 || step.Statements.Count < OnInstructionComplete.Count)
        {
            return 0;
        }

        return step.Statements
            .Skip(step.Statements.Count - OnInstructionComplete.Count)
            .Zip(OnInstructionComplete, ReferenceEquals)
            .All(match => match)
                ? OnInstructionComplete.Count
                : 0;
    }

    [Pure]
    public StepSequence GetSequence(string name) =>
        Sequences.TryGetValue(name, out var sequence) ? sequence : throw new InvalidOperationException($"No sequence named {name} exists.");

    [Pure]
    public SequenceGroup GetSequenceGroup(string name) =>
        SequenceGroups.TryGetValue(name, out var sequenceGroup) ? sequenceGroup : throw new InvalidOperationException($"No sequence group named {name} exists.");

    [Pure]
    public int GetOverlapMethodIndex(Step step) =>
        overlap.MethodIndices.TryGetValue(GetOverlapImplementation(step), out var index) ? index : throw new InvalidOperationException($"No overlap has been defined for step {step.Name}.");

    [Pure]
    public ushort GetOverlapIndex(Step step) =>
        overlap.Indices.TryGetValue(GetOverlapImplementation(step), out var index) ? index : throw new InvalidOperationException($"No overlap has been defined for step {step.Name}.");

    [Pure]
    public IReadOnlyList<Step> GetOverlapImplementationAndDuplicates(Step step) =>
        overlap.ImplementationAndDuplicates.TryGetValue(GetOverlapImplementation(step), out var steps)
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
        DefinitionValidation.Validate(yaml);

        var model = CreateModelState(yaml);
        var overlap = CreateOverlapState(rootNamespace, model);
        return new GeneratorContext(rootNamespace, model, overlap);
    }

    [Pure]
    internal FileGeneratorContext CreateFileContext() => new(this);

    [Pure]
    private Step GetOverlapImplementation(Step step) =>
        overlap.Implementations.TryGetValue(step, out var implementation) ? implementation : throw new InvalidOperationException($"No overlap implementation has been defined for step {step.Name}.");

    [Pure]
    private InstructionEmulatorDispatchInfo CreateInstructionEmulatorDispatchInfo()
    {
        var sequences = new[] { OpcodeRead }
            .Concat(PrefixJumps.Values)
            .Concat(Instructions)
            .Concat(SequenceGroups.Values.SelectMany(group => group.Members.Values))
            .Concat(Interrupts.AllSequences)
            .Distinct()
            .OrderBy(sequence => GetStepLayout(sequence.FirstStep).Index)
            .ToList();

        var sequenceIndices = sequences
            .Select((sequence, index) => (sequence, Index: (ushort)index))
            .ToDictionary(x => x.sequence, x => x.Index);

        return new InstructionEmulatorDispatchInfo(sequences, sequenceIndices, (ushort)sequences.Count);
    }

    [Pure]
    private static ModelState CreateModelState(YamlFile yaml)
    {
        var configuration = CreateConfiguration(yaml);
        var parserContext = CreateParserContext(configuration, yaml.OnInstructionComplete);
        var namedSequences = CreateNamedSequences(parserContext, yaml.Sequences);
        var opcodeRead = CreateOpcodeReadSequence(parserContext, namedSequences, yaml);
        var instructions = Instruction.Create(parserContext, yaml.Instructions);
        var prefixJumps = PrefixJump.Create(parserContext, instructions);
        var interrupts = Interrupts.Create(parserContext, yaml.Interrupts, namedSequences);
        var sequences = CreateSequences(namedSequences, opcodeRead, interrupts);
        var sequenceGroups = CreateSequenceGroups(yaml, interrupts, sequences);
        var allSteps = CreateAllSteps(opcodeRead, prefixJumps, sequences, instructions);
        var stepLayouts = StepFinalizer.Finalize(allSteps, CreateStepSequences(opcodeRead, prefixJumps, sequences, instructions));

        return new ModelState(configuration, Cpu.Create(yaml.Cpu), interrupts, sequences, sequenceGroups, opcodeRead, parserContext.OnInstructionComplete, instructions, prefixJumps, allSteps, stepLayouts.Layouts, stepLayouts.FunctionSteps);
    }

    [Pure]
    private static ParserContext CreateParserContext(Configuration configuration, string? onInstructionComplete)
    {
        var context = new ParserContext(configuration);
        return context.WithOnInstructionComplete(Parser.ParseStatements(context, onInstructionComplete));
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
    private static OverlapState CreateOverlapState(string rootNamespace, ModelState model)
    {
        var overlapCandidates = model.AllSteps
            .Where(step => model.StepLayouts[step].ExecutesAsOverlapOnly)
            .ToList();

        var overlapGroups = CreateOverlapGroups(rootNamespace, model, overlapCandidates);

        var overlapSteps = overlapGroups
            .Select(group => group.First())
            .OrderBy(step => model.StepLayouts[step].Index)
            .ToList();

        var overlapImplementations = overlapGroups
            .SelectMany(group => group.Select(step => (Step: step, Implementation: group.First())))
            .ToDictionary(x => x.Step, x => x.Implementation);

        var overlapImplementationAndDuplicates = overlapGroups
            .ToDictionary(group => group.First(), group => (IReadOnlyList<Step>)group.OrderBy(step => model.StepLayouts[step].Index).ToList());

        var overlapMethodIndices = overlapSteps
            .Select((step, index) => (Step: step, Index: index))
            .ToDictionary(x => x.Step, x => x.Index);

        var overlapIndices = overlapSteps
            .Select((step, index) => (Step: step, Index: (ushort)(index + 1)))
            .ToDictionary(x => x.Step, x => x.Index);

        return new OverlapState(overlapSteps, overlapImplementations, overlapImplementationAndDuplicates, overlapMethodIndices, overlapIndices);
    }

    [Pure]
    private static IReadOnlyList<IReadOnlyList<Step>> CreateOverlapGroups(string rootNamespace, ModelState model, IReadOnlyList<Step> overlapCandidates)
    {
        var temporaryContext = new GeneratorContext(rootNamespace, model, OverlapState.Empty);

        var overlapBodies = overlapCandidates.ToDictionary(
            step => step,
            step => (IReadOnlyList<StatementSyntax>)StatementGenerator.GenerateOverlapStatements(temporaryContext.CreateFileContext(), step)
                .ToList());

        var groups = new List<IReadOnlyList<Step>>();
        foreach (var step in overlapCandidates)
        {
            var existingGroup = groups.FirstOrDefault(group => OverlapStatementsEquivalence.AreEquivalent(overlapBodies[group[0]], overlapBodies[step]));
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
        var actions = Action.Create(yaml.Cpu);
        var registers = Register.Create(yaml.Registers);
        var flags = Flag.Create(yaml.Flags);
        var opcodeStepTables = new OpcodeStepTables(yaml.Instructions);
        var userDefinedDataMembers = UserDefinedDataMember.Create(yaml.Cpu.Fields, Visibility.Private)
            .Concat(UserDefinedDataMember.Create(yaml.Interrupts.Properties, Visibility.Internal))
            .ToDictionary(u => u.Name);

        var userDefinedFunctions = UserDefinedFunction.CreateDeclarations(yaml.Functions);
        var configuration = new Configuration(actions, registers, flags, opcodeStepTables, userDefinedDataMembers, userDefinedFunctions);
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

    private sealed record ModelState(
        Configuration Configuration,
        Cpu Cpu,
        Interrupts Interrupts,
        IReadOnlyDictionary<string, StepSequence> Sequences,
        IReadOnlyDictionary<string, SequenceGroup> SequenceGroups,
        StepSequence OpcodeRead,
        IReadOnlyList<Statement> OnInstructionComplete,
        IReadOnlyList<Instruction> Instructions,
        IReadOnlyDictionary<byte, PrefixJump> PrefixJumps,
        IReadOnlyList<Step> AllSteps,
        IReadOnlyDictionary<Step, StepLayout> StepLayouts,
        IReadOnlyList<Step> FunctionSteps);

    private sealed record OverlapState(
        IReadOnlyList<Step> OverlapSteps,
        IReadOnlyDictionary<Step, Step> Implementations,
        IReadOnlyDictionary<Step, IReadOnlyList<Step>> ImplementationAndDuplicates,
        IReadOnlyDictionary<Step, int> MethodIndices,
        IReadOnlyDictionary<Step, ushort> Indices)
    {
        public static OverlapState Empty { get; } = new(
            [],
            new Dictionary<Step, Step>(),
            new Dictionary<Step, IReadOnlyList<Step>>(),
            new Dictionary<Step, int>(),
            new Dictionary<Step, ushort>());
    }

    private sealed record InstructionEmulatorDispatchInfo(
        IReadOnlyList<StepSequence> Sequences,
        IReadOnlyDictionary<StepSequence, ushort> SequenceIndices,
        ushort ErrorIndex);
}