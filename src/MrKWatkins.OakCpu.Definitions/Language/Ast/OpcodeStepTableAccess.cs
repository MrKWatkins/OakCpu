namespace MrKWatkins.OakCpu.Definitions.Language.Ast;

public sealed class OpcodeStepTableAccess(OpcodeStepTable opcodeStepTable) : Access(opcodeStepTable.Name)
{
    public OpcodeStepTable OpcodeStepTable { get; } = opcodeStepTable;

    public override DataType Type => DataType.I32;
}