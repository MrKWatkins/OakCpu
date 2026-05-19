using MrKWatkins.EmulatorTestSuites.M6502;
using NUnit.Framework;

namespace MrKWatkins.OakCpu.M6502.Testing;

public sealed class M6502InstructionEmulatorTestHarness : M6502TestHarness
{
    private readonly M6502InstructionEmulator emulator;
    private readonly byte[] memory = new byte[65536];
    private readonly Action<ActionRequired, ushort, byte> performActionRequiredCallback;

    public M6502InstructionEmulatorTestHarness()
        : this(new M6502InstructionEmulator())
    {
    }
    private M6502InstructionEmulatorTestHarness(M6502InstructionEmulator emulator)
    {
        this.emulator = emulator;
        performActionRequiredCallback = PerformActionRequired;
    }

    public override byte RegisterA
    {
        get => emulator.A;
        set => emulator.A = value;
    }

    public override byte RegisterX
    {
        get => emulator.X;
        set => emulator.X = value;
    }

    public override byte RegisterY
    {
        get => emulator.Y;
        set => emulator.Y = value;
    }

    public override byte RegisterS
    {
        get => emulator.S;
        set => emulator.S = value;
    }

    public override byte RegisterP
    {
        get => (byte)(emulator.P | 0x20);
        set => emulator.P = (byte)(value | 0x20);
    }

    public override ushort RegisterPC
    {
        get => emulator.PC;
        set => emulator.PC = value;
    }

    public override byte ReadByteFromMemory(ushort address) => memory[address];

    public override void WriteByteToMemory(ushort address, byte value) => memory[address] = value;

    public override void Reset()
    {
        base.Reset();
        emulator.Reset();
        emulator.P = 0x20;
    }

    public override void AssertFail(string message) => Assert.Fail(message + Environment.NewLine);

    public override void ExecuteInstruction()
    {
        var opcode = ReadByteFromMemory(emulator.PC);
        var initialTStates = TStates;
        var executedTStates = emulator.ExecuteInstruction(performActionRequiredCallback);
        if (opcode == 0xEA && executedTStates == 2 && TStates == initialTStates + 1)
        {
            // The instruction emulator does not expose NOP's overlapped prefetch cycle, but the single-step suite expects it.
            emulator.Data = ReadByteFromMemory(emulator.PC);
            MutableCycles?.Add(new Cycle(CycleType.Read, TStates, emulator.PC, emulator.Data));
            TStates++;
        }
    }

    private void PerformActionRequired(ActionRequired actionRequired, ushort address, byte data)
    {
        switch (actionRequired)
        {
            case ActionRequired.OpcodeRead:
            case ActionRequired.MemoryRead:
                emulator.Data = ReadByteFromMemory(address);
                data = emulator.Data;
                MutableCycles?.Add(new Cycle(CycleType.Read, TStates, address, data));
                TStates++;
                return;

            case ActionRequired.MemoryWrite:
                WriteByteToMemory(address, data);
                MutableCycles?.Add(new Cycle(CycleType.Write, TStates, address, data));
                TStates++;
                return;
        }

        throw new NotSupportedException($"The {nameof(ActionRequired)} {actionRequired} is not supported.");
    }
}