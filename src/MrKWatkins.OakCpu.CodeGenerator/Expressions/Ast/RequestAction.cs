using System.Text;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class RequestAction(Action action) : TerminalStatement
{
    public static readonly RequestAction None = new(Action.None);

    public Action Action { get; } = action;

    protected override void WriteStatementStringRepresentation(StringBuilder stringRepresentation)
    {
        stringRepresentation.Append("action.");
        stringRepresentation.Append(Action.Name);
        stringRepresentation.Append("()");
    }
}