using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class MoveToOpcodeRead : Statement
{
    public static readonly MoveToOpcodeRead Instance = new();

    private MoveToOpcodeRead()
    {
    }

    public override void WriteStringRepresentation(StringBuilder stringRepresentation) => stringRepresentation.Append("move_to_opcode_read()");
}