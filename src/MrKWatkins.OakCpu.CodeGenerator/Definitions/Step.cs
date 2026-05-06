using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Step
{
    private readonly List<Step> duplicates = new();
    private ushort? index;
    private readonly Action? specifiedAction;
    private Step? implementation;
    private ushort? methodIndex;
    private bool methodIndexAssigned;
    private StepSequence? sequence;

    private Step(string name, IReadOnlyList<Statement> statements, Action? specifiedAction = null)
    {
        Name = name;
        Statements = statements;
        this.specifiedAction = specifiedAction;
    }

    public string Name { get; }

    public StepSequence Sequence => sequence ?? throw new InvalidOperationException($"{nameof(Sequence)} not set.");

    public ushort Index => index ?? throw new InvalidOperationException($"{nameof(Index)} not set.");

    /// <summary>
    /// The index of the method in the generated code, or <c>null</c> if this is not an implementation step or the step does nothing.
    /// </summary>
    public ushort? MethodIndex => methodIndexAssigned ? methodIndex : throw new InvalidOperationException($"{nameof(MethodIndex)} not set.");

    public IReadOnlyList<Statement> Statements { get; }

    /// <summary>
    /// The <see cref="Step" /> instance we used to generate the implementation code. For unique steps this will be this. For a step with
    /// duplicates, the first step encountered will have this set to itself, and all duplicates will have this set to that step.
    /// </summary>
    public Step Implementation => implementation ?? throw new InvalidOperationException($"{nameof(Implementation)} step not set.");

    /// <summary>
    /// Any duplicates of this step. Will only contain steps for the implementation steps we used to generate code.
    /// </summary>
    public IReadOnlyList<Step> Duplicates => duplicates;

    public IReadOnlyList<Step> ImplementationAndDuplicates => Duplicates.Prepend(Implementation).ToList();

    [Pure]
    public Action RequiredAction
    {
        get
        {
            if (NextOpcode is null or NextOpcodeMode.Custom)
            {
                return OverlapAction;
            }

            if (specifiedAction != null)
            {
                throw new InvalidOperationException($"No {PreDefinedFunction.Request.Name} function should be specified for the last step in an instruction, unless the next_opcode mode is set to custom.");
            }

            return Action.None;
        }
    }

    public Action OverlapAction => specifiedAction ?? Action.None;

    /// <summary>
    /// The next opcode operation to perform after this step.
    /// </summary>
    public NextOpcodeMode? NextOpcode => Sequence.Steps.Last() == this ? Sequence.NextOpcode : null;

    public bool RequiresPrefixReset => Sequence is Instruction { Prefix: not null } && Sequence.Steps[0] == this;

    public bool ExecutesStoredOverlapOnStart => Sequence.ExecuteOverlapOnStart && Sequence.FirstStep == this;

    public bool ExecutesAsOverlapOnly =>
        Sequence is Instruction { NextOpcode: NextOpcodeMode.Overlapped } &&
        (Sequence.Steps.Count == 1 ? Sequence.Steps[0] == this : Sequence.Steps[^1] == this);

    public bool QueuesOverlapStep =>
        Sequence is Instruction { NextOpcode: NextOpcodeMode.Overlapped } &&
        Sequence.Steps.Count > 1 &&
        Sequence.Steps[^2] == this;

    public Step QueuedOverlapStep =>
        QueuesOverlapStep
            ? Sequence.Steps[^1]
            : throw new InvalidOperationException($"The step {Name} does not queue an overlap step.");

    public bool DoesNothing => Statements.Count == 0 && !RequiresPrefixReset && NextOpcode != NextOpcodeMode.Overlapped;

    public override string ToString() => $"{Name} => {string.Join("; ", Statements)};";

    [Pure]
    public static IReadOnlyList<Step> Parse(string baseName, ParserContext context, IReadOnlyList<string?> steps) =>
        steps.Select((s, i) => Parse($"{baseName} [{i}]", context, s, false)).ToList();

    [Pure]
    public static Step Parse(string name, ParserContext context, string? step, bool requiresCompleteInstruction)
    {
        var statements = Parser.ParseStatements(context, step);
        if (requiresCompleteInstruction)
        {
            statements.AddRange(context.OnInstructionComplete);
        }

        var requestStatements = statements.Where(s => s is CallStatement call && call.Call.Function == PreDefinedFunction.Request).OfType<CallStatement>().ToList();
        switch (requestStatements.Count)
        {
            case 0:
                return new Step(name, statements);
            case > 1:
                throw new InvalidOperationException($"The {PreDefinedFunction.Request.Name} function can only be called once per step.");
        }

        var callStatement = requestStatements[0];
        if (callStatement.Call.Arguments.FirstOrDefault() is not ActionAccess actionAccess)
        {
            throw new InvalidOperationException($"The {PreDefinedFunction.Request.Name} function must have an action as the first argument.");
        }

        // Remove the call from the statements as it is handled by the code generation for the action.
        statements.Remove(callStatement);

        return new Step(name, statements, actionAccess.Action);
    }

    internal void AddDuplicate(Step duplicate) => duplicates.Add(duplicate);

    internal void AssignIndex(ushort value) => index = value;

    internal void AssignMethodIndex(ushort? value)
    {
        methodIndex = value;
        methodIndexAssigned = true;
    }

    internal void AttachToSequence(StepSequence value) => sequence = value;

    internal void SetImplementation(Step value) => implementation = value;
}