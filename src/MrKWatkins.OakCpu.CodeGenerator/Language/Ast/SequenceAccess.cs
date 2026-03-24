namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class SequenceAccess(string sequenceName) : Access($"sequence.{sequenceName}")
{
    public string SequenceName { get; } = sequenceName;

    public override DataType Type => DataType.I32;
}
