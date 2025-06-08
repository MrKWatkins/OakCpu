using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class IfStatement : Statement
{
    internal IfStatement(Expression condition)
        : this(condition, [])
    {
    }

    private IfStatement(Expression condition, IReadOnlyList<Statement> body)
    {
        Condition = condition;
        Body = body;
    }

    public Expression Condition { get; }

    public IReadOnlyList<Statement> Body { get; }

    [Pure]
    internal IfStatement WithBody(IReadOnlyList<Statement> body) => new(Condition, body);

    public override void WriteStringRepresentation(StringBuilder stringRepresentation)
    {
        stringRepresentation.Append("if ");
        stringRepresentation.Append(Condition);
        stringRepresentation.Append("; ");
        foreach (var statement in Body)
        {
            statement.WriteStringRepresentation(stringRepresentation);
            stringRepresentation.Append("; ");
        }
        stringRepresentation.Append("endif;");
    }
}