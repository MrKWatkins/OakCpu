namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class ArgumentAccess(string name) : Access(name)
{
    public override DataType Type => DataType.I32;
}