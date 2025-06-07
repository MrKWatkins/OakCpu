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

    public override DataType Type => Function.Type;

    public IReadOnlyList<Expression> Arguments => arguments;

    public override void WriteStringRepresentation(StringBuilder stringRepresentation)
    {
        stringRepresentation.Append(Function.Name);
        stringRepresentation.Append('(');
        var separatorNeeded = false;
        foreach (var argument in Arguments)
        {
            if (separatorNeeded)
            {
                stringRepresentation.Append(", ");
            }
            else
            {
                separatorNeeded = true;
            }
            argument.WriteStringRepresentation(stringRepresentation);
        }
        stringRepresentation.Append(')');
    }
}