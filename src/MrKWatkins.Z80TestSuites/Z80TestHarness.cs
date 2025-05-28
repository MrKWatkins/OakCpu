namespace MrKWatkins.Z80TestSuites;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class Z80TestHarness
{
    public abstract ushort AF { get; set; }

    public abstract ushort BC { get; set; }

    public abstract ushort DE { get; set; }

    public abstract ushort HL { get; set; }

    public abstract ushort IX { get; set; }

    public abstract ushort IY { get; set; }

    public abstract ushort SP { get; set; }

    public abstract ushort PC { get; set; }

    public abstract ushort WZ { get; set; }

    public abstract byte I { get; set; }

    public abstract byte R { get; set; }

    public abstract ushort ShadowAF { get; set; }

    public abstract ushort ShadowBC { get; set; }

    public abstract ushort ShadowDE { get; set; }

    public abstract ushort ShadowHL { get; set; }

    public abstract bool IFF1 { get; set; }

    public abstract bool IFF2 { get; set; }

    public abstract byte IM { get; set; }

    public abstract bool IsHalted { get; set; }

    public int TStates { get; protected set; }

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