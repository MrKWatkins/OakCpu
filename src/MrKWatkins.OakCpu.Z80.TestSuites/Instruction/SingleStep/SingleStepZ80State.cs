namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction.SingleStep;

// TODO: Unit tests.
// TODO: Delta for final state?
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class SingleStepZ80State : Z80State
{
    [MustUseReturnValue]
    internal static SingleStepZ80State Load(BinaryReader reader)
    {
        var state = new SingleStepZ80State
        {
            RegisterAF = reader.ReadUInt16(),
            RegisterBC = reader.ReadUInt16(),
            RegisterDE = reader.ReadUInt16(),
            RegisterHL = reader.ReadUInt16(),
            RegisterIX = reader.ReadUInt16(),
            RegisterIY = reader.ReadUInt16(),
            RegisterSP = reader.ReadUInt16(),
            RegisterPC = reader.ReadUInt16(),
            RegisterWZ = reader.ReadUInt16(),
            RegisterI = reader.ReadByte(),
            RegisterR = reader.ReadByte(),
            ShadowRegisterAF = reader.ReadUInt16(),
            ShadowRegisterBC = reader.ReadUInt16(),
            ShadowRegisterDE = reader.ReadUInt16(),
            ShadowRegisterHL = reader.ReadUInt16()
        };

        var interrupts = reader.ReadByte();
        state.IFF1 = (interrupts & 0b00000001) == 0b00000001;
        state.IFF2 = (interrupts & 0b00000010) == 0b00000010;
        state.IM = (byte)((interrupts & 0b00001100) >> 2);

        state.Memory = LoadMemory(reader);

        return state;
    }

    [MustUseReturnValue]
    private static IReadOnlyList<MemoryState> LoadMemory(BinaryReader reader)
    {
        var memorySize = reader.Read7BitEncodedInt();
        var memory = new MemoryState[memorySize];
        for (var f = 0; f < memorySize; f++)
        {
            memory[f] = new MemoryState(reader.ReadUInt16(), reader.ReadByte());
        }
        return memory;
    }
}