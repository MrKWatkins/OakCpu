namespace MrKWatkins.OakCpu.Z80.TestSuites;

public interface IIOWriter
{
    void Write(ushort port, byte value);
}