namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public abstract class Z80State
{
    public ushort RegisterAF { get; private protected set; }

    public ushort RegisterBC { get; private protected  set; }

    public ushort RegisterDE { get; private protected  set; }

    public ushort RegisterHL { get; private protected  set; }

    public ushort ShadowRegisterAF { get; private protected  set; }

    public ushort ShadowRegisterBC { get; private protected  set; }

    public ushort ShadowRegisterDE { get; private protected  set; }

    public ushort ShadowRegisterHL { get; private protected  set; }

    public ushort RegisterIX { get; private protected  set; }

    public ushort RegisterIY { get; private protected  set; }

    public ushort RegisterSP { get; private protected  set; }

    public ushort RegisterPC { get; private protected  set; }

    public ushort RegisterWZ { get; private protected  set; }

    public byte RegisterI { get; private protected  set; }

    public byte RegisterR { get; private protected  set; }

    public bool FlagC => (RegisterAF & 0b00000001) != 0;

    public bool FlagN => (RegisterAF & 0b00000010) != 0;

    public bool FlagPV => (RegisterAF & 0b00000100) != 0;

    public bool FlagX => (RegisterAF & 0b00001000) != 0;

    public bool FlagH => (RegisterAF & 0b00010000) != 0;

    public bool FlagY => (RegisterAF & 0b00100000) != 0;

    public bool FlagZ => (RegisterAF & 0b01000000) != 0;

    public bool FlagS => (RegisterAF & 0b10000000) != 0;

    public bool IFF1 { get; private protected  set; }

    public bool IFF2 { get; private protected  set; }

    public byte IM { get; private protected set; }

    public bool Halted { get; private protected set; }

    public IReadOnlyList<MemoryState> Memory { get; private protected set; } = null!;

    public virtual void Setup(Z80TestHarness z80)
    {
        z80.RegisterAF = RegisterAF;
        z80.RegisterBC = RegisterBC;
        z80.RegisterDE = RegisterDE;
        z80.RegisterHL = RegisterHL;
        z80.RegisterI = RegisterI;
        z80.RegisterR = RegisterR;
        z80.RegisterPC = RegisterPC;
        z80.RegisterSP = RegisterSP;
        z80.RegisterIX = RegisterIX;
        z80.RegisterIY = RegisterIY;
        z80.RegisterWZ = RegisterWZ;
        z80.ShadowRegisterAF = ShadowRegisterAF;
        z80.ShadowRegisterBC = ShadowRegisterBC;
        z80.ShadowRegisterDE = ShadowRegisterDE;
        z80.ShadowRegisterHL = ShadowRegisterHL;
        z80.IM = IM;
        z80.IFF1 = IFF1;
        z80.IFF2 = IFF2;

        foreach (var memory in Memory)
        {
            z80.WriteByteToMemory(memory.Address, memory.Value);
        }
    }

    public virtual void Assert(Assertions assertionsToRun, Z80TestHarness z80)
    {
        AssertRegisters(assertionsToRun, z80);
        AssertFlags(assertionsToRun, z80);
        AssertInterrupts(assertionsToRun, z80);
        AssertMemory(assertionsToRun, z80);
    }

    private void AssertRegisters(Assertions assertionsToRun, Z80TestHarness z80)
    {
        AssertEqual(assertionsToRun, z80, Assertions.AF, z80.RegisterAF, RegisterAF, $"Register AF should be 0x{RegisterAF:X4} but was 0x{z80.RegisterAF:X4}");
        AssertEqual(assertionsToRun, z80, Assertions.BC, z80.RegisterBC, RegisterBC, $"Register BC should be 0x{RegisterBC:X4} but was 0x{z80.RegisterBC:X4}");
        AssertEqual(assertionsToRun, z80, Assertions.DE, z80.RegisterDE, RegisterDE, $"Register DE should be 0x{RegisterDE:X4} but was 0x{z80.RegisterDE:X4}");
        AssertEqual(assertionsToRun, z80, Assertions.HL, z80.RegisterHL, RegisterHL, $"Register HL should be 0x{RegisterHL:X4} but was 0x{z80.RegisterHL:X4}");
        AssertEqual(assertionsToRun, z80, Assertions.PC, z80.RegisterPC, RegisterPC, $"Register PC should be 0x{RegisterPC:X4} but was 0x{z80.RegisterPC:X4}");
        AssertEqual(assertionsToRun, z80, Assertions.SP, z80.RegisterSP, RegisterSP, $"Register SP should be 0x{RegisterSP:X4} but was 0x{z80.RegisterSP:X4}");
        AssertEqual(assertionsToRun, z80, Assertions.IX, z80.RegisterIX, RegisterIX, $"Register IX should be 0x{RegisterIX:X4} but was 0x{z80.RegisterIX:X4}");
        AssertEqual(assertionsToRun, z80, Assertions.IY, z80.RegisterIY, RegisterIY, $"Register IY should be 0x{RegisterIY:X4} but was 0x{z80.RegisterIY:X4}");
        AssertEqual(assertionsToRun, z80, Assertions.WZ, z80.RegisterWZ, RegisterWZ, $"Register WZ should be 0x{RegisterWZ:X4} but was 0x{z80.RegisterWZ:X4}");
        AssertEqual(assertionsToRun, z80, Assertions.I, z80.RegisterI, RegisterI, $"Register I should be 0x{RegisterI:X2} but was 0x{z80.RegisterI:X2}");
        AssertEqual(assertionsToRun, z80, Assertions.R, z80.RegisterR, RegisterR, $"Register R should be 0x{RegisterAF:X2} but was 0x{z80.RegisterAF:X2}");
        AssertEqual(assertionsToRun, z80, Assertions.ShadowAF, z80.ShadowRegisterAF, ShadowRegisterAF, $"Register AF' should be 0x{ShadowRegisterAF:X4} but was 0x{z80.ShadowRegisterAF:X4}");
        AssertEqual(assertionsToRun, z80, Assertions.ShadowBC, z80.ShadowRegisterBC, ShadowRegisterBC, $"Register BC' should be 0x{ShadowRegisterBC:X4} but was 0x{z80.ShadowRegisterBC:X4}");
        AssertEqual(assertionsToRun, z80, Assertions.ShadowDE, z80.ShadowRegisterDE, ShadowRegisterDE, $"Register DE' should be 0x{ShadowRegisterDE:X4} but was 0x{z80.ShadowRegisterDE:X4}");
        AssertEqual(assertionsToRun, z80, Assertions.ShadowHL, z80.ShadowRegisterHL, ShadowRegisterHL, $"Register HL' should be 0x{ShadowRegisterHL:X4} but was 0x{z80.ShadowRegisterHL:X4}");
    }

    private void AssertFlags(Assertions assertionsToRun, Z80TestHarness z80)
    {
        AssertEqual(assertionsToRun, z80, Assertions.C, z80.FlagC, FlagC, $"Flag C should should be {FormatFlag(FlagC)} but was {FormatFlag(z80.FlagC)}");
        AssertEqual(assertionsToRun, z80, Assertions.N, z80.FlagN, FlagN, $"Flag N should be {FormatFlag(FlagN)} but was {FormatFlag(z80.FlagN)}");
        AssertEqual(assertionsToRun, z80, Assertions.PV, z80.FlagPV, FlagPV, $"Flag PV should be {FormatFlag(FlagPV)} but was {FormatFlag(z80.FlagPV)}");
        AssertEqual(assertionsToRun, z80, Assertions.X, z80.FlagX, FlagX, $"Flag X should be {FormatFlag(FlagX)} but was {FormatFlag(z80.FlagX)}");
        AssertEqual(assertionsToRun, z80, Assertions.H, z80.FlagH, FlagH, $"Flag H should be {FormatFlag(FlagH)} but was {FormatFlag(z80.FlagH)}");
        AssertEqual(assertionsToRun, z80, Assertions.Y, z80.FlagY, FlagY, $"Flag Y should be {FormatFlag(FlagY)} but was {FormatFlag(z80.FlagY)}");
        AssertEqual(assertionsToRun, z80, Assertions.Z, z80.FlagZ, FlagZ, $"Flag Z should be {FormatFlag(FlagZ)} but was {FormatFlag(z80.FlagZ)}");
        AssertEqual(assertionsToRun, z80, Assertions.S, z80.FlagS, FlagS, $"Flag S should be {FormatFlag(FlagS)} but was {FormatFlag(z80.FlagS)}");
    }

    private void AssertInterrupts(Assertions assertionsToRun, Z80TestHarness z80)
    {
        AssertEqual(assertionsToRun, z80, Assertions.IM, z80.IM, IM, $"IM should should be {IM} but was {z80.IM}");
        AssertEqual(assertionsToRun, z80, Assertions.IFF1, z80.IFF1, IFF1, $"IFF1 should should be {FormatFlag(IFF1)} but was {FormatFlag(z80.IFF1)}");
        AssertEqual(assertionsToRun, z80, Assertions.IFF2, z80.IFF2, IFF2, $"IFF2 should should be {FormatFlag(IFF2)} but was {FormatFlag(z80.IFF2)}");
        AssertEqual(assertionsToRun, z80, Assertions.Halted, z80.Halted, Halted, $"Halted should should be {Halted} but was {z80.Halted}");
    }

    private void AssertMemory(Assertions assertionsToRun, Z80TestHarness z80)
    {
        if (!assertionsToRun.HasFlag(Assertions.Memory))
        {
            return;
        }

        foreach (var memory in Memory)
        {
            var address = memory.Address;
            var expected = memory.Value;
            var actual = z80.ReadByteFromMemory(address);
            z80.AssertEqual(actual, memory.Value, $"memory at 0x{address:X4} should be 0x{expected:X2} but was 0x{actual:X2}");
        }
    }

    private static char FormatFlag(bool flag) => flag ? '1' : '0';

    protected static void AssertEqual<T>(Assertions assertionsToRun, Z80TestHarness z80, Assertions assertionsCategory, T actual, T expected, string message)
    {
        if (assertionsToRun.HasFlag(assertionsCategory))
        {
            z80.AssertEqual(actual, expected, message);
        }
    }
}