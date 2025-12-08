using System.Text;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class ConditionAccess(Condition condition) : Access(condition.Name)
{
    public Condition Condition { get; } = condition;

    public override DataType Type => DataType.I32Bool;

    public override void WriteStringRepresentation(StringBuilder stringRepresentation) => stringRepresentation.Append(Condition);
}