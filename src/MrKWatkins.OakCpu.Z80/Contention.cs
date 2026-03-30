using System.Runtime.CompilerServices;
namespace MrKWatkins.OakCpu.Z80;

/// <summary>
/// Applies ZX Spectrum 48K ULA contention to a <see cref="Z80StepEmulator" />. Memory and I/O accesses to contended
/// addresses are delayed based on the current position in the frame. Frame-position tracking is advanced one external
/// T-state at a time so wrappers can assert the INT line before any kind of wrapped step, including contention delay.
/// </summary>
/// <remarks>
/// Each bus cycle type has a known number of T-states: OpcodeRead (M1) = 4, MemoryRead/Write = 3, IoRead/Write = 4.
/// Contention is checked once at the start of each bus cycle. The remaining T-states in the cycle (reported as
/// <see cref="ActionRequired.None" /> by the emulator) are continuation steps that should not be separately contended.
/// Any <see cref="ActionRequired.None" /> steps beyond the continuation period are internal 1-T-state machine cycles
/// that each get their own contention check based on the address currently on the bus.
/// <para>
/// I/O cycles are 4 T-states but split across two types of step: a None step (T1, which sets the port address)
/// followed by IoRead/IoWrite (T2), then two continuation None steps (T3, T4). The contention for the entire
/// I/O cycle is calculated in <see cref="CalculateIoContention"/> starting from T1's frame position.
/// Any spurious memory contention added at the T1 None step is undone.
/// </para>
/// </remarks>
internal sealed class Contention
{
    // ReSharper disable once InconsistentNaming
    private const int TStatesPerFrame = ContentionTable.TStatesPerFrame;

    private readonly ContentionTable contentionTable;

    /// <summary>
    /// Number of continuation None steps remaining for the current bus cycle. When a bus cycle starts
    /// (OpcodeRead, MemoryRead, etc.), this is set to the number of subsequent None steps that are part
    /// of that cycle and should not be separately contended. It is decremented for each None step.
    /// </summary>
    private byte skipCount;

    /// <summary>
    /// The contention delay that was applied by the most recent non-skipped None step. Used by
    /// <see cref="CalculateIoContention"/> to undo spurious memory contention from the I/O cycle's T1 step.
    /// </summary>
    private byte prevNoneDelay;

    /// <summary>
    /// Creates a contention tracker at the start of a frame using either the early- or late-interrupt timing table.
    /// </summary>
    public Contention(bool earlyTimings = true)
        : this(0, earlyTimings)
    {
    }

    internal Contention(int tStatesInCurrentFrame, bool earlyTimings)
    {
        ValidateTStatesInCurrentFrame(tStatesInCurrentFrame);

        contentionTable = earlyTimings ? ContentionTable.EarlyTimings : ContentionTable.LateTimings;
        TStatesInCurrentFrame = tStatesInCurrentFrame;
    }

    // ReSharper disable once InconsistentNaming
    public int TStatesInCurrentFrame { get; private set; }

    public bool IsEarlyTimings => ReferenceEquals(contentionTable, ContentionTable.EarlyTimings);

    /// <summary>
    /// Resets the tracker to the start of a frame and clears any in-flight cycle bookkeeping.
    /// </summary>
    internal void StartFrame()
    {
        TStatesInCurrentFrame = 0;
        skipCount = 0;
        prevNoneDelay = 0;
    }

    /// <summary>
    /// Writes the transient contention state so a wrapper can resume mid-frame without losing delay bookkeeping.
    /// </summary>
    internal void Serialize(BinaryWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);

