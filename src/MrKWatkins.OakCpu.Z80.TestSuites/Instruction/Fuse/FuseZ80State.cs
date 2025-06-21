namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction.Fuse;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public abstract class FuseZ80State : Z80State
{
    public ulong TStates { get; private set; }

    protected void Parse(string firstLine, StreamReader reader)
    {
        ParseRegisters(firstLine);
        ParseInterrupts(reader.ReadLine()!);

        var memory = new List<MemoryState>();
        while (reader.ReadLine() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line) || line == "-1")
            {
                break;
            }

            memory.AddRange(ParseMemory(line));
        }

        Memory = memory;
    }

    private void ParseRegisters(string line)
    {
        var registers = line.Split(' ');
        RegisterAF = registers[0].ToWord();
        RegisterBC = registers[1].ToWord();
        RegisterDE = registers[2].ToWord();
        RegisterHL = registers[3].ToWord();
        ShadowRegisterAF = registers[4].ToWord();
        ShadowRegisterBC = registers[5].ToWord();
        ShadowRegisterDE = registers[6].ToWord();
        ShadowRegisterHL = registers[7].ToWord();
        RegisterIX = registers[8].ToWord();
        RegisterIY = registers[9].ToWord();
        RegisterSP = registers[10].ToWord();
        RegisterPC = registers[11].ToWord();
        RegisterWZ = registers[12].ToWord();
    }

    private void ParseInterrupts(string line)
    {
        var interrupts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        RegisterI = interrupts[0].ToByte();
        RegisterR = interrupts[1].ToByte();
        IFF1 = interrupts[2].ToBool();
        IFF2 = interrupts[3].ToBool();
        IM = interrupts[4].ToByte();
        Halted = interrupts[5].ToBool();
        TStates = ulong.Parse(interrupts[6]);
    }

    [Pure]
    private static IEnumerable<MemoryState> ParseMemory(string line)
    {
        var data = line.Split(' ');
        var address = data[0].ToWord();
        foreach (var @byte in data[1..^1].Select(d => d.ToByte()))
        {
            yield return new MemoryState(address, @byte);
            address++;
        }
    }
}