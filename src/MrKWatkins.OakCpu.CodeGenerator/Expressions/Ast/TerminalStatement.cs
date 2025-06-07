using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public abstract class TerminalStatement : Statement
{
    public sealed override void WriteStringRepresentation(StringBuilder stringRepresentation)
    {
        stringRepresentation.Append("return ");
        WriteStatementStringRepresentation(stringRepresentation);
    }

    protected abstract void WriteStatementStringRepresentation(StringBuilder expression);
}