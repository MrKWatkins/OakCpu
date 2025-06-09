using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class ActionAccess(Action action) : Access(action.Name)
{
    public Action Action { get; } = action;

    public override DataType Type => DataType.I32;
}