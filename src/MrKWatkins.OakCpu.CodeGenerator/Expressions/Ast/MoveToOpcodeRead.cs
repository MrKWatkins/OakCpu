using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class MoveToOpcodeRead : Statement
{
    public static readonly MoveToOpcodeRead Instance = new();

    private MoveToOpcodeRead()
    {
    }

    public override void WriteExpression(StringBuilder expression) => expression.Append(nameof(MoveToOpcodeRead));
}