namespace MrKWatkins.Z80TestSuites.Fuse;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public abstract class Z80State
{
    public ushort RegisterAF { get; private set; }

    public ushort RegisterBC { get; private set; }

    public ushort RegisterDE { get; private set; }

    public ushort RegisterHL { get; private set; }

    public ushort ShadowRegisterAF { get; private set; }

    public ushort ShadowRegisterBC { get; private set; }

    public ushort ShadowRegisterDE { get; private set; }

    public ushort ShadowRegisterHL { get; private set; }

    public ushort RegisterIX { get; private set; }

    public ushort RegisterIY { get; private set; }

    public ushort RegisterSP { get; private set; }

    public ushort RegisterPC { get; private set; }

    public ushort RegisterWZ { get; private set; }

    public bool FlagC => (RegisterAF & 0b00000001) != 0;

    public bool FlagN => (RegisterAF & 0b00000010) != 0;

    public bool FlagPV => (RegisterAF & 0b00000100) != 0;

    public bool FlagX => (RegisterAF & 0b00001000) != 0;

    public bool FlagH => (RegisterAF & 0b00010000) != 0;

    public bool FlagY => (RegisterAF & 0b00100000) != 0;

    public bool FlagZ => (RegisterAF & 0b01000000) != 0;

    public bool FlagS => (RegisterAF & 0b10000000) != 0;

    public byte RegisterI { get; private set; }

    public byte RegisterR { get; private set; }

    public bool IFF1 { get; private set; }

    public bool IFF2 { get; private set; }

    public byte IM { get; private set; }

    public bool IsHalted { get; private set; }

    public int TStates { get; private set; }

    public IReadOnlyList<MemorySection> Memory { get; private set; } = null!;

    protected static void Parse(string firstLine, StreamReader reader, Z80State state)
    {
        ParseRegisters(firstLine, state);
        ParseInterrupts(reader.ReadLine()!, state);

        var memory = new List<MemorySection>();
        while (reader.ReadLine() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line) || line == "-1")
            {
                break;
            }

            memory.Add(ParseMemory(line));
        }

        state.Memory = memory;
    }

    private static void ParseRegisters(string line, Z80State state)
    {
        var registers = line.Split(' ');
        state.RegisterAF = registers[0].ToWord();
        state.RegisterBC = registers[1].ToWord();
        state.RegisterDE = registers[2].ToWord();
        state.RegisterHL = registers[3].ToWord();
        state.ShadowRegisterAF = registers[4].ToWord();
        state.ShadowRegisterBC = registers[5].ToWord();
        state.ShadowRegisterDE = registers[6].ToWord();
        state.ShadowRegisterHL = registers[7].ToWord();
        state.RegisterIX = registers[8].ToWord();
        state.RegisterIY = registers[9].ToWord();
        state.RegisterSP = registers[10].ToWord();
        state.RegisterPC = registers[11].ToWord();
        state.RegisterWZ = registers[12].ToWord();
    }

    private static void ParseInterrupts(string line, Z80State state)
    {
        var interrupts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        state.RegisterI = interrupts[0].ToByte();
        state.RegisterR = interrupts[1].ToByte();
        state.IFF1 = interrupts[2].ToBool();
        state.IFF2 = interrupts[3].ToBool();
        state.IM = interrupts[4].ToByte();
        state.IsHalted = interrupts[5].ToBool();
        state.TStates = int.Parse(interrupts[6]);
    }

    [Pure]
    private static MemorySection ParseMemory(string line)
    {
        var data = line.Split(' ');
        var address = data[0].ToWord();
        var bytes = data[1..^1].Select(d => d.ToByte()).ToArray();
        return new MemorySection(address, bytes);
    }
}