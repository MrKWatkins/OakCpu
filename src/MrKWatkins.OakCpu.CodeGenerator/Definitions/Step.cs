using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Parsing;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Step(IReadOnlyList<Expression> expressions)
{
    public IReadOnlyList<Expression> Expressions { get; } = expressions;

    [Pure]
    public static IReadOnlyList<Step> Parse(ParserContext context, IReadOnlyList<IReadOnlyList<string>> steps) =>
        steps.Select(s => Parse(context, s)).ToList();

    [Pure]
    public static Step Parse(ParserContext context, IReadOnlyList<string> expressions) =>
        new(expressions.Select(x =>
        {
            var expression = ExpressionParser.Parse(context, x);
            if (expression is not Assignment && expression is not RequestAction)
            {
                throw new InvalidOperationException($"Steps cannot be expressions of type {expression.GetType().Name}: {expression}");
            }
            return expression;
        }).ToList());
}