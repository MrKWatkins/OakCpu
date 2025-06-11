namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class PreDefinedDataMember : DataMember
{
    public static readonly PreDefinedDataMember OpcodeStepTable = new("opcodeStepTable", DataType.U16, isArray: true);
    public static readonly PreDefinedDataMember Address = new("Address", DataType.U16, DataMemberVisibility.Public);
    public static readonly PreDefinedDataMember Data = new("Data", DataType.U8, DataMemberVisibility.Public);
    public static readonly PreDefinedDataMember Step = new("step", DataType.U16, DataMemberVisibility.Internal);   // TODO: Make private.

    public static readonly IReadOnlyDictionary<string, PreDefinedDataMember> All = new Dictionary<string, PreDefinedDataMember>(StringComparer.OrdinalIgnoreCase)
    {
        { OpcodeStepTable.Name, OpcodeStepTable },
        { Address.Name, Address },
        { Data.Name, Data },
        { Step.Name, Step }
    };

    private PreDefinedDataMember(string name, DataType type, DataMemberVisibility visibility = DataMemberVisibility.Private, bool isArray = false)
        : base(name, type, visibility, isArray)
    {
    }
}