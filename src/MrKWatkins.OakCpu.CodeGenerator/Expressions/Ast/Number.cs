using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class Number(int value) : Expression
{
    public int Value { get; } = value;

    public override DataType Type =>
        Value switch
        {
            <= 255 => DataType.U8,
            <= 65535 => DataType.U16,
            _ => DataType.I32
        };

    public string NumberString =>
        Type switch
        {
            DataType.U8 => $"0x{Value:X2}",
            DataType.U16 => $"0x{Value:X4}",
            _ => Value.ToString()
        };

    public override void WriteStringRepresentation(StringBuilder stringRepresentation) => stringRepresentation.Append(NumberString);
}