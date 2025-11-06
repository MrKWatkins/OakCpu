namespace MrKWatkins.OakCpu.Z80.Testing;

public sealed class Z80EmulatorWithContentionTestHarness : Z80EmulatorTestHarness
{
    private readonly Contention contention;

    public Z80EmulatorWithContentionTestHarness()
        : this(new Z80Emulator())
    {
    }

    public Z80EmulatorWithContentionTestHarness(Z80Emulator emulator)
        : base(emulator)
    {
        contention = new Contention(Emulator);
    }

    public override void Reset()
    {
        base.Reset();
        contention.ResynchroniseFrame(0);
    }

    public override void Step()
    {
        // The order here is intentional:
        // 1) Ask contention whether interrupt should be visible before this step.
        // 2) Execute the emulator step (where HandleInterrupts checks the interrupt line).
        // 3) Advance contention/frame bookkeeping for the completed step.
        var interruptPredictedThisStep = contention.CheckForFrameInterrupt();

        var preStepAddress = Emulator.Address;
        var actionRequired = Emulator.Step();
        var delay = contention.Advance(actionRequired, Emulator.Address, preStepAddress, interruptPredictedThisStep);
        TStates += (ulong)delay;

        PerformActionRequired(actionRequired);
    }
}