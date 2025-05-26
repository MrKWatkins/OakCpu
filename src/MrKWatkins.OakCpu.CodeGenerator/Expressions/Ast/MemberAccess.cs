using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class MemberAccess(string name) : Expression
{
    public static readonly IReadOnlyCollection<string> KnownFields = new HashSet<string> { "Address", "Data", "lastOpcode" };

    public string Name { get; } = name;

    public override void WriteExpression(StringBuilder expression)
    {
        expression.Append("Member.");
        expression.Append(Name);
    }
}