namespace MrKWatkins.OakCpu.Z80.TestSuites.InstructionLevel;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum TestEventType
{
    MemoryContend,
    OpcodeRead,
    MemoryRead,
    MemoryWrite,
    IOContend,
    IORead,
    IOWrite
}