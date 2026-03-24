namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class SequenceGroupAccess(string sequenceGroupName) : Access($"sequence_group.{sequenceGroupName}")
{
    public string SequenceGroupName { get; } = sequenceGroupName;

    public override DataType Type => DataType.I32;
}