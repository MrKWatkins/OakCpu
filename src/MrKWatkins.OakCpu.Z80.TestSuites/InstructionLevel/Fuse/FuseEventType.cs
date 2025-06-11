namespace MrKWatkins.OakCpu.Z80.TestSuites.InstructionLevel.Fuse;

public enum FuseEventType
{
    MemoryContend,
    MemoryRead,
    MemoryWrite,
    PortWrite,
    PortContend,
    PortRead,
}