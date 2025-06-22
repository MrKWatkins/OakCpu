namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction;

public sealed class InstructionIO : IIOReader, IIOWriter
{
    private readonly List<IOEvent> ioWrites = new();

    public byte Read(ushort port)
    {
        throw new NotImplementedException();
    }

    public void Write(ushort port, byte value) => ioWrites.Add(new IOEvent(port, value));

    public IReadOnlyList<IOEvent> IOWrites => ioWrites;
}