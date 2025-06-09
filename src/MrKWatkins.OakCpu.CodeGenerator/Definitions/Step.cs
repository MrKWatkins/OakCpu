using System.ComponentModel;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Step
{
    private Step(string name, IReadOnlyList<Statement> statements)
    {
        if (statements.Count == 0)
        {
            throw new ArgumentException("Value cannot be empty.", nameof(statements));
        }

        if (!statements.Last().IsTerminal)
        {
            throw new ArgumentException("The last statement must be a terminal statement.", nameof(statements));
        }
        Name = name;
        Statements = statements;
    }

    public string Name { get; }

    public Instruction? Instruction { get; internal set; }

    public ushort Index { get; private set; }

    public IReadOnlyList<Statement> Statements { get; }

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

        var lastStep = statements.LastOrDefault();
        if (lastStep is not { IsTerminal: true })
        {
            statements.Add(new CallStatement(new Call(PreDefinedFunction.Request, [new ActionAccess(Action.None)])));
        }
        return new Step(name, statements);
    }
}