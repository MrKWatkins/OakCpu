namespace MrKWatkins.OakCpu.Z80.TestSuites;

public interface IIOReader
{
    [Pure]
    byte Read(ushort port);
}