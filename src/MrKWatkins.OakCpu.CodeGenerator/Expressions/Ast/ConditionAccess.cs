using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class ConditionAccess(Condition condition) : Access(condition.Name)
{
    public Condition Condition { get; } = condition;

    public override DataType Type => DataType.I32Bool;
}