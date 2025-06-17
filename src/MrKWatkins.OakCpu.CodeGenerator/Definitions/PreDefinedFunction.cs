namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class PreDefinedFunction : Function
{
    public static readonly PreDefinedFunction CopyFrom = new("copy_from", DataType.U8, ["value"]);
    public static readonly PreDefinedFunction Flags = new("flags", DataType.Void, []);
    public static readonly PreDefinedFunction FinishInstruction = new("finish_instruction", DataType.Void, ["value"]);
    public static readonly PreDefinedFunction IsZero = new("is_zero", DataType.Bool, ["value"]);
    public static readonly PreDefinedFunction MoveToOpcode = new("move_to_opcode", DataType.Void, []);
    public static readonly PreDefinedFunction PopCount = new("pop_count", DataType.U8, ["value"]);
    public static readonly PreDefinedFunction Request = new("request", DataType.Void, ["action"]);
    public static readonly PreDefinedFunction SetOpcodeStepTable = new("set_opcode_step_table", DataType.Void, []);
    public static readonly PreDefinedFunction Signed = new("signed", DataType.I8, ["value"]);

    public static readonly IReadOnlyDictionary<string, PreDefinedFunction> All = new Dictionary<string, PreDefinedFunction>(StringComparer.OrdinalIgnoreCase)
    {
        { CopyFrom.Name, CopyFrom },
        { Flags.Name, Flags },
        { FinishInstruction.Name, FinishInstruction },
        { IsZero.Name, IsZero },
        { MoveToOpcode.Name, MoveToOpcode },
        { PopCount.Name, PopCount },
        { Request.Name, Request },
        { SetOpcodeStepTable.Name, SetOpcodeStepTable },
        { Signed.Name, Signed }
    };

    private PreDefinedFunction(string name, DataType type, IReadOnlyList<string> parameters)
        : base(name, type, parameters)
    {
    }

    public override string ToString() => $"{Name}({string.Join(", ", Parameters)}";
}