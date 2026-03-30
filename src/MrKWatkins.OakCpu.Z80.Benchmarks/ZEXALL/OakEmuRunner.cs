using MrKWatkins.OakEmu.Cpus.Z80;

namespace MrKWatkins.OakCpu.Z80.Benchmarks.ZEXALL;

internal sealed class OakEmuRunner : Runner
{
    private readonly Z80Emulator emulator = new();

    internal OakEmuRunner(byte[] initialMemory) => emulator.Memory.Load(0, initialMemory);

    internal override void Run()
    {
        emulator.Registers.PC = StartAddress;
        emulator.Registers.SP = 0xFFFE;

        while (emulator.Registers.PC != StopAddress)
        {
            _ = emulator.Step();
        }
    }
}