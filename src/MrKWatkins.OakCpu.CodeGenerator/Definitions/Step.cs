using System.ComponentModel;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Parsing;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Step(IReadOnlyList<Expression> expressions)
{
    public ushort Index { get; private set; }

    public IReadOnlyList<Expression> Expressions { get; } = expressions;

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
    public static IReadOnlyList<Step> Parse(ParserContext context, IReadOnlyList<IReadOnlyList<string>> steps) =>
        steps.Select(s => Parse(context, s)).ToList();

    [Pure]
    public static Step Parse(ParserContext context, [InstantHandle] IEnumerable<string> expressions)
    {
        var steps = expressions.Select(x =>
        {
            var expression = ExpressionParser.Parse(context, x);
            if (expression is not Assignment && expression is not RequestAction && expression is not OpcodeReadOverlap)
            {
                throw new InvalidOperationException($"Steps cannot be expressions of type {expression.GetType().Name}: {expression}");
            }

            return expression;
        }).ToList();

        var lastStep = steps.LastOrDefault();
        if (lastStep is not RequestAction && lastStep is not OpcodeReadOverlap)
        {
            steps.Add(RequestAction.None);
        }
        return new Step(steps);
    }
}