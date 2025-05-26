using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class RequestAction(string name) : Expression
{
    public string Name { get; } = name;

    public override void WriteExpression(StringBuilder expression)
    {
        expression.Append("Action.");
        expression.Append(Name);
    }
}