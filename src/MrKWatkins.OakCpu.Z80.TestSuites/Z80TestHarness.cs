using System.Runtime.CompilerServices;

namespace MrKWatkins.OakCpu.Z80.TestSuites;

[SuppressMessage("ReSharper", "InconsistentNaming")]
#pragma warning disable CA1001
public abstract class Z80TestHarness
#pragma warning restore CA1001
{
    private readonly byte[] memory = new byte[65536];
    private AssertionScope? assertionScope;

    public abstract ushort RegisterAF { get; set; }

    public byte RegisterA
    {
        get => GetLowByte(RegisterAF);
        set => RegisterAF = SetLowByte(RegisterAF, value);
    }

    public byte RegisterF
    {
        get => GetHighByte(RegisterAF);
        set => RegisterAF = SetHighByte(RegisterAF, value);
    }

    public abstract ushort RegisterBC { get; set; }

    public byte RegisterB
    {
        get => GetLowByte(RegisterBC);
        set => RegisterBC = SetLowByte(RegisterBC, value);
    }

    public byte RegisterC
    {
        get => GetHighByte(RegisterBC);
        set => RegisterBC = SetHighByte(RegisterBC, value);
    }

    public abstract ushort RegisterDE { get; set; }

    public byte RegisterD
    {
        get => GetLowByte(RegisterDE);
        set => RegisterDE = SetLowByte(RegisterDE, value);
    }

    public byte RegisterE
    {
        get => GetHighByte(RegisterDE);
        set => RegisterDE = SetHighByte(RegisterDE, value);
    }

    public abstract ushort RegisterHL { get; set; }

    public byte RegisterH
    {
        get => GetLowByte(RegisterHL);
        set => RegisterHL = SetLowByte(RegisterHL, value);
    }

    public byte RegisterL
    {
        get => GetHighByte(RegisterHL);
        set => RegisterHL = SetHighByte(RegisterHL, value);
    }

    public abstract ushort RegisterIX { get; set; }

    public abstract ushort RegisterIY { get; set; }

    public abstract ushort RegisterSP { get; set; }

    public abstract ushort RegisterPC { get; set; }

    public abstract ushort RegisterWZ { get; set; }

    public abstract byte RegisterI { get; set; }

    public abstract byte RegisterR { get; set; }

    public abstract byte RegisterQ { get; set; }

    public abstract ushort ShadowRegisterAF { get; set; }

    public abstract ushort ShadowRegisterBC { get; set; }

    public abstract ushort ShadowRegisterDE { get; set; }

    public abstract ushort ShadowRegisterHL { get; set; }

    public abstract bool FlagC { get; set; }

    public abstract bool FlagN { get; set; }

    public abstract bool FlagPV { get; set; }

    public abstract bool FlagX { get; set; }

    public abstract bool FlagH { get; set; }

    public abstract bool FlagY { get; set; }

    public abstract bool FlagZ { get; set; }

    public abstract bool FlagS { get; set; }

    public abstract bool IFF1 { get; set; }

    public abstract bool IFF2 { get; set; }

    public abstract byte IM { get; set; }

    public abstract bool Halted { get; set; }

    public abstract bool Interrupt { get; set; }

    public ulong TStates
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set;
    }

    [OverloadResolutionPriority(1)]
    public void CopyIntoMemory(ushort address, ReadOnlySpan<byte> source) => source.CopyTo(memory.AsSpan(address));

    public void CopyIntoMemory(ushort address, IReadOnlyList<byte> source)
    {
        foreach (var @byte in source)
        {
            memory[address] = @byte;
            address++;
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByteFromMemory(ushort address) => memory[address];

    [Pure]
    public ushort ReadWordFromMemory(ushort address)
    {
        // Read the two bytes separately and assemble rather than use something like BinaryPrimitives.ReadUInt16LittleEndian.
        // This enables us to cope with wraparound from 0xFFFF -> 0x0000 by using the overflow on ushort.
        var lsb = ReadByteFromMemory(address);

        address++;
        var msb = ReadByteFromMemory(address);

        return (ushort)((msb << 8) | lsb);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void WriteByteToMemory(ushort address, byte value) => memory[address] = value;

    public void WriteWordToMemory(ushort address, ushort value)
    {
        WriteByteToMemory(address, (byte)value);

        address++;
        WriteByteToMemory(address, (byte)(value >> 8));
    }

    public IIOReader IOReader { get; set; } = new NullIO();

    public IIOWriter IOWriter { get; set; } = new NullIO();

    public void SetIO<TIO>(TIO io)
        where TIO : IIOReader, IIOWriter
    {
        IOReader = io;
        IOWriter = io;
    }

    public bool RecordCycles
    {
        get => MutableCycles != null;
        set
        {
            if (value)
            {
                MutableCycles ??= [];
            }
            else
            {
                MutableCycles = null;
            }
        }
    }

    protected internal List<Cycle>? MutableCycles { get; private set; }

    public IReadOnlyList<Cycle> Cycles => MutableCycles ?? throw new InvalidOperationException("Cycles are not being recorded.");

    [MustDisposeResource]
    public IDisposable CreateAssertionScope(string? name = null)
    {
        if (assertionScope != null)
        {
            throw new InvalidOperationException("Cannot create a nested assertion scope.");
        }

        assertionScope = new AssertionScope(this, name);
        return assertionScope;
    }

    public void AssertEqual<T>(T actual, T expected, DefaultInterpolatedStringHandler message)
    {
        if (!EqualityComparer<T>.Default.Equals(actual, expected))
        {
            if (assertionScope != null)
            {
                assertionScope.AddError(message.ToString());
            }
            else
            {
                AssertFail(message.ToString());
            }
        }
    }

    public abstract void AssertFail(string message);

    public void Step(ulong tStates)
    {
        while (TStates <= tStates)
        {
            Step();
        }
    }

    public abstract void Step();

    public abstract void ExecuteInstruction(TextWriter? debug = null);

    [Pure]
    private static byte GetLowByte(ushort value) => (byte)(value >> 8); // Little endian, so the lowest byte is first in memory, i.e. the first byte in the short.

    [Pure]
    private static byte GetHighByte(ushort value) => (byte)(value & 0xFF);

    [Pure]
    private static ushort SetLowByte(ushort value, byte lowByte) => (ushort)((value & 0x00FF) | (lowByte << 8));

    [Pure]
    private static ushort SetHighByte(ushort value, byte highByte) => (ushort)((value & 0xFF00) | highByte);

    private sealed class AssertionScope(Z80TestHarness z80, string? name) : IDisposable
    {
        private readonly List<string> errors = new();

        public void AddError(string error) => errors.Add(error);

        public void Dispose()
        {
            if (errors.Any())
            {
                var prefix = name != null ? $"{name} failed:{Environment.NewLine}{Environment.NewLine}" : "";

                z80.AssertFail(prefix + string.Join(Environment.NewLine, errors) + Environment.NewLine);
            }
        }
    }
}