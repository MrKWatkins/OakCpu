namespace MrKWatkins.OakCpu.Z80.Benchmarks.ZEXALL;

internal sealed class ContendedZ80StepEmulatorRunner : Runner
{
    // Using contendedEmulator to step, but going direct to the fields on emulator to avoid extra overhead.
    private readonly Z80StepEmulator emulator = new();
    private readonly ContendedZ80StepEmulator contendedEmulator;
    private readonly byte[] memory;

    public ContendedZ80StepEmulatorRunner(byte[] initialMemory)
    {
        contendedEmulator = new ContendedZ80StepEmulator(emulator);
        memory = initialMemory.ToArray();
    }

    internal override void Run()
    {
        emulator.PC = StartAddress;
        emulator.SP = 0xFFFE;

        while (emulator.PC != StopAddress)
        {
            Step();
        }
    }

    private void Step()
    {
        var actionRequired = contendedEmulator.Step();
        HandleActionRequired(actionRequired, emulator.Address, emulator.Data);
    }

    private void HandleActionRequired(ActionRequired actionRequired, ushort address, byte data)
    {
        switch (actionRequired)
        {
            case ActionRequired.None:
                return;

            case ActionRequired.OpcodeRead:
            case ActionRequired.MemoryRead:
                emulator.Data = memory[address];
                return;

            case ActionRequired.MemoryWrite:
                memory[address] = data;
                return;

            case ActionRequired.IoRead:
            case ActionRequired.IoWrite:
                throw new InvalidOperationException($"Unexpected I/O action {actionRequired} at 0x{address:X4}.");

            default:
                throw new NotSupportedException($"The {nameof(ActionRequired)} {actionRequired} is not supported.");
        }
    }
}
