using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class OpcodeJump : Statement
{
    public static readonly OpcodeJump Instance = new();

    private OpcodeJump()
    {
    }

    public override void WriteExpression(StringBuilder expression) => expression.Append(nameof(OpcodeJump));
}