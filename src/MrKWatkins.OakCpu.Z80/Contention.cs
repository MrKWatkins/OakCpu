using System.Runtime.CompilerServices;
using System.Reflection;

namespace MrKWatkins.OakCpu.Z80;

/// <summary>
/// Applies ZX Spectrum 48K ULA contention to a <see cref="Z80Emulator" />. Memory and I/O accesses to contended
/// addresses are delayed based on the current position in the frame. Also fires interrupts at frame boundaries.
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

    // Precomputed per-step mask for boundaries where pre-asserting INT would be early. These are steps that
    // emit an OpcodeRead and immediately continue at step 1, i.e. overlap opcode fetch with the previous instruction.
    private static readonly bool[] SuppressImmediateInterruptByStep = CreateSuppressImmediateInterruptByStep();

    private readonly ContentionTable contentionTable;

    /// <summary>
    /// Number of continuation None steps remaining for the current bus cycle. When a bus cycle starts
    /// (OpcodeRead, MemoryRead, etc.), this is set to the number of subsequent None steps that are part
    /// of that cycle and should not be separately contended. It is decremented for each None step.
    /// </summary>
    private int skipCount;

    /// <summary>
    /// The contention delay that was applied by the most recent non-skipped None step. Used by
    /// <see cref="CalculateIoContention"/> to undo spurious memory contention from the I/O cycle's T1 step.
    /// </summary>
    private int prevNoneDelay;

    /// <summary>
    /// True when the frame boundary was crossed on a step where pre-asserting the interrupt would have been early.
    /// In that case we queue the request here and assert it at the next safe pre-step boundary.
    /// </summary>
    private bool frameInterruptPending;

    public Contention(Z80Emulator z80, int tStatesInCurrentFrame = 0, bool earlyTimings = true)
    {
        ValidateTStatesInCurrentFrame(tStatesInCurrentFrame);

        Z80 = z80;
        contentionTable = earlyTimings ? ContentionTable.EarlyTimings : ContentionTable.LateTimings;
        TStatesInCurrentFrame = tStatesInCurrentFrame;
    }

    public Z80Emulator Z80 { get; }

    // ReSharper disable once InconsistentNaming
    public int TStatesInCurrentFrame { get; private set; }

    internal void ResynchroniseFrame(int tStatesInCurrentFrame)
    {
        ValidateTStatesInCurrentFrame(tStatesInCurrentFrame);

        TStatesInCurrentFrame = tStatesInCurrentFrame;
        skipCount = 0;
        prevNoneDelay = 0;
        frameInterruptPending = false;
    }

    /// <summary>
    /// Asserts the interrupt line on the Z80 before a step if this boundary should be visible to the emulator's
    /// interrupt check in that step.
    ///
    /// Returns <c>true</c> when this method pre-asserted the interrupt for the current step. The caller must pass
    /// that value to <see cref="Advance"/> so we do not queue the same interrupt again on frame wrap.
    ///
    /// Why the split:
    /// - The emulator checks interrupts at specific instruction-boundary steps (including overlap handlers).
    /// - Contention frame position advances after the step in <see cref="Advance"/>.
    /// - For non-overlapped boundaries we can pre-assert when we know the next T-state will wrap.
    /// - For overlapped opcode-read boundaries, pre-assert would be one boundary early, so we suppress pre-assert
    ///   and queue the interrupt to be asserted on the next pre-step.
    /// </summary>
    public bool CheckForFrameInterrupt()
    {
        if (frameInterruptPending)
        {
            Z80.interrupt = true;
            frameInterruptPending = false;
            return false;
        }

        // Steps that perform overlapped opcode reads (ActionRequired.OpcodeRead + next step 1) should not
        // see a frame interrupt that occurs during that same step; the interrupt becomes visible on a later
        // instruction boundary. For all other steps, preserve the existing pre-check behaviour.
        if (!SuppressImmediateInterruptByStep[Z80.CurrentStep] &&
            TStatesInCurrentFrame + 1 >= TStatesPerFrame)
        {
            Z80.interrupt = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns the number of extra T-states (contention delay) to add for the given step and advances frame
    /// position bookkeeping.
    ///
    /// The caller must save <c>emulator.Address</c> before calling <c>Step()</c> and pass it as
    /// <paramref name="preStepAddress"/>; this is the address that was on the bus at the start of the step,
    /// before any handler modified it.
    ///
    /// <paramref name="interruptPredictedThisStep"/> must match the return value from
    /// <see cref="CheckForFrameInterrupt"/> for this step.
    /// </summary>
    public int Advance(ActionRequired actionRequired, ushort address, ushort preStepAddress, bool interruptPredictedThisStep)
    {
        var delay = actionRequired switch
        {
            ActionRequired.OpcodeRead => CalculateOpcodeReadContention(address),

            ActionRequired.MemoryRead or ActionRequired.MemoryWrite
                => CalculateMemoryContention(address),

            ActionRequired.IoRead or ActionRequired.IoWrite
                => IsInterruptAcknowledge() ? CalculateInterruptAcknowledgeContention() : CalculateIoContention(address),

            // Use the pre-step address for internal cycles. Handlers may update Address to prepare for
            // the next bus cycle (e.g. OUTI sets Address=BC before IoWrite), but the ULA sees the
            // address that was on the bus at the start of the step.
            ActionRequired.None => CalculateNoneContention(preStepAddress),

            _ => 0
        };

        // Advance the frame position by the delay plus the normal 1 T-state for this step.
        TStatesInCurrentFrame += delay + 1;

        // Wrap at the frame boundary and queue the frame interrupt to be asserted before the next step.
        if (TStatesInCurrentFrame >= TStatesPerFrame)
        {
            TStatesInCurrentFrame -= TStatesPerFrame;
            // If this step already pre-asserted interrupt visibility, do not queue again. Otherwise, queue so the
            // next pre-step boundary can expose the interrupt to the emulator.
            if (!interruptPredictedThisStep)
            {
                frameInterruptPending = true;
            }
        }

        return delay;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CalculateOpcodeReadContention(ushort address)
    {
        // M1 cycle is 4 T-states: 1 OpcodeRead + 3 None continuation steps.
        skipCount = 3;
        prevNoneDelay = 0;
        return CalculateContention(address);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CalculateMemoryContention(ushort address)
    {
        // Memory read/write cycle is 3 T-states: 1 MemoryRead/Write + 2 None continuation steps.
        skipCount = 2;
        prevNoneDelay = 0;
        return CalculateContention(address);
    }

    private int CalculateIoContention(ushort port)
    {
        // The I/O cycle in the emulator is 4 steps: None (T1, sets port address) + IoRead/IoWrite (T2)
        // + 2 continuation None steps (T3, T4). The preceding None step (T1) may have added spurious
        // memory contention at the wrong address; undo it and recalculate from T1's frame position.
        TStatesInCurrentFrame -= prevNoneDelay;
        var posT1 = (TStatesInCurrentFrame - 1 + TStatesPerFrame) % TStatesPerFrame;

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
        var total = 0;

        if (!highByteContended && isUlaPort)
        {
            // N:1, C:3 - no check at T1, contention check at T2.
            pos = (pos + 1) % TStatesPerFrame;
            total = contentionTable[pos];
        }
        else if (highByteContended && isUlaPort)
        {
            // C:1, C:3 - contention check at T1 and T2.
            var d1 = contentionTable[pos];
            total = d1;
            pos = (pos + d1 + 1) % TStatesPerFrame;
            total += contentionTable[pos];
        }
        else
        {
            // C:1, C:1, C:1, C:1 - contention check at each of the 4 T-states.
            for (var i = 0; i < 4; i++)
            {
                var d = contentionTable[pos];
                total += d;
                pos = (pos + d + 1) % TStatesPerFrame;
            }
        }

        return total;
    }

    /// <summary>
    /// The interrupt acknowledge IoRead/IoWrite is not a regular I/O cycle. FUSE adds 7 T-states flat
    /// for the first part of the interrupt response with no contention. We detect the acknowledge by
    /// checking if the emulator is now at the step after the IM handler's IoRead.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsInterruptAcknowledge()
    {
        var step = Z80.CurrentStep;
        return step == Z80Emulator.IM0Start + 2 ||
               step == Z80Emulator.IM1Start + 2 ||
               step == Z80Emulator.IM2Start + 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CalculateInterruptAcknowledgeContention()
    {
        // No IO contention for interrupt acknowledge. Undo any spurious memory contention from the
        // preceding None step (IM handler T1). FUSE adds 7 flat T-states for the interrupt response
        // with no contention at all. The IoRead step is step 2 of the 7-step sequence; skip the
        // remaining 5 steps (3-7) so they are not independently contended.
        TStatesInCurrentFrame -= prevNoneDelay;
        skipCount = 5;
        prevNoneDelay = 0;
        return 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CalculateNoneContention(ushort address)
    {
        // If this None step is a continuation of the previous bus cycle, don't contend it.
        if (skipCount > 0)
        {
            skipCount--;
            prevNoneDelay = 0;
            return 0;
        }

        // This is an independent internal 1-T-state machine cycle; contend based on address.
        var delay = CalculateContention(address);
        prevNoneDelay = delay;
        return delay;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CalculateContention(ushort address)
    {
        if (IsContendedAddress(address))
        {
            return contentionTable[TStatesInCurrentFrame];
        }

        return 0;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsContendedAddress(ushort address) => address is >= 0x4000 and <= 0x7FFF;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ValidateTStatesInCurrentFrame(int tStatesInCurrentFrame)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(tStatesInCurrentFrame);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(tStatesInCurrentFrame, TStatesPerFrame);
    }

    private static bool[] CreateSuppressImmediateInterruptByStep()
    {
        // We intentionally derive this from the generated step table so it stays correct if generation changes.
        // Hard-coding step IDs would be brittle and easy to desynchronize from generated code.
        var stepsField = typeof(Z80Emulator).GetField("Steps", BindingFlags.Static | BindingFlags.NonPublic) ??
                         throw new InvalidOperationException("Unable to find Z80 step table.");
        var steps = stepsField.GetValue(null) as Step[] ??
                    throw new InvalidOperationException("Unable to load Z80 step table.");

        var suppressByStep = new bool[steps.Length];
        for (var i = 0; i < steps.Length; i++)
        {
            var step = steps[i];
            // Overlapped opcode-read shape:
            // - current action is opcode read,
            // - next step is step 1 (refresh), meaning this fetch belongs to the next instruction.
            // If we exposed INT before this step, the emulator could service one boundary too early.
            suppressByStep[i] = step.ActionRequired == ActionRequired.OpcodeRead && step.NextStep == 1;
        }

        return suppressByStep;
    }
}