using MrKWatkins.OakCpu.Z80.TestSuites;

namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class Z80EmulatorTestHarness : Z80TestHarness
{
    private readonly Z80Emulator emulator = new();

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

    public override void AssertFail(string message) => Assert.Fail(message + Environment.NewLine);

    public override void Step()
    {
        var actionRequired = emulator.Step();
        PerformActionRequired(actionRequired);
        MutableCycles?.Add(CreateCycle(actionRequired));
        TStates++;
    }

    public override void ExecuteInstruction(TextWriter? debug = null)
    {
        Z80Debugging.WriteDebugInformation(this, debug);
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