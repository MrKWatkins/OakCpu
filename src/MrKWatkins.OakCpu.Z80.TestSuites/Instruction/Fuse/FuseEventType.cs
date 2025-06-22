namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction.Fuse;

public enum FuseEventType : byte
{
    MemoryContend,
    MemoryRead,
    MemoryWrite,
    PortWrite,
    PortContend,
    PortRead,
}