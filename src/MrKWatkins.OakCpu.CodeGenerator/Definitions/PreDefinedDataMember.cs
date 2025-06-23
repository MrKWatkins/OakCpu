namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class PreDefinedDataMember : DataMember
{
    public static readonly PreDefinedDataMember OpcodeStepTable = new("opcode_step_table", DataType.U16, isArray: true);
    public static readonly PreDefinedDataMember Address = new("address", DataType.U16, getterVisibility: Visibility.Public);
    public static readonly PreDefinedDataMember Data = new("data", DataType.U8, getterVisibility: Visibility.Public, setterVisibility: Visibility.Public);
    public static readonly PreDefinedDataMember CurrentStep = new("current_step", DataType.U16);

    public static readonly IReadOnlyDictionary<string, PreDefinedDataMember> All = new Dictionary<string, PreDefinedDataMember>(StringComparer.OrdinalIgnoreCase)
    {
        { OpcodeStepTable.Name, OpcodeStepTable },
        { Address.Name, Address },
        { Data.Name, Data },
        { CurrentStep.Name, CurrentStep }
    };

    private PreDefinedDataMember(string name, DataType type, Visibility fieldVisibility = Visibility.Private, Visibility? getterVisibility = null, Visibility? setterVisibility = null, bool isArray = false)
        : base(name, type, fieldVisibility, getterVisibility, setterVisibility, isArray)
    {
    }
}