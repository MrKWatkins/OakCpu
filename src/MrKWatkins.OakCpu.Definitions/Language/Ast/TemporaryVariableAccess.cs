namespace MrKWatkins.OakCpu.Definitions.Language.Ast;

public sealed class TemporaryVariableAccess(TemporaryVariable variable) : Access(variable.Name), IReferencesTemporaryVariable
{
    public TemporaryVariable Variable { get; } = variable;

    public override DataType Type => Variable.Type;
}