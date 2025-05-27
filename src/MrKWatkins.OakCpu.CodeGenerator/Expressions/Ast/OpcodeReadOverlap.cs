using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class OpcodeReadOverlap : Expression
{
    public static readonly OpcodeReadOverlap Instance = new();

    private OpcodeReadOverlap()
    {
    }

    public override void WriteExpression(StringBuilder expression) => expression.Append("OpcodeReadOverlap");
}