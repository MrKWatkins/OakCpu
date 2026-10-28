using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.Definitions;

public sealed class Register
{
    internal Register(string name, DataType type, Documentation documentation, bool flags, bool programCounter, string? category, int fieldOffset, bool hasRegisterClassProperty, Register? parent, bool transient = false, bool undocumented = false)
    {
        Name = name;
        Type = type;
        Documentation = documentation;
        Flags = flags;
        ProgramCounter = programCounter;
        Transient = transient;
        Undocumented = undocumented;
        Category = category;
        FieldOffset = fieldOffset;
        HasRegisterClassProperty = hasRegisterClassProperty;
        Parent = parent;
    }

    public string Name { get; }

    public string FieldName => Category != null ? $"{Category}_{PropertyName}" : PropertyName;

    public string PropertyName => Name.Replace("'", "", StringComparison.Ordinal);

    public DataType Type { get; }

    public Documentation Documentation { get; }

    public bool Flags { get; }

    public bool ProgramCounter { get; }

    // A transient register is never read at its full 16-bit width across instruction boundaries, e.g. WZ, which
    // every instruction that uses it writes before reading. Its halves can be stored back independently without
    // risking a 16-bit read stalling on the two byte stores.
    public bool Transient { get; }

    /// <summary>
    /// Whether the register is undocumented, i.e. not in the manufacturer's documentation for the CPU.
    /// </summary>
    public bool Undocumented { get; }

    public string? Category { get; }

    public int FieldOffset { get; }

    public bool HasRegisterClassProperty { get; }

    public Register? Parent { get; }

    public Register? HighRegister { get; private set; }

    public Register? LowRegister { get; private set; }

    public override string ToString() => $"{Name}: {Type}";

    [Pure]
    public static IReadOnlyDictionary<string, Register> Create(IReadOnlyList<RegisterYaml> yamls)
    {
        var registers = new List<Register>();
        if (yamls.Any(y => y.Type != DataType.U16 && y.Type != DataType.U8))
        {
            throw new InvalidOperationException("Registers must have type u8 or u16.");
        }
        if (yamls.Any(y => (y.High != null && y.High.Type != DataType.U8) || (y.Low != null && y.Low.Type != DataType.U8)))  // skipcq: CS-R1136
        {
            throw new InvalidOperationException("High and low registers must have type u8.");
        }

        var fieldOffset = 0;
        var u8Size = DataType.U8.Size();
        foreach (var yaml in Order(yamls.Where(y => y.Type == DataType.U16)))
        {
            var registerPair = new Register(yaml.Name, yaml.Type, Documentation.Create(yaml.Documentation, $"register {yaml.Name}"), yaml.Flags, yaml.ProgramCounter, yaml.Category, fieldOffset, true, null, yaml.Transient, yaml.Undocumented);
            registers.Add(registerPair);

            // Little endian; low byte is at the lowest address.
            registerPair.LowRegister = yaml.Low != null
                ? new Register(yaml.Low.Name, yaml.Low.Type, Documentation.Create(yaml.Low.Documentation, $"register {yaml.Low.Name}"), yaml.Low.Flags, yaml.Low.ProgramCounter, yaml.Category, fieldOffset, true, registerPair, yaml.Transient, yaml.Low.Undocumented)
                : new Register(yaml.Name + "L", DataType.U8, Documentation.Empty, false, false, yaml.Category, fieldOffset, false, registerPair, yaml.Transient);

            registers.Add(registerPair.LowRegister);
            fieldOffset += u8Size;

            registerPair.HighRegister = yaml.High != null
                ? new Register(yaml.High.Name, yaml.High.Type, Documentation.Create(yaml.High.Documentation, $"register {yaml.High.Name}"), yaml.High.Flags, yaml.High.ProgramCounter, yaml.Category, fieldOffset, true, registerPair, yaml.Transient, yaml.High.Undocumented)
                : new Register(yaml.Name + "H", DataType.U8, Documentation.Empty, false, false, yaml.Category, fieldOffset, false, registerPair, yaml.Transient);

            registers.Add(registerPair.HighRegister);
            fieldOffset += u8Size;
        }

        foreach (var yaml in Order(yamls.Where(y => y.Type == DataType.U8)))
        {
            registers.Add(new Register(yaml.Name, yaml.Type, Documentation.Create(yaml.Documentation, $"register {yaml.Name}"), yaml.Flags, yaml.ProgramCounter, yaml.Category, fieldOffset, true, null, yaml.Transient, yaml.Undocumented));
            fieldOffset += u8Size;
        }

        return registers.ToDictionary(r => r.Name);
    }

    [Pure]
    private static IOrderedEnumerable<RegisterYaml> Order(IEnumerable<RegisterYaml> yamls) =>
        // Put U16s first, so they are all on a two-byte boundary.
        yamls.OrderByDescending(y => y.Type).ThenBy(y => y.Category).ThenBy(y => y.Name);
}