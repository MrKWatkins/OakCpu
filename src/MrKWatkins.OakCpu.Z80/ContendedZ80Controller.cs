namespace MrKWatkins.OakCpu.Z80;

/// <summary>
/// Controls execution of a <see cref="Z80Emulator" /> using ZX Spectrum contention timing and caller-provided bus actions.
/// </summary>
/// <remarks>
/// This class is intended to be used by host emulators that need to keep external devices (screen/audio/tape) in
/// lockstep with elapsed T-states while still honouring OakCpu contention and interrupt-edge behaviour.
/// <para>
/// The loop supports partial execution budgets: if a call ends in the middle of a delayed bus cycle, the remaining
/// delay and pending action are carried over to the next call.
/// </para>
/// </remarks>
public sealed class ContendedZ80Controller
{
    private readonly Contention contention;
    private readonly Action<Z80Emulator, ActionRequired> performActionRequired;
    private ActionRequired pendingActionRequired;

    public ContendedZ80Controller(Z80Emulator z80, Action<Z80Emulator, ActionRequired> performActionRequired)
        : this(z80, performActionRequired, 0)
    {
    }

    internal ContendedZ80Controller(
        Z80Emulator z80,
        Action<Z80Emulator, ActionRequired> performActionRequired,
        int tStatesInCurrentFrame)
    {
        ArgumentNullException.ThrowIfNull(performActionRequired);
        ArgumentOutOfRangeException.ThrowIfNegative(tStatesInCurrentFrame);

        contention = new Contention(z80, tStatesInCurrentFrame);
        this.performActionRequired = performActionRequired;
    }

    // ReSharper disable once InconsistentNaming
    public int TStatesInCurrentFrame => contention.TStatesInCurrentFrame;

    public int PendingDelay { get; private set; }

    public bool HasPendingAction { get; private set; }

    /// <summary>
    /// Resynchronises frame-position and contention/controller transient state.
    /// </summary>
    /// <remarks>
    /// This can only be called when no delay or action is pending from a previous <see cref="Execute"/> call.
    /// </remarks>
    public void ResynchroniseFrame(int tStatesInCurrentFrame)
    {
        if (PendingDelay != 0 || HasPendingAction)
        {
            throw new InvalidOperationException("Cannot resynchronise frame while a delayed cycle is pending.");
        }

        pendingActionRequired = ActionRequired.None;
        contention.ResynchroniseFrame(tStatesInCurrentFrame);
    }

    /// <summary>
    /// Executes up to <paramref name="tStates" /> T-states.
    /// </summary>
    /// <param name="tStates">The maximum number of T-states to execute.</param>
    /// <param name="onElapsed">
    /// Called every time T-states elapse. The callback value can be greater than 1 when consuming contention delay.
    /// </param>
    /// <param name="shouldStop">
    /// Optional callback checked after each elapsed chunk; return <c>true</c> to stop early (e.g. breakpoints).
    /// </param>
    /// <returns><c>true</c> if the whole budget was consumed; otherwise <c>false</c>.</returns>
    public bool Execute(ulong tStates, Action<int> onElapsed, Func<bool>? shouldStop = null)
    {
        ArgumentNullException.ThrowIfNull(onElapsed);

        var z80 = contention.Z80;

        var remaining = tStates;
        while (remaining > 0)
        {
            if (PendingDelay > 0)
            {
                var elapsed = Math.Min((ulong)PendingDelay, remaining);
                PendingDelay -= (int)elapsed;
                remaining -= elapsed;
                onElapsed((int)elapsed);
                if (shouldStop?.Invoke() == true)
                {
                    return false;
                }
                continue;
            }

            if (HasPendingAction)
            {
                if (pendingActionRequired != ActionRequired.None)
                {
                    performActionRequired(z80, pendingActionRequired);
                }

                HasPendingAction = false;
                remaining -= 1;
                onElapsed(1);
                if (shouldStop?.Invoke() == true)
                {
                    return false;
                }
                continue;
            }

            // Ordering is intentional: let contention expose any frame interrupt before the CPU step, then run
            // the step, then ask contention for delay using both pre-step and post-step bus address context.
            var interruptPredictedThisStep = contention.CheckForFrameInterrupt();
            var preStepAddress = z80.Address;
            var actionRequired = z80.Step();
            PendingDelay = contention.Advance(actionRequired, z80.Address, preStepAddress, interruptPredictedThisStep);
            pendingActionRequired = actionRequired;
            HasPendingAction = true;
        }

        return true;
    }

    /// <summary>
    /// Convenience helper for frame-based hosts.
    /// </summary>
    public bool ExecuteFrame(Action<int> onElapsed, Func<bool>? shouldStop = null) =>
        Execute(ContentionTable.TStatesPerFrame, onElapsed, shouldStop);
}