using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class Boolean : Expression
{
    public static readonly Boolean True = new(true);
    public static readonly Boolean False = new(false);

    private Boolean(bool value)
    {
        Value = value;
    }

    public bool Value { get; }

    public override DataType Type => DataType.Bool;

    public override void WriteStringRepresentation(StringBuilder stringRepresentation) => stringRepresentation.Append(Value.ToString());
}