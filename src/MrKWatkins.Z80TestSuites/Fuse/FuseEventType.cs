namespace MrKWatkins.Z80TestSuites.Fuse;

public enum FuseEventType
{
    MemoryContend,
    MemoryRead,
    MemoryWrite,
    PortWrite,
    PortContend,
    PortRead,
}