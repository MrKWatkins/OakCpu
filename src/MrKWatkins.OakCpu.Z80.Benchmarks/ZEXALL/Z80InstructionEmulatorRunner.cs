using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MrKWatkins.OakCpu.Z80.Benchmarks.ZEXALL;

internal sealed class Z80InstructionEmulatorRunner : Runner
{
    private readonly Z80InstructionEmulator emulator = new();
    private readonly byte[] memory;

    internal Z80InstructionEmulatorRunner(byte[] initialMemory)
    {
        memory = initialMemory.ToArray();
    }

    internal override void Run()
    {
        emulator.PC = StartAddress;
        emulator.SP = 0xFFFE;

        var handler = new BusHandler(emulator, memory);
        while (emulator.PC != StopAddress)
        {
            _ = emulator.ExecuteInstruction(ref handler);
        }
    }

    private readonly struct BusHandler(Z80InstructionEmulator emulator, byte[] memory) : IZ80BusHandler
    {
        public void OnActionRequired(ActionRequired actionRequired, ushort address, byte data)
        {
            switch (actionRequired)
            {
                case ActionRequired.OpcodeRead:
                case ActionRequired.MemoryRead:
                    emulator.Data = Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(memory), address);
                    return;

                case ActionRequired.MemoryWrite:
                    Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(memory), address) = data;
                    return;

                case ActionRequired.IORead:
                case ActionRequired.IOWrite:
                    throw new InvalidOperationException($"Unexpected I/O action {actionRequired} at 0x{address:X4}.");

                default:
                    throw new NotSupportedException($"The {nameof(ActionRequired)} {actionRequired} is not supported.");
            }
        }
    }
}