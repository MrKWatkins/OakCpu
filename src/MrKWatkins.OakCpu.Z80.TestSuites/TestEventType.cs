namespace MrKWatkins.OakCpu.Z80.TestSuites;

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