using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class MoveToOpcode : Statement
{
    public static readonly MoveToOpcode Instance = new();

    private MoveToOpcode()
    {
    }

    public override void WriteStringRepresentation(StringBuilder stringRepresentation) => stringRepresentation.Append("move_to_opcode()");
}