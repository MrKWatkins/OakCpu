namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction.SingleStep;

#pragma warning disable CA1028
[Flags]
public enum Pins : byte
{
    None = 0,
    Read = 1,
    Write = 2,
    Memory = 4,
    IO = 8,
    MemoryRead = Read | Memory,
    MemoryWrite = Write | Memory,
    IORead = Read | IO,
    IOWrite = Write | IO
}
#pragma warning restore CA1028