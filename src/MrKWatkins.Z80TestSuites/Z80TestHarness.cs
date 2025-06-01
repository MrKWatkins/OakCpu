namespace MrKWatkins.Z80TestSuites;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class Z80TestHarness
{
    private readonly List<TestEvent> events = new();

    public abstract ushort RegisterAF { get; set; }

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

    protected void AddEvent(TestEvent @event) => events.Add(@event);

    protected void RemoveLastEvent() => events.RemoveAt(events.Count - 1);

    [Pure]
    public abstract byte GetMemory(ushort address);

    public abstract void SetMemory(ushort address, byte value);

    [MustDisposeResource]
    public virtual IDisposable CreateAssertionScope() => NullDisposable.Instance;

    public abstract void AssertEqual<T>(T actual, T expected, string? message = null);

    public abstract void ExecuteInstruction();

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