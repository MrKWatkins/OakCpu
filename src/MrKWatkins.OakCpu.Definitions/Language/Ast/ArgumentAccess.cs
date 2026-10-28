namespace MrKWatkins.OakCpu.Definitions.Language.Ast;

public sealed class ArgumentAccess(string name) : Access(name)
{
    public override DataType Type => DataType.I32;
}