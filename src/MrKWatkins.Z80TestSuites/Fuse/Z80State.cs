namespace MrKWatkins.Z80TestSuites.Fuse;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public abstract class Z80State
{
    public ushort AF { get; private set; }

    public ushort BC { get; private set; }

    public ushort DE { get; private set; }

    public ushort HL { get; private set; }

    public ushort ShadowAF { get; private set; }

    public ushort ShadowBC { get; private set; }

    public ushort ShadowDE { get; private set; }

    public ushort ShadowHL { get; private set; }

    public ushort IX { get; private set; }

    public ushort IY { get; private set; }

    public ushort SP { get; private set; }

    public ushort PC { get; private set; }

    public ushort WZ { get; private set; }

    public byte I { get; private set; }

    public byte R { get; private set; }

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
        state.AF = registers[0].ToWord();
        state.BC = registers[1].ToWord();
        state.DE = registers[2].ToWord();
        state.HL = registers[3].ToWord();
        state.ShadowAF = registers[4].ToWord();
        state.ShadowBC = registers[5].ToWord();
        state.ShadowDE = registers[6].ToWord();
        state.ShadowHL = registers[7].ToWord();
        state.IX = registers[8].ToWord();
        state.IY = registers[9].ToWord();
        state.SP = registers[10].ToWord();
        state.PC = registers[11].ToWord();
        state.WZ = registers[12].ToWord();
    }

    private static void ParseInterrupts(string line, Z80State state)
    {
        var interrupts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        state.I = interrupts[0].ToByte();
        state.R = interrupts[1].ToByte();
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