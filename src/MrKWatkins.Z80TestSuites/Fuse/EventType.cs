namespace MrKWatkins.Z80TestSuites.Fuse;

public enum EventType
{
    MemoryRead,
    MemoryWrite,
    MemoryContend,
    PortRead,
    PortWrite,
    PortContend
}