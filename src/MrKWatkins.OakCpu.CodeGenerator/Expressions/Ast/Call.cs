using System.Text;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class Call : Expression
{
    private readonly List<Expression> arguments;

    internal Call(Function function, [InstantHandle] IEnumerable<Expression> arguments)
    {
        Function = function;
        this.arguments = arguments.ToList();
    }

    public Function Function { get; }

    public IReadOnlyList<Expression> Arguments => arguments;

    public override void WriteExpression(StringBuilder expression)
    {
        expression.Append(Function.Name);
        expression.Append('(');
        var separatorNeeded = false;
        foreach (var argument in Arguments)
        {
            if (separatorNeeded)
            {
                expression.Append(", ");
            }
            else
            {
                separatorNeeded = true;
            }
            argument.WriteExpression(expression);
        }
        expression.Append(')');
    }
}