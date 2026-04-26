using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Register
{
    internal Register(string name, DataType type, Documentation documentation, bool flags, bool programCounter, string? category, int fieldOffset, bool hasRegisterClassProperty, Register? parent)
    {
        Name = name;
        Type = type;
        Documentation = documentation;
        Flags = flags;
        ProgramCounter = programCounter;
        Category = category;
        FieldOffset = fieldOffset;
        HasRegisterClassProperty = hasRegisterClassProperty;
        Parent = parent;
    }

    public string Name { get; }

    public string FieldName => Category != null ? $"{Category}_{PropertyName}" : PropertyName;

    public string PropertyName => Name.Replace("'", "", StringComparison.Ordinal);

    public DataType Type { get; }

    public TypeSyntax TypeSyntax => Type.TypeSyntax();

    public Documentation Documentation { get; }

    public bool Flags { get; }

    public bool ProgramCounter { get; }

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
            var registerPair = new Register(yaml.Name, yaml.Type, Documentation.Create(yaml.Documentation, $"register {yaml.Name}"), yaml.Flags, yaml.ProgramCounter, yaml.Category, fieldOffset, true, null);
            registers.Add(registerPair);

            // Little endian; low byte is at the lowest address.
            registerPair.LowRegister = yaml.Low != null
                ? new Register(yaml.Low.Name, yaml.Low.Type, Documentation.Create(yaml.Low.Documentation, $"register {yaml.Low.Name}"), yaml.Low.Flags, yaml.Low.ProgramCounter, yaml.Low.Category, fieldOffset, true, registerPair)
                : new Register(yaml.Name + "L", DataType.U8, Documentation.Empty, false, false, yaml.Category, fieldOffset, false, registerPair);

            registers.Add(registerPair.LowRegister);
            fieldOffset += u8Size;

            registerPair.HighRegister = yaml.High != null
                ? new Register(yaml.High.Name, yaml.High.Type, Documentation.Create(yaml.High.Documentation, $"register {yaml.High.Name}"), yaml.High.Flags, yaml.High.ProgramCounter, yaml.High.Category, fieldOffset, true, registerPair)
                : new Register(yaml.Name + "H", DataType.U8, Documentation.Empty, false, false, yaml.Category, fieldOffset, false, registerPair);

            registers.Add(registerPair.HighRegister);
            fieldOffset += u8Size;
        }

        foreach (var yaml in Order(yamls.Where(y => y.Type == DataType.U8)))
        {
            registers.Add(new Register(yaml.Name, yaml.Type, Documentation.Create(yaml.Documentation, $"register {yaml.Name}"), yaml.Flags, yaml.ProgramCounter, yaml.Category, fieldOffset, true, null));
            fieldOffset += u8Size;
        }

        return registers.ToDictionary(r => r.Name);
    }

    [Pure]
    private static IOrderedEnumerable<RegisterYaml> Order(IEnumerable<RegisterYaml> yamls) =>
        // Put U16s first, so they are all on a two-byte boundary.
        yamls.OrderByDescending(y => y.Type).ThenBy(y => y.Category).ThenBy(y => y.Name);
}