        writer.Write(TStatesInCurrentFrame);
        writer.Write(skipCount);
        writer.Write(prevNoneDelay);
    }

    /// <summary>
    /// Restores the transient contention state previously written by <see cref="Serialize"/>.
    /// </summary>
    internal void Restore(BinaryReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var tStatesInCurrentFrame = reader.ReadInt32();
        ValidateTStatesInCurrentFrame(tStatesInCurrentFrame);

        TStatesInCurrentFrame = tStatesInCurrentFrame;
        skipCount = reader.ReadByte();
        prevNoneDelay = reader.ReadByte();
    }

    /// <summary>
    /// Returns the number of extra T-states (contention delay) to add for the given step.
    ///
    /// The caller must save <c>emulator.Address</c> before calling <c>Step()</c> and pass it as
    /// <paramref name="preStepAddress"/>; this is the address that was on the bus at the start of the step,
    /// before any handler modified it.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CalculateDelay(ActionRequired actionRequired, ushort address, ushort preStepAddress)
    {
        // None is the common case, and most None steps are either bus-cycle continuations or hit
        // uncontended addresses, so fast-path those before touching the less common external cycles.
        if (actionRequired == ActionRequired.None)
        {
            if (skipCount != 0)
            {
                skipCount--;
                prevNoneDelay = 0;
                return 0;
            }

            // Use the pre-step address for internal cycles. Handlers may update Address to prepare for
            // the next bus cycle (e.g. OUTI sets Address=BC before IoWrite), but the ULA sees the
            // address that was on the bus at the start of the step.
            if (!IsContendedAddress(preStepAddress))
            {
                prevNoneDelay = 0;
                return 0;
            }

            var delay = contentionTable.GetContentionAt(TStatesInCurrentFrame);
            prevNoneDelay = delay;
            return delay;
        }

        if (actionRequired == ActionRequired.OpcodeRead)
        {
            // M1 cycle is 4 T-states: 1 OpcodeRead + 3 None continuation steps.
            skipCount = 3;
            prevNoneDelay = 0;
            return IsContendedAddress(address) ? contentionTable.GetContentionAt(TStatesInCurrentFrame) : 0;
        }

        if (actionRequired is ActionRequired.MemoryRead or ActionRequired.MemoryWrite)
        {
            // Memory read/write cycle is 3 T-states: 1 MemoryRead/Write + 2 None continuation steps.
            skipCount = 2;
            prevNoneDelay = 0;
            return IsContendedAddress(address) ? contentionTable.GetContentionAt(TStatesInCurrentFrame) : 0;
        }

        return actionRequired is ActionRequired.IoRead or ActionRequired.IoWrite ? CalculateIoContention(address) : 0;
    }

    /// <summary>
    /// Advances external time by the supplied number of T-states and wraps the frame position as needed.
    /// </summary>
    public void Elapse(int tStates = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(tStates);

        if (tStates == 0)
        {
            return;
        }

        var nextTStatesInCurrentFrame = TStatesInCurrentFrame + tStates;
        if (nextTStatesInCurrentFrame < TStatesPerFrame)
        {
            TStatesInCurrentFrame = nextTStatesInCurrentFrame;
            return;
        }

        TStatesInCurrentFrame = nextTStatesInCurrentFrame - TStatesPerFrame;
    }

    /// <summary>
    /// Recomputes the full four-T-state I/O cycle contention from T1, undoing any provisional None-step delay first.
    /// </summary>
    private int CalculateIoContention(ushort port)
    {
        // The I/O cycle in the emulator is 4 steps: None (T1, sets port address) + IoRead/IoWrite (T2)
        // + 2 continuation None steps (T3, T4). The preceding None step (T1) may have added spurious
        // memory contention at the wrong address; undo it and recalculate from T1's frame position.
        TStatesInCurrentFrame = WrapSubtract(TStatesInCurrentFrame, prevNoneDelay);
        var posT1 = WrapSubtract(TStatesInCurrentFrame, 1);

        // 2 continuation steps remain after this IoRead/IoWrite (T3, T4).
        skipCount = 2;
        prevNoneDelay = 0;

        var highByteContended = IsContendedAddress(port);
        var isUlaPort = (port & 1) == 0;

        // N:4 - no contention at all.
        if (!highByteContended && !isUlaPort)
        {
            return 0;
        }

        var pos = posT1;
        int total;

        if (!highByteContended && isUlaPort)
        {
            // N:1, C:3 - no check at T1, contention check at T2.
            pos = WrapAdd(pos, 1);
            total = contentionTable.GetContentionAt(pos);
        }
        else if (highByteContended && isUlaPort)
        {
            // C:1, C:3 - contention check at T1 and T2.
            var d1 = contentionTable.GetContentionAt(pos);
            total = d1;
            pos = WrapAdd(pos, d1 + 1);
            total += contentionTable.GetContentionAt(pos);
        }
        else
        {
            // C:1, C:1, C:1, C:1 - contention check at each of the 4 T-states.
            var d = contentionTable.GetContentionAt(pos);
            total = d;
            pos = WrapAdd(pos, d + 1);

            d = contentionTable.GetContentionAt(pos);
            total += d;
            pos = WrapAdd(pos, d + 1);

            d = contentionTable.GetContentionAt(pos);
            total += d;
            pos = WrapAdd(pos, d + 1);

            total += contentionTable.GetContentionAt(pos);
        }

        return total;
    }

    /// <summary>
    /// Returns whether the address lies within the Spectrum 48K contended RAM window.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsContendedAddress(ushort address) => (address & 0xC000) == 0x4000;

    /// <summary>
    /// Adds within the frame timeline, wrapping once at the frame boundary.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int WrapAdd(int value, int amount)
    {
        value += amount;
        return value >= TStatesPerFrame ? value - TStatesPerFrame : value;
    }

    /// <summary>
    /// Subtracts within the frame timeline, wrapping once when moving back before T-state zero.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int WrapSubtract(int value, int amount)
    {
        value -= amount;
        return value < 0 ? value + TStatesPerFrame : value;
    }

    /// <summary>
    /// Ensures a frame position is within the valid half-open range <c>[0, TStatesPerFrame)</c>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ValidateTStatesInCurrentFrame(int tStatesInCurrentFrame)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(tStatesInCurrentFrame);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(tStatesInCurrentFrame, TStatesPerFrame);
    }
}