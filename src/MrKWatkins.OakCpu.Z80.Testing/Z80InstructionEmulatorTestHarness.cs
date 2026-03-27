using MrKWatkins.EmulatorTestSuites.Z80;
using NUnit.Framework;

namespace MrKWatkins.OakCpu.Z80.Testing;

public sealed class Z80InstructionEmulatorTestHarness : Z80TestHarness
{
    private readonly Z80InstructionEmulator emulator;
    private readonly byte[] memory = new byte[65536];
    private readonly Action<ActionRequired, ushort, byte> performActionRequiredCallback;

    public Z80InstructionEmulatorTestHarness()
        : this(new Z80InstructionEmulator())
    {
    }

    private Z80InstructionEmulatorTestHarness(Z80InstructionEmulator emulator)
    {
        this.emulator = emulator;
        performActionRequiredCallback = PerformActionRequired;
    }

    public override ushort RegisterAF
    {
        get => emulator.AF;
        set => emulator.AF = value;
    }

    public override ushort RegisterBC
    {
        get => emulator.BC;
        set => emulator.BC = value;
    }

    public override ushort RegisterDE
    {
        get => emulator.DE;
        set => emulator.DE = value;
    }

    public override ushort RegisterHL
    {
        get => emulator.HL;
        set => emulator.HL = value;
    }

    public override ushort RegisterIX
    {
        get => emulator.IX;
        set => emulator.IX = value;
    }

    public override ushort RegisterIY
    {
        get => emulator.IY;
        set => emulator.IY = value;
    }

    public override ushort RegisterSP
    {
        get => emulator.SP;
        set => emulator.SP = value;
    }

    public override ushort RegisterPC
    {
        get => emulator.PC;
        set => emulator.PC = value;
    }

    public override ushort RegisterWZ
    {
        get => emulator.WZ;
        set => emulator.WZ = value;
    }

    public override byte RegisterI
    {
        get => emulator.I;
        set => emulator.I = value;
    }

    public override byte RegisterR
    {
        get => emulator.R;
        set => emulator.R = value;
    }

    public override byte RegisterQ
    {
        get => emulator.Q;
        set => emulator.Q = value;
    }

    public override ushort ShadowRegisterAF
    {
        get => emulator.Shadow_AF;
        set => emulator.Shadow_AF = value;
    }

    public override ushort ShadowRegisterBC
    {
        get => emulator.Shadow_BC;
        set => emulator.Shadow_BC = value;
    }

    public override ushort ShadowRegisterDE
    {
        get => emulator.Shadow_DE;
        set => emulator.Shadow_DE = value;
    }

    public override ushort ShadowRegisterHL
    {
        get => emulator.Shadow_HL;
        set => emulator.Shadow_HL = value;
    }

    public override bool IFF1
    {
        get => emulator.iff1;
        set => emulator.iff1 = value;
    }

    public override bool IFF2
    {
        get => emulator.iff2;
        set => emulator.iff2 = value;
    }

    public override byte IM
    {
        get => emulator.im;
        set => emulator.im = value;
    }

    public override bool Halted
    {
        get => emulator.halted;
        set => emulator.halted = value;
    }

    public override bool Interrupt
    {
        get => emulator.interrupt;
        set => emulator.interrupt = value;
    }

    public override byte ReadByteFromMemory(ushort address) => memory[address];

    public override void WriteByteToMemory(ushort address, byte value) => memory[address] = value;

    public override void Reset()
    {
        base.Reset();
        emulator.Reset();
    }

    public override void AssertFail(string message) => Assert.Fail(message + Environment.NewLine);

    public int LastInstructionTStates { get; private set; }

    public override void ExecuteInstruction()
    {
        var instructionStartTStates = TStates;
        LastInstructionTStates = emulator.ExecuteInstruction(performActionRequiredCallback);
        TStates = instructionStartTStates + (ulong)LastInstructionTStates;
    }

    private void PerformActionRequired(ActionRequired actionRequired, ushort address, byte data)
    {
        switch (actionRequired)
        {
            case ActionRequired.OpcodeRead:
            case ActionRequired.MemoryRead:
                emulator.Data = ReadByteFromMemory(address);
                data = emulator.Data;
                break;

            case ActionRequired.MemoryWrite:
                if (RomArea == null || address < RomArea.Value.Start || address > RomArea.Value.End)
                {
                    WriteByteToMemory(address, data);
                }
                break;

            case ActionRequired.IoRead:
                emulator.Data = IOReader.Read(address);
                data = emulator.Data;
                break;

            case ActionRequired.IoWrite:
                IOWriter.Write(address, data);
                break;
        }

        MutableCycles?.Add(CreateCycle(actionRequired, address, data));
        TStates++;
    }

    [Pure]
    private Cycle CreateCycle(ActionRequired actionRequired, ushort address, byte data)
    {
        switch (actionRequired)
        {
            case ActionRequired.OpcodeRead:
                return new Cycle(CycleType.MemoryRead, TStates, address, data, true);

            case ActionRequired.MemoryRead:
                return new Cycle(CycleType.MemoryRead, TStates, address, data);

            case ActionRequired.MemoryWrite:
                return new Cycle(CycleType.MemoryWrite, TStates, address, data);

            case ActionRequired.IoRead:
                return new Cycle(CycleType.IORead, TStates, address, data);

            case ActionRequired.IoWrite:
                return new Cycle(CycleType.IOWrite, TStates, address, data);
        }

        throw new NotSupportedException($"The {nameof(ActionRequired)} {actionRequired} is not supported.");
    }
}
