using System.ComponentModel;
using MrKWatkins.OakCpu.CodeGenerator.Generators;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Step
{
    private readonly List<Step> duplicates = new();
    private readonly Action? specifiedAction;
    private Step? root;

    private Step(string name, IReadOnlyList<Statement> statements, Action? specifiedAction = null)
    {
        Name = name;
        Statements = statements;
        this.specifiedAction = specifiedAction;
    }

    public string Name { get; }

    public StepSequence Sequence { get; internal set; } = null!;

    public ushort Index { get; private set; }

    public ushort? FunctionIndex { get; private set; }

    public IReadOnlyList<Statement> Statements { get; }

    public bool HasImplementation => Root.Statements.Count > 0;

    public Step Root => root ?? throw new InvalidOperationException("Root step not set.");

    public IReadOnlyList<Step> Duplicates => duplicates;

    public IReadOnlyList<Step> StepAndDuplicates => Duplicates.Prepend(Root).ToList();

    public Instruction? Instruction => Sequence as Instruction;

    [Pure]
    public Action GetAction(GeneratorContext context)
    {
        if (NextOpcode is null or NextOpcodeMode.Custom)
        {
            return specifiedAction ?? Action.None;
        }

        if (specifiedAction != null)
        {
            throw new InvalidOperationException($"No {PreDefinedFunction.Request.Name} function should be specified for the last step in an instruction, unless the next_opcode mode is set to custom.");
        }

        return NextOpcode == NextOpcodeMode.Overlapped ? context.OpcodeRead.FirstStep.GetAction(context) : Action.None;
    }

    /// <summary>
    /// The next opcode operation to perform after this step.
    /// </summary>
    public NextOpcodeMode? NextOpcode => Sequence.Steps.Last() == this ? Sequence.NextOpcode : null;

    public bool RequiresPrefixReset => Sequence is Instruction { Prefix: not null } && Sequence.Steps[0] == this;

    public bool DoesNothing => Statements.Count == 0 && !RequiresPrefixReset;

    public override string ToString() => $"{Name} => {string.Join("; ", Statements)};";

    [MustUseReturnValue]
    public static IEnumerable<Step> MapDuplicates([InstantHandle] IEnumerable<Step> steps)
    {
        foreach (var group in steps.GroupBy(s => s, StepDuplicateEqualityComparer.Instance))
        {
            var step = group.First();
            step.root = step;
            foreach (var duplicate in group.Skip(1))
            {
                step.duplicates.Add(duplicate);
                duplicate.root = step;
            }

            yield return step;
        }
    }

    internal static void AssignIndices([InstantHandle] IEnumerable<Step> steps)
    {
        var index = 0;
        foreach (var step in steps)
        {
            if (index > ushort.MaxValue)
            {
                throw new InvalidAsynchronousStateException("Too many steps; will need to change to int.");
            }
            step.Index = (ushort)index++;
        }
    }

    internal static void AssignFunctionIndices([InstantHandle] IEnumerable<Step> steps)
    {
        var index = 0;
        foreach (var step in steps)
        {
            if (step.DoesNothing)
            {
                continue;
            }

            step.FunctionIndex = (ushort)index++;
            foreach (var duplicate in step.Duplicates)
            {
                duplicate.FunctionIndex = step.FunctionIndex;
            }
        }
    }

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
}