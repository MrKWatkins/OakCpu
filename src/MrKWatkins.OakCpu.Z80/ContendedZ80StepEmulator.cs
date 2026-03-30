using System.Text;

namespace MrKWatkins.OakCpu.Z80;

/// <summary>
/// Wraps a <see cref="Z80StepEmulator" /> and exposes the same public surface while applying ZX Spectrum contention to
/// <see cref="Step()" />.
/// </summary>
public sealed class ContendedZ80StepEmulator
{
    private readonly Z80StepEmulator emulator;
    private readonly Contention contention;
    private ActionRequired pendingActionRequired;

    public ContendedZ80StepEmulator()
        : this(new Z80StepEmulator())
    {
    }

    public ContendedZ80StepEmulator(Z80StepEmulator emulator, bool earlyTimings = true)
        : this(emulator, 0, earlyTimings)
    {
    }

    internal ContendedZ80StepEmulator(Z80StepEmulator emulator, int tStatesInCurrentFrame, bool earlyTimings = true)
    {
        ArgumentNullException.ThrowIfNull(emulator);
        ArgumentOutOfRangeException.ThrowIfNegative(tStatesInCurrentFrame);

        this.emulator = emulator;
        contention = new Contention(tStatesInCurrentFrame, earlyTimings);
    }

    public const ushort IM0Start = Z80StepEmulator.IM0Start;
    public const ushort IM1Start = Z80StepEmulator.IM1Start;
    public const ushort IM2Start = Z80StepEmulator.IM2Start;

    public Z80Registers Registers => emulator.Registers;

    public Z80Flags Flags => emulator.Flags;

    public Z80Interrupts Interrupts => emulator.Interrupts;

    public ushort Address => emulator.Address;

    public byte Data
    {
        get => emulator.Data;
        set => emulator.Data = value;
    }

    public ushort CurrentStep => emulator.CurrentStep;

    // ReSharper disable once InconsistentNaming
    public int TStatesInCurrentFrame => contention.TStatesInCurrentFrame;

    public int PendingDelay { get; private set; }

    public bool HasPendingAction { get; private set; }

    public void Reset()
    {
        emulator.Reset();
        PendingDelay = 0;
        pendingActionRequired = ActionRequired.None;
        HasPendingAction = false;
        contention.StartFrame();
    }

    /// <summary>
    /// Starts a new frame and clears the contention bookkeeping for the current wrapper state.
    /// </summary>
    /// <remarks>
    /// This can only be called when no delay or action is pending from a previous <see cref="Step"/> call.
    /// </remarks>
    public void StartFrame()
    {
        if (PendingDelay != 0 || HasPendingAction)
        {
            throw new InvalidOperationException("Cannot start frame while a delayed cycle is pending.");
        }

        contention.StartFrame();
    }

    public ActionRequired Step()
    {
        if (PendingDelay == 0 && !HasPendingAction)
        {
            var preStepAddress = emulator.Address;
            var nextActionRequired = emulator.Step();
            PendingDelay = contention.CalculateDelay(nextActionRequired, emulator.Address, preStepAddress);
            pendingActionRequired = nextActionRequired;
            HasPendingAction = true;
        }

        if (PendingDelay > 0)
        {
            PendingDelay--;
            contention.Elapse();
            return ActionRequired.None;
        }

        var actionRequired = pendingActionRequired;
        pendingActionRequired = ActionRequired.None;
        HasPendingAction = false;
        contention.Elapse();
        return actionRequired;
    }

    public void Serialize(Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
        writer.Write(contention.IsEarlyTimings);
        writer.Write(PendingDelay);
        writer.Write((byte)pendingActionRequired);
        writer.Write(HasPendingAction);
        contention.Serialize(writer);
        emulator.Serialize(stream);
    }

    public static ContendedZ80StepEmulator Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);
        var earlyTimings = reader.ReadBoolean();
        var deserialized = new ContendedZ80StepEmulator(new Z80StepEmulator(), earlyTimings: earlyTimings);
        deserialized.Restore(stream, reader);
        return deserialized;
    }

    public void Restore(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);
        var earlyTimings = reader.ReadBoolean();
        if (contention.IsEarlyTimings != earlyTimings)
        {
            throw new InvalidOperationException("Cannot restore contention state with different timing tables.");
        }

        Restore(stream, reader);
    }

    private void Restore(Stream stream, BinaryReader reader)
    {
        PendingDelay = reader.ReadInt32();
        pendingActionRequired = (ActionRequired)reader.ReadByte();
        HasPendingAction = reader.ReadBoolean();
        contention.Restore(reader);
        emulator.Restore(stream);
    }
}