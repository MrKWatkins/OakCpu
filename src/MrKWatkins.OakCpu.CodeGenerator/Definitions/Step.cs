using System.ComponentModel;
using MrKWatkins.OakCpu.CodeGenerator.Generators;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Step
{
    private readonly Action? specifiedAction;

    private Step(string name, IReadOnlyList<Statement> statements, Action? specifiedAction = null)
    {
        Name = name;
        Statements = statements;
        this.specifiedAction = specifiedAction;
    }

    public string Name { get; }

    public Instruction? Instruction { get; internal set; }

    public ushort Index { get; private set; }

    public IReadOnlyList<Statement> Statements { get; }

    [Pure]
    public Action GetAction(GeneratorContext context)
    {
        if (NextOpcode.HasValue)
        {
            if (specifiedAction != null)
            {
                throw new InvalidOperationException($"No {PreDefinedFunction.Request.Name} function should be specified for the last step in an instruction.");
            }

            return NextOpcode == NextOpcodeMode.Overlapped ? context.OpcodeReadFirstStep.GetAction(context) : Action.None;
        }

        return specifiedAction ?? Action.None;
    }

    /// <summary>
    /// The next opcode operation to perform after this step.
    /// </summary>
    public NextOpcodeMode? NextOpcode => Instruction != null && Instruction.Steps.Last() == this ? Instruction.NextOpcode : null;

    public bool RequiresPrefixReset => Instruction is { Prefix: not null } && Instruction.Steps[0] == this;

    public bool DoesNothing => Statements.Count == 0 &&
                               !RequiresPrefixReset &&
                               NextOpcode != NextOpcodeMode.Read &&
                               NextOpcode != NextOpcodeMode.Overlapped;

    public override string ToString() => $"{Name} => {string.Join("; ", Statements)};";

    public static void AssignIndexes([InstantHandle] IEnumerable<Step> steps)
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

    [Pure]
    public static IReadOnlyList<Step> Parse(string baseName, ParserContext context, IReadOnlyList<string?> steps) =>
        steps.Select((s, i) => Parse($"{baseName} [{i}]", context, s)).ToList();

    [Pure]
    public static Step Parse(string name, ParserContext context, string? step)
    {
        var statements = Parser.ParseStatements(context, step).ToList();

        var requestStatements = statements.Where(s => s is CallStatement call && call.Call.Function == PreDefinedFunction.Request).OfType<CallStatement>().ToList();
        switch (requestStatements.Count)
        {
            case 0:
                return new Step(name, statements);
            case > 1:
                throw new InvalidOperationException($"The {PreDefinedFunction.Request.Name} function can only be called once per step.");
        }

        var callStatement = requestStatements[0];
        if (callStatement != statements.Last())
        {
            throw new InvalidOperationException($"The {PreDefinedFunction.Request.Name} function must be the last statement in the step.");
        }

        if (callStatement.Call.Arguments.FirstOrDefault() is not ActionAccess actionAccess)
        {
            throw new InvalidOperationException($"The {PreDefinedFunction.Request.Name} function must have an action as the first argument.");
        }

        // Remove the call from the statements as it is handled by the code generation for the action.
        statements.RemoveAt(statements.Count - 1);

        return new Step(name, statements, actionAccess.Action);
    }
}