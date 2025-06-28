namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public abstract class Z80State
{
    public ushort RegisterAF { get; internal set; }

    public byte RegisterA => (byte)(RegisterAF >> 8);

    public byte RegisterF => (byte)(RegisterAF & 0xFF);

    public ushort RegisterBC { get; internal set; }

    public ushort RegisterDE { get; internal set; }

    public ushort RegisterHL { get; internal set; }

    public ushort ShadowRegisterAF { get; internal set; }

    public ushort ShadowRegisterBC { get; internal set; }

    public ushort ShadowRegisterDE { get; internal set; }

    public ushort ShadowRegisterHL { get; internal set; }

    public ushort RegisterIX { get; internal set; }

    public ushort RegisterIY { get; internal set; }

    public ushort RegisterSP { get; internal set; }

    public ushort RegisterPC { get; internal set; }

    public ushort RegisterWZ { get; internal set; }

    public byte RegisterI { get; internal set; }

    public byte RegisterR { get; internal set; }

    public byte RegisterQ { get; internal set; }

    public bool FlagC => (RegisterAF & 0b00000001) != 0;

    public bool FlagN => (RegisterAF & 0b00000010) != 0;

    public bool FlagPV => (RegisterAF & 0b00000100) != 0;

    public bool FlagX => (RegisterAF & 0b00001000) != 0;

    public bool FlagH => (RegisterAF & 0b00010000) != 0;

    public bool FlagY => (RegisterAF & 0b00100000) != 0;

    public bool FlagZ => (RegisterAF & 0b01000000) != 0;

    public bool FlagS => (RegisterAF & 0b10000000) != 0;

    public bool IFF1 { get; internal set; }

    public bool IFF2 { get; internal set; }

    public byte IM { get; internal set; }

    public bool Halted { get; internal set; }

    public IReadOnlyList<MemoryState> Memory { get; internal set; } = null!;
}