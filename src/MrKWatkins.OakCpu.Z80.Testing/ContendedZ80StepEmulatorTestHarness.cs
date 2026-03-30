using MrKWatkins.EmulatorTestSuites.Z80.Program.Timing;

namespace MrKWatkins.OakCpu.Z80.Testing;

public sealed class ContendedZ80StepEmulatorTestHarness : Z80StepEmulatorTestHarness, IFrameAwareTestHarness
{
    private const int FinalTStateInFrame = ContentionTable.TStatesPerFrame - 1;
    // The 48K Spectrum ULA holds INT active for 32 T-states at the start of each frame.
    private const int InterruptPulseLength = 32;

    private readonly ContendedZ80StepEmulator contended;
    private readonly FloatingBusIO timingTestIo;
    private bool interruptLine;
    private int interruptPulseRemaining;

    public ContendedZ80StepEmulatorTestHarness()
        : this(new Z80StepEmulator())
    {
    }

    public ContendedZ80StepEmulatorTestHarness(Z80StepEmulator emulator)
        : this(emulator, 0)
    {
    }

    internal ContendedZ80StepEmulatorTestHarness(Z80StepEmulator emulator, int tStatesInCurrentFrame)
        : base(emulator)
    {
        contended = new(emulator, tStatesInCurrentFrame, earlyTimings: true);
        timingTestIo = new(() => contended.TStatesInCurrentFrame, ReadByteFromMemory);
        SetIO(timingTestIo);
    }

    public override void Reset()
    {
        base.Reset();
        SetIO(timingTestIo);
        interruptLine = false;
        StartFrame();
    }

    public void StartFrame()
    {
        contended.StartFrame();
        interruptPulseRemaining = 0;
    }

    public int TStatesInCurrentFrame => contended.TStatesInCurrentFrame;

    public override bool Interrupt
    {
        get => interruptLine;
        set => interruptLine = value;
    }

    public override void Step()
    {
        if (interruptPulseRemaining == 0 && ShouldStartInterruptPulse())
        {
            interruptPulseRemaining = InterruptPulseLength;
        }

        contended.Interrupts.Interrupt = interruptLine || interruptPulseRemaining != 0;
        var actionRequired = contended.Step();
        if (interruptPulseRemaining != 0)
        {
            interruptPulseRemaining--;
        }

        PerformActionRequired(actionRequired);
    }

    [Pure]
    private bool ShouldStartInterruptPulse() =>
        contended.TStatesInCurrentFrame == 0 ||
        contended is { TStatesInCurrentFrame: FinalTStateInFrame, PendingDelay: 0, HasPendingAction: false };
}