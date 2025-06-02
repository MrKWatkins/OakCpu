namespace MrKWatkins.Z80TestSuites;

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