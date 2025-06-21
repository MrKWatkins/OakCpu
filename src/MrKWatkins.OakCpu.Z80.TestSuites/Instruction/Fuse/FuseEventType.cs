namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction.Fuse;

#pragma warning disable CA1028
public enum FuseEventType : byte
{
    MemoryContend,
    MemoryRead,
    MemoryWrite,
    PortWrite,
    PortContend,
    PortRead,
}
#pragma warning restore CA1028