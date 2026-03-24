namespace MrKWatkins.OakCpu.Z80.Testing;

public sealed class Z80EmulatorWithContentionTestHarness : Z80EmulatorTestHarness
{
    private const int FinalTStateInFrame = ContentionTable.TStatesPerFrame - 1;

    private readonly ContendedZ80Emulator contended;

    public Z80EmulatorWithContentionTestHarness()
        : this(new Z80Emulator())
    {
    }

    public Z80EmulatorWithContentionTestHarness(Z80Emulator emulator)
        : base(emulator)
    {
        contended = new ContendedZ80Emulator(emulator);
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