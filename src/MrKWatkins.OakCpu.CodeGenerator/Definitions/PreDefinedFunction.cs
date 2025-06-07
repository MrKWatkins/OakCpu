namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class PreDefinedFunction : Function
{
    public static readonly PreDefinedFunction CopyFrom = new("copy_from", DataType.U8, ["value"]);
    public static readonly PreDefinedFunction Flags = new("flags", DataType.Void, []);
    public static readonly PreDefinedFunction FinishInstruction = new("finish_instruction", DataType.Void, ["value"]);
    public static readonly PreDefinedFunction IsNegative = new("is_negative", DataType.I32Bool, ["value"]);
    public static readonly PreDefinedFunction IsZero = new("is_zero", DataType.I32Bool, ["value"]);
    public static readonly PreDefinedFunction PopCount = new("pop_count", DataType.U8, ["value"]);
    public static readonly PreDefinedFunction Signed = new("signed", DataType.I8, ["value"]);

    public static readonly IReadOnlyDictionary<string, PreDefinedFunction> All = new Dictionary<string, PreDefinedFunction>(StringComparer.OrdinalIgnoreCase)
    {
        { CopyFrom.Name, CopyFrom },
        { Flags.Name, Flags },
        { FinishInstruction.Name, FinishInstruction },
        { IsNegative.Name, IsNegative },
        { IsZero.Name, IsZero },
        { PopCount.Name, PopCount },
        { Signed.Name, Signed }
    };

    private PreDefinedFunction(string name, DataType type, IReadOnlyList<string> parameters)
        : base(name, type, parameters)
    {
    }

    public override string ToString() => $"{Name}({string.Join(", ", Parameters)}";
}