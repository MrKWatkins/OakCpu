namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class PreDefinedDataMember : DataMember
{
    public static readonly PreDefinedDataMember OpcodeStepTable = new("opcodeStepTable", DataType.U16, isArray: true);
    public static readonly PreDefinedDataMember Address = new("Address", DataType.U16, isPublic: true);
    public static readonly PreDefinedDataMember Data = new("Data", DataType.U8, isPublic: true);
    public static readonly PreDefinedDataMember Step = new("step", DataType.U16, isPublic: true);   // TODO: Make private.

    public static readonly IReadOnlyDictionary<string, PreDefinedDataMember> All = new Dictionary<string, PreDefinedDataMember>(StringComparer.OrdinalIgnoreCase)
    {
        { OpcodeStepTable.Name, OpcodeStepTable },
        { Address.Name, Address },
        { Data.Name, Data },
        { Step.Name, Step }
    };

    private PreDefinedDataMember(string name, DataType type, bool isArray = false, bool isPublic = false)
        : base(name, type, isArray, isPublic)
    {
    }
}