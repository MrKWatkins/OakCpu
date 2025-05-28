using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class OverlappedOpcodeRead : TerminalStatement
{
    public static readonly OverlappedOpcodeRead Instance = new();

    private OverlappedOpcodeRead()
    {
    }

    public override void WriteExpression(StringBuilder expression) => expression.Append(nameof(OverlappedOpcodeRead));
}