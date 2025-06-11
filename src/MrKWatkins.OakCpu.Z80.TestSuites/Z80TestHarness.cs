using System.Diagnostics;

namespace MrKWatkins.OakCpu.Z80.TestSuites;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class Z80TestHarness
{
    private readonly List<TestEvent> events = new();
    private readonly byte[] memory = new byte[65536];
    private readonly List<IOWrite> ioWrites = new();

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

    public abstract ushort RegisterDE { get; set; }

    public abstract ushort RegisterHL { get; set; }

    public abstract ushort RegisterIX { get; set; }

    public abstract ushort RegisterIY { get; set; }

    public abstract ushort RegisterSP { get; set; }

    public abstract ushort RegisterPC { get; set; }

    public abstract ushort RegisterWZ { get; set; }

    public abstract byte RegisterI { get; set; }

    public abstract byte RegisterR { get; set; }

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

    public abstract bool IsHalted { get; set; }

    public int TStates { get; protected set; }

    public IReadOnlyList<TestEvent> Events => events;

    protected void AddEvent(TestEvent fuseEvent) => events.Add(fuseEvent);

    protected void AddEvents([InstantHandle] IEnumerable<TestEvent> fuseEvents) => events.AddRange(fuseEvents);

    public void RemoveLastEvent() => events.RemoveAt(events.Count - 1);

    [Pure]
    public byte ReadMemory(ushort address) => memory[address];

    public virtual void WriteMemory(ushort address, byte value) => memory[address] = value;

    public virtual byte? DefaultIORead { get; set; }

    public Queue<IORead> ExpectedIOReads { get; } = new();

    [Pure]
    public byte ReadIO(ushort port)
    {
        if (DefaultIORead.HasValue)
        {
            return DefaultIORead.Value;
        }

        if (ExpectedIOReads.TryDequeue(out var read))
        {
            AssertEqual(port, read.Port, $"expected read of port {port}.");
            return read.Data;
        }

        AssertFail($"Unexpected IO read of port {port}.");
        throw new UnreachableException();
    }

    public IReadOnlyList<IOWrite> IOWrites => ioWrites;

    public void WriteIO(ushort port, byte value)
    {
        ioWrites.Add(new IOWrite(port, value));
    }

    [MustDisposeResource]
    public virtual IDisposable CreateAssertionScope() => NullDisposable.Instance;

    public abstract void AssertEqual<T>(T actual, T expected, string? message = null);

    public abstract void AssertFail(string message);

    public abstract void ExecuteStep();

    public abstract void ExecuteInstruction();

    [Pure]
    private static byte GetLowByte(ushort value) => (byte)(value >> 8); // Little endian, so the lowest byte is first in memory, i.e. the first byte in the short.

    [Pure]
    private static byte GetHighByte(ushort value) => (byte)(value & 0xFF);

    [Pure]
    private static ushort SetLowByte(ushort value, byte lowByte) => (ushort)((value & 0x00FF) | (lowByte << 8));

    [Pure]
    private static ushort SetHighByte(ushort value, byte highByte) => (ushort)((value & 0xFF00) | highByte);

    private sealed class NullDisposable : IDisposable
    {
        public static readonly NullDisposable Instance = new();

        private NullDisposable()
        {
        }

        public void Dispose()
        {
        }
    }
}