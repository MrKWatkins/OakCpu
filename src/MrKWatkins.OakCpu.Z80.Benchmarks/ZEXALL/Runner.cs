namespace MrKWatkins.OakCpu.Z80.Benchmarks.ZEXALL;

internal abstract class Runner
{
    protected const ushort StartAddress = 0x0100;
    protected const ushort StopAddress = 0x0000;

    internal abstract void Run();
}