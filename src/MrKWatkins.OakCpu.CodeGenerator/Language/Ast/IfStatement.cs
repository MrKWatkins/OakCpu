using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class IfStatement : Statement
{
    internal IfStatement(Expression condition)
        : this(condition, [], [])
    {
    }

    private IfStatement(Expression condition, IReadOnlyList<Statement> ifStatements, IReadOnlyList<Statement> elseStatements)
    {
        Condition = condition;
        IfStatements = ifStatements;
        ElseStatements = elseStatements;
    }

    public Expression Condition { get; }

    public IReadOnlyList<Statement> IfStatements { get; }

    public IReadOnlyList<Statement> ElseStatements { get; }

    [Pure]
    internal IfStatement WithStatements(IReadOnlyList<Statement> ifStatements, IReadOnlyList<Statement> elseStatements) => new(Condition, ifStatements, elseStatements);

    public override void WriteStringRepresentation(StringBuilder stringRepresentation)
    {
        stringRepresentation.Append("if ");
        stringRepresentation.Append(Condition);
        stringRepresentation.Append("; ");
        foreach (var statement in IfStatements)
        {
            statement.WriteStringRepresentation(stringRepresentation);
            stringRepresentation.Append("; ");
        }
        stringRepresentation.Append("endif;");
    }
}