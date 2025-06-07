namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class ArgumentAccess(string name) : Access(name)
{
    public override DataType Type => DataType.I32;
}