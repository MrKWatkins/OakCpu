namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class PreDefinedFunction : Function
{
    public static readonly PreDefinedFunction CopyFrom = new("copy_from", typeof(byte), ["value"]);
    public static readonly PreDefinedFunction Flags = new("flags", typeof(byte), []);
    public static readonly PreDefinedFunction InstructionFinishedIf = new("instruction_finished_if", typeof(void), ["value"]);
    public static readonly PreDefinedFunction IsNegative = new("is_negative", typeof(byte), ["value"]);
    public static readonly PreDefinedFunction IsZero = new("is_zero", typeof(byte), ["value"]);
    public static readonly PreDefinedFunction PopCount = new("pop_count", typeof(byte), ["value"]);

    public static readonly IReadOnlyDictionary<string, PreDefinedFunction> All = new Dictionary<string, PreDefinedFunction>(StringComparer.OrdinalIgnoreCase)
    {
        { CopyFrom.Name, CopyFrom },
        { Flags.Name, Flags },
        { InstructionFinishedIf.Name, InstructionFinishedIf },
        { IsNegative.Name, IsNegative },
        { IsZero.Name, IsZero },
        { PopCount.Name, PopCount }
    };

    private PreDefinedFunction(string name, Type type, IReadOnlyList<string> parameters)
        : base(name, type, parameters)
    {
    }

    public override string ToString() => $"{Name}({string.Join(", ", Parameters)}";
}