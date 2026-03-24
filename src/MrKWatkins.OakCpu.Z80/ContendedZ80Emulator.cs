using System.Text;

namespace MrKWatkins.OakCpu.Z80;

/// <summary>
/// Wraps a <see cref="Z80Emulator" /> and exposes the same public surface while applying ZX Spectrum contention to
/// <see cref="Step()" />.
/// </summary>
public sealed class ContendedZ80Emulator
{
    private readonly Z80Emulator emulator;
    private readonly Contention contention;
    private ActionRequired pendingActionRequired;

    public ContendedZ80Emulator()
        : this(new Z80Emulator())
    {
    }

    public ContendedZ80Emulator(Z80Emulator emulator, int tStatesInCurrentFrame = 0, bool earlyTimings = true)
    {
        ArgumentNullException.ThrowIfNull(emulator);
        ArgumentOutOfRangeException.ThrowIfNegative(tStatesInCurrentFrame);

        this.emulator = emulator;
        contention = new Contention(tStatesInCurrentFrame, earlyTimings);
    }

    public const ushort IM0Start = Z80Emulator.IM0Start;
    public const ushort IM1Start = Z80Emulator.IM1Start;
    public const ushort IM2Start = Z80Emulator.IM2Start;

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
        contention.ResynchroniseFrame(0);
    }

    /// <summary>
    /// Resynchronises frame-position and wrapper transient state.
    /// </summary>
    /// <remarks>
    /// This can only be called when no delay or action is pending from a previous <see cref="Step"/> call.
    /// </remarks>
    public void ResynchroniseFrame(int tStatesInCurrentFrame)
    {
        if (PendingDelay != 0 || HasPendingAction)
        {
            throw new InvalidOperationException("Cannot resynchronise frame while a delayed cycle is pending.");
        }

        contention.ResynchroniseFrame(tStatesInCurrentFrame);
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

    public void ExecuteInstruction(Action<ActionRequired> onStepComplete, Action? onBeforeStep = null)
    {
        if (PendingDelay != 0 || HasPendingAction)
        {
            throw new InvalidOperationException("Cannot execute an instruction while a delayed cycle is pending.");
        }

        emulator.ExecuteInstruction(onStepComplete, onBeforeStep);
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

    public static ContendedZ80Emulator Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);
        var earlyTimings = reader.ReadBoolean();
        var deserialized = new ContendedZ80Emulator(new Z80Emulator(), earlyTimings: earlyTimings);
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