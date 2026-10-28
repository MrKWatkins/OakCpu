using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.Definitions.Language.Ast;
using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class GeneratorContext
{
    private readonly OverlapState overlap;
    private readonly InstructionEmulatorDispatchInfo instructionEmulatorDispatchInfo;

    private GeneratorContext(string rootNamespace, CpuModel model, OverlapState overlap)
    {
        RootNamespace = rootNamespace;
        Model = model;
        this.overlap = overlap;
        instructionEmulatorDispatchInfo = CreateInstructionEmulatorDispatchInfo();
    }

    public string RootNamespace { get; }

    /// <summary>
    /// The model of the CPU built from its definitions.
    /// </summary>
    public CpuModel Model { get; }

    public Configuration Configuration => Model.Configuration;

    public Cpu Cpu => Model.Cpu;

    public Interrupts Interrupts => Model.Interrupts;

    public IReadOnlyDictionary<string, StepSequence> Sequences => Model.Sequences;

    public IReadOnlyDictionary<string, SequenceGroup> SequenceGroups => Model.SequenceGroups;

    public StepSequence OpcodeRead => Model.OpcodeRead;

    public IReadOnlyList<Statement> OnInstructionStepsComplete => Model.OnInstructionStepsComplete;

    public IReadOnlyList<Statement> OnInstructionComplete => Model.OnInstructionComplete;

    public IReadOnlyList<Statement> OnStepComplete => Model.OnStepComplete;

    public IReadOnlyList<Instruction> Instructions => Model.Instructions;

    public IReadOnlyDictionary<byte, PrefixJump> PrefixJumps => Model.PrefixJumps;

    public IReadOnlyList<Step> FunctionSteps => Model.FunctionSteps;

    public IReadOnlyList<Step> AllSteps => Model.AllSteps;

    public IReadOnlyList<Step> OverlapSteps => overlap.OverlapSteps;

    public int ErrorStepIndex => AllSteps.Count;

    public IReadOnlyList<StepSequence> InstructionEmulatorSequences => instructionEmulatorDispatchInfo.Sequences;

    public int InstructionEmulatorDispatchCount => instructionEmulatorDispatchInfo.ErrorIndex + 1;

    public ushort InstructionEmulatorErrorIndex => instructionEmulatorDispatchInfo.ErrorIndex;

    public ushort GetInstructionEmulatorSequenceIndex(StepSequence sequence) => instructionEmulatorDispatchInfo.SequenceIndices[sequence];

    [Pure]
    public StepLayout GetStepLayout(Step step) => Model.GetStepLayout(step);

    [Pure]
    internal int GetImplicitInstructionStepsCompleteStatementCount(Step step)
    {
        if (OnInstructionStepsComplete.Count == 0 || step.Statements.Count < OnInstructionStepsComplete.Count)
        {
            return 0;
        }

        return step.Statements
            .Skip(step.Statements.Count - OnInstructionStepsComplete.Count)
            .Zip(OnInstructionStepsComplete, ReferenceEquals)
            .All(match => match)
                ? OnInstructionStepsComplete.Count
                : 0;
    }

    [Pure]
    public StepSequence GetSequence(string name) => Model.GetSequence(name);

    [Pure]
    public SequenceGroup GetSequenceGroup(string name) => Model.GetSequenceGroup(name);

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
    public static async Task<GeneratorContext> CreateAsync(string rootNamespace, DirectoryInfo definitionsDirectory) =>
        Create(rootNamespace, await CpuModel.LoadAsync(definitionsDirectory));

    [Pure]
    internal static GeneratorContext Create(string rootNamespace, YamlFile yaml) => Create(rootNamespace, CpuModel.Create(yaml));

    [Pure]
    public static GeneratorContext Create(string rootNamespace, CpuModel model)
    {
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
            .Concat(Sequences.Values)
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
    private static OverlapState CreateOverlapState(string rootNamespace, CpuModel model)
    {
        var overlapCandidates = model.AllSteps
            .Where(step => model.GetStepLayout(step).ExecutesAsOverlapOnly)
            .ToList();

        var overlapGroups = CreateOverlapGroups(rootNamespace, model, overlapCandidates);

        var overlapSteps = overlapGroups
            .Select(group => group.First())
            .OrderBy(step => model.GetStepLayout(step).Index)
            .ToList();

        var overlapImplementations = overlapGroups
            .SelectMany(group => group.Select(step => (Step: step, Implementation: group.First())))
            .ToDictionary(x => x.Step, x => x.Implementation);

        var overlapImplementationAndDuplicates = overlapGroups
            .ToDictionary(group => group.First(), group => (IReadOnlyList<Step>)group.OrderBy(step => model.GetStepLayout(step).Index).ToList());

        var overlapMethodIndices = overlapSteps
            .Select((step, index) => (Step: step, Index: index))
            .ToDictionary(x => x.Step, x => x.Index);

        var overlapIndices = overlapSteps
            .Select((step, index) => (Step: step, Index: (ushort)(index + 1)))
            .ToDictionary(x => x.Step, x => x.Index);

        return new OverlapState(overlapSteps, overlapImplementations, overlapImplementationAndDuplicates, overlapMethodIndices, overlapIndices);
    }

    [Pure]
    private static IReadOnlyList<IReadOnlyList<Step>> CreateOverlapGroups(string rootNamespace, CpuModel model, IReadOnlyList<Step> overlapCandidates)
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