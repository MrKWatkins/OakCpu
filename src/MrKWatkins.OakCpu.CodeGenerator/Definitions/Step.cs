using System.ComponentModel;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Parsing;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Step
{
    private Step(string name, IReadOnlyList<Statement> statements)
    {
        if (statements.Count == 0)
        {
            throw new ArgumentException("Value cannot be empty.", nameof(statements));
        }

        if (statements.Last() is not TerminalStatement)
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
    public static IReadOnlyList<Step> Parse(string baseName, ParserContext context, IReadOnlyList<IReadOnlyList<string>> steps, Statement? finalStepStatement = null) =>
        steps.Select((s, i) => Parse($"{baseName} [{i}]", context, s, i == steps.Count - 1 ? finalStepStatement : null)).ToList();

    [Pure]
    public static Step Parse(string name, ParserContext context, [InstantHandle] IEnumerable<string> expressions, Statement? finalStatement = null)
    {
        var statements = expressions.Select(x => ExpressionParser.ParseStatement(context, x)).ToList();
        if (finalStatement != null)
        {
            statements.Add(finalStatement);
        }

        var lastStep = statements.LastOrDefault();
        if (lastStep is not TerminalStatement)
        {
            statements.Add(RequestAction.None);
        }
        return new Step(name, statements);
    }
}