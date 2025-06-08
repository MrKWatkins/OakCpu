using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class OverlappedOpcodeRead : TerminalStatement
{
    public static readonly OverlappedOpcodeRead Instance = new();

    private OverlappedOpcodeRead()
    {
    }

    protected override void WriteStatementStringRepresentation(StringBuilder stringRepresentation) => stringRepresentation.Append("overlapped_opcode_read()");
}