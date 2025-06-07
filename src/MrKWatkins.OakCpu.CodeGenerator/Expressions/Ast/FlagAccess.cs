using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class FlagAccess(Flag flag) : Access(flag.Name)
{
    public Flag Flag { get; } = flag;

    public override DataType Type => DataType.I32Bool;
}