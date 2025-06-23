using MrKWatkins.OakCpu.Z80.TestSuites;

namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class Z80EmulatorTestHarness : Z80TestHarness
{
    private readonly Z80Emulator emulator = new();

    public override ushort RegisterAF
    {
        get => emulator.Registers.AF;
        set => emulator.Registers.AF = value;
    }

    public override ushort RegisterBC
    {
        get => emulator.Registers.BC;
        set => emulator.Registers.BC = value;
    }

    public override ushort RegisterDE
    {
        get => emulator.Registers.DE;
        set => emulator.Registers.DE = value;
    }

    public override ushort RegisterHL
    {
        get => emulator.Registers.HL;
        set => emulator.Registers.HL = value;
    }

    public override ushort RegisterIX
    {
        get => emulator.Registers.IX;
        set => emulator.Registers.IX = value;
    }

    public override ushort RegisterIY
    {
        get => emulator.Registers.IY;
        set => emulator.Registers.IY = value;
    }

    public override ushort RegisterSP
    {
        get => emulator.Registers.SP;
        set => emulator.Registers.SP = value;
    }

    public override ushort RegisterPC
    {
        get => emulator.Registers.PC;
        set => emulator.Registers.PC = value;
    }

    public override ushort RegisterWZ
    {
        get => emulator.Registers.WZ;
        set => emulator.Registers.WZ = value;
    }

    public override byte RegisterI
    {
        get => emulator.Registers.I;
        set => emulator.Registers.I = value;
    }

    public override byte RegisterR
    {
        get => emulator.Registers.R;
        set => emulator.Registers.R = value;
    }

    public override byte RegisterQ
    {
        get => emulator.Registers.Q;
        set => emulator.Registers.Q = value;
    }

    public override ushort ShadowRegisterAF
    {
        get => emulator.Registers.Shadow.AF;
        set => emulator.Registers.Shadow.AF = value;
    }

    public override ushort ShadowRegisterBC
    {
        get => emulator.Registers.Shadow.BC;
        set => emulator.Registers.Shadow.BC = value;
    }

    public override ushort ShadowRegisterDE
    {
        get => emulator.Registers.Shadow.DE;
        set => emulator.Registers.Shadow.DE = value;
    }

    public override ushort ShadowRegisterHL
    {
        get => emulator.Registers.Shadow.HL;
        set => emulator.Registers.Shadow.HL = value;
    }

    public override bool FlagC
    {
        get => emulator.Flags.C;
        set => emulator.Flags.C = value;
    }

    public override bool FlagN
    {
        get => emulator.Flags.N;
        set => emulator.Flags.N = value;
    }

    public override bool FlagPV
    {
        get => emulator.Flags.PV;
        set => emulator.Flags.PV = value;
    }

    public override bool FlagX
    {
        get => emulator.Flags.X;
        set => emulator.Flags.X = value;
    }

    public override bool FlagH
    {
        get => emulator.Flags.H;
        set => emulator.Flags.H = value;
    }

    public override bool FlagY
    {
        get => emulator.Flags.Y;
        set => emulator.Flags.Y = value;
    }

    public override bool FlagZ
    {
        get => emulator.Flags.Z;
        set => emulator.Flags.Z = value;
    }

    public override bool FlagS
    {
        get => emulator.Flags.S;
        set => emulator.Flags.S = value;
    }

    public override bool IFF1
    {
        get => emulator.Interrupts.IFF1;
        set => emulator.Interrupts.IFF1 = value;
    }

    public override bool IFF2
    {
        get => emulator.Interrupts.IFF2;
        set => emulator.Interrupts.IFF2 = value;
    }

    public override byte IM
    {
        get => emulator.Interrupts.IM;
        set => emulator.Interrupts.IM = value;
    }

    public override bool Halted
    {
        get => emulator.Interrupts.Halted;
        set => emulator.Interrupts.Halted = value;
    }

    public override bool Interrupt
    {
        get => emulator.Interrupts.Interrupt;
        set => emulator.Interrupts.Interrupt = value;
    }

    public override void AssertFail(string message) => Assert.Fail(message + Environment.NewLine);

    public override void Step()
    {
        var actionRequired = emulator.Step();
        PerformActionRequired(actionRequired);
        MutableCycles?.Add(CreateCycle(actionRequired));
        TStates++;
    }

    public override void ExecuteInstruction()
    {
        emulator.InstructionComplete = false;
        while (!emulator.InstructionComplete)
        {
            Step();
        }
    }

    private void PerformActionRequired(ActionRequired actionRequired)
    {
        switch (actionRequired)
        {
            case ActionRequired.OpcodeRead:
            case ActionRequired.MemoryRead:
                emulator.Data = ReadByteFromMemory(emulator.Address);
                return;

            case ActionRequired.MemoryWrite:
                WriteByteToMemory(emulator.Address, emulator.Data);
                return;

            case ActionRequired.IoRead:
                emulator.Data = IOReader.Read(emulator.Address);
                return;

            case ActionRequired.IoWrite:
                IOWriter.Write(emulator.Address, emulator.Data);
                return;
        }
    }

    [Pure]
    private Cycle CreateCycle(ActionRequired actionRequired)
    {
        switch (actionRequired)
        {
            case ActionRequired.None:
                return new Cycle(CycleType.None, TStates, emulator.Address, emulator.Data);

            case ActionRequired.OpcodeRead:
                return new Cycle(CycleType.MemoryRead, TStates, emulator.Address, emulator.Data, true);

            case ActionRequired.MemoryRead:
                return new Cycle(CycleType.MemoryRead, TStates, emulator.Address, emulator.Data);

            case ActionRequired.MemoryWrite:
                return new Cycle(CycleType.MemoryWrite, TStates, emulator.Address, emulator.Data);

            case ActionRequired.IoRead:
                return new Cycle(CycleType.IORead, TStates, emulator.Address, emulator.Data);

            case ActionRequired.IoWrite:
                return new Cycle(CycleType.IOWrite, TStates, emulator.Address, emulator.Data);
        }
        throw new NotSupportedException($"The {nameof(ActionRequired)} {actionRequired} is not supported.");
    }
}