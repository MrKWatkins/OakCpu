using MrKWatkins.EmulatorTestSuites.M6502;
using NUnit.Framework;

namespace MrKWatkins.OakCpu.M6502.Testing;

public sealed class M6502StepEmulatorTestHarness(M6502StepEmulator emulator) : M6502TestHarness
{
    private readonly byte[] memory = new byte[65536];

    public M6502StepEmulatorTestHarness()
        : this(new M6502StepEmulator())
    {
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

    public override void ExecuteInstruction() => emulator.ExecuteInstruction(PerformActionRequired);

    private void PerformActionRequired(ActionRequired actionRequired)
    {
        switch (actionRequired)
        {
            case ActionRequired.None:
                return;

            case ActionRequired.OpcodeRead:
            case ActionRequired.MemoryRead:
                emulator.Data = ReadByteFromMemory(emulator.Address);
                MutableCycles?.Add(new Cycle(CycleType.Read, TStates, emulator.Address, emulator.Data));
                TStates++;
                return;

            case ActionRequired.MemoryWrite:
                WriteByteToMemory(emulator.Address, emulator.Data);
                MutableCycles?.Add(new Cycle(CycleType.Write, TStates, emulator.Address, emulator.Data));
                TStates++;
                return;
        }

        throw new NotSupportedException($"The {nameof(ActionRequired)} {actionRequired} is not supported.");
    }
}