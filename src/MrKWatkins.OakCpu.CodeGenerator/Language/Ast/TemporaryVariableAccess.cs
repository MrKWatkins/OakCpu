namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class TemporaryVariableAccess(string name) : Access(name)
{
    public override DataType Type => DataType.I32;
}