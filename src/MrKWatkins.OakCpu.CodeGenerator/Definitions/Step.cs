using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Step
{
    private Step(string name, IReadOnlyList<Statement> statements, Action? specifiedAction = null)
    {
        Name = name;
        Statements = statements;
        SpecifiedAction = specifiedAction;
    }

    public string Name { get; }

    public IReadOnlyList<Statement> Statements { get; }

    public override string ToString() => $"{Name} => {string.Join("; ", Statements)};";

    internal Action? SpecifiedAction { get; }

    [Pure]
    public static IReadOnlyList<Step> Parse(string baseName, ParserContext context, IReadOnlyList<string?> steps) =>
        steps.Select((s, i) => Parse($"{baseName} [{i}]", context, s, false)).ToList();

    [Pure]
    public static Step Parse(string name, ParserContext context, string? step, bool requiresCompleteInstruction)
    {
        var statements = Parser.ParseStatements(context, step);
        if (requiresCompleteInstruction)
        {
            statements.AddRange(context.OnInstructionStepsComplete);
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