namespace MrKWatkins.OakCpu.Z80.TestSuites.Fuse;

public enum FuseEventType
{
    MemoryContend,
    MemoryRead,
    MemoryWrite,
    PortWrite,
    PortContend,
    PortRead,
}