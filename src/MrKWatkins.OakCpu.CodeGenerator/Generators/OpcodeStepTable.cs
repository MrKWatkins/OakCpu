namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class OpcodeStepTable
{
    public static readonly OpcodeStepTable NoPrefix = new("NoPrefix", null, null);

    private OpcodeStepTable(string name, byte? prefix, string? customName)
    {
        Name = name;
        Prefix = prefix;
        CustomName = customName;
    }

    [Pure]
    public static OpcodeStepTable CreatePrefix(byte prefix) => new($"Prefix{prefix:X2}", prefix, null);

    [Pure]
    public static OpcodeStepTable CreateCustom(string name) => new(name, null, name);

    public string Name { get; }

    public byte? Prefix { get; }

    public string? CustomName { get; }

    public string FieldName => $"OpcodeStepTable{Name}";
}