using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class Number(int value) : Expression
{
    public int Value { get; } = value;

    public override void WriteExpression(StringBuilder expression) => expression.Append(Value > 255 ? $"0x{Value:X4}": $"0x{Value:X2}");
}