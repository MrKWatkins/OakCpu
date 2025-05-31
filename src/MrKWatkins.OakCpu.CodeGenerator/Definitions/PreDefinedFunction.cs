namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class PreDefinedFunction : Function
{
    public static readonly PreDefinedFunction CopyFrom = new("copy_from", typeof(byte), ["value"]);
    public static readonly PreDefinedFunction Flags = new("flags", typeof(byte), []);
    public static readonly PreDefinedFunction IsZero = new("is_zero", typeof(byte), ["value"]);
    public static readonly PreDefinedFunction PopCount = new("pop_count", typeof(byte), ["value"]);
    public static readonly PreDefinedFunction Request = new("request", typeof(void), ["action"]);

    public static readonly IReadOnlyDictionary<string, PreDefinedFunction> All = new Dictionary<string, PreDefinedFunction>(StringComparer.OrdinalIgnoreCase)
    {
        { CopyFrom.Name, CopyFrom },
        { Flags.Name, Flags },
        { IsZero.Name, IsZero },
        { PopCount.Name, PopCount },
        { Request.Name, Request }
    };

    private PreDefinedFunction(string name, Type type, IReadOnlyList<string> parameters)
        : base(name, type, parameters)
    {
    }

    public override string ToString() => $"{Name}({string.Join(", ", Parameters)}";
}