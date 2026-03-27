namespace MrKWatkins.OakCpu.Z80.Benchmarks.ZEXALL;

internal sealed class Z80InstructionEmulatorRunner : Runner
{
    private readonly Z80InstructionEmulator emulator = new();
    private readonly byte[] memory;
    private readonly Action<ActionRequired, ushort, byte> onActionRequired;

    internal Z80InstructionEmulatorRunner(byte[] initialMemory)
    {
        memory = initialMemory.ToArray();
        onActionRequired = HandleActionRequired;
    }

    internal override void Run()
    {
        emulator.PC = StartAddress;
        emulator.SP = 0xFFFE;

        while (emulator.PC != StopAddress)
        {
            _ = emulator.ExecuteInstruction(onActionRequired);
        }
    }

    private void HandleActionRequired(ActionRequired actionRequired, ushort address, byte data)
    {
        switch (actionRequired)
        {
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
