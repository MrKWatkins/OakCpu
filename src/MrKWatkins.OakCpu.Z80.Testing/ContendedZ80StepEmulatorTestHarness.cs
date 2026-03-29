namespace MrKWatkins.OakCpu.Z80.Testing;

public sealed class ContendedZ80StepEmulatorTestHarness(Z80StepEmulator emulator) : Z80StepEmulatorTestHarness(emulator)
{
    private const int FinalTStateInFrame = ContentionTable.TStatesPerFrame - 1;

    private readonly ContendedZ80StepEmulator contended = new(emulator);

    public ContendedZ80StepEmulatorTestHarness()
        : this(new Z80StepEmulator())
    {
    }

    public override void Reset()
    {
        base.Reset();
        ResynchroniseFrame(0);
    }

    public void ResynchroniseFrame(int tStatesInCurrentFrame)
    {
        contended.ResynchroniseFrame(tStatesInCurrentFrame);
    }

    public override void Step()
    {
        if (ShouldAssertInterruptBeforeStep())
        {
            contended.Interrupts.Interrupt = true;
        }

        var actionRequired = contended.Step();
        PerformActionRequired(actionRequired);
    }

    [Pure]
    private bool ShouldAssertInterruptBeforeStep() =>
        contended.TStatesInCurrentFrame == 0 ||
        contended is { TStatesInCurrentFrame: FinalTStateInFrame, PendingDelay: 0, HasPendingAction: false };
}