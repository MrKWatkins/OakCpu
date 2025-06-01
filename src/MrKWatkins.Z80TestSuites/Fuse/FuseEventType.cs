namespace MrKWatkins.Z80TestSuites.Fuse;

public enum FuseEventType
{
    MemoryRead,
    MemoryWrite,
    MemoryContend,
    PortRead,
    PortWrite,
    PortContend
}