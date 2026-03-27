namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class ContendedZ80StepEmulatorTests
{
    [Test]
    public void Step_PreservesPendingActionAcrossDelay()
    {
        var memory = new byte[65536];
        memory[0x4000] = 0x3E;

        var z80 = new Z80StepEmulator();
        z80.Registers.PC = 0x4000;

        var contended = new ContendedZ80StepEmulator(z80, tStatesInCurrentFrame: 14335);

        contended.Step().Should().Equal(ActionRequired.None);
        contended.PendingDelay.Should().Equal(5);
        contended.HasPendingAction.Should().BeTrue();
        z80.Data.Should().Equal(0x00);

        for (var i = 0; i < 5; i++)
        {
            contended.Step().Should().Equal(ActionRequired.None);
        }

        contended.PendingDelay.Should().Equal(0);
        contended.HasPendingAction.Should().BeTrue();
        z80.Data.Should().Equal(0x00);

        PerformActionRequired(memory, z80, contended.Step());
        contended.HasPendingAction.Should().BeFalse();
        z80.Data.Should().Equal(0x3E);
    }

    [Test]
    public void ResynchroniseFrame_ThrowsWhenDelayIsPending()
    {
        var z80 = new Z80StepEmulator();
        z80.Registers.PC = 0x4000;

        var contended = new ContendedZ80StepEmulator(z80, tStatesInCurrentFrame: 14335);
        contended.Step().Should().Equal(ActionRequired.None);

        contended.Invoking(c => c.ResynchroniseFrame(0))
            .Should().Throw<InvalidOperationException>()
            .That.Should().HaveMessage("Cannot resynchronise frame while a delayed cycle is pending.");
    }

    [Test]
    public void ResynchroniseFrame_ThrowsWhenActionIsPending()
    {
        var z80 = new Z80StepEmulator();
        z80.Registers.PC = 0x4000;

        var contended = new ContendedZ80StepEmulator(z80, tStatesInCurrentFrame: 14335);
        for (var i = 0; i < 6; i++)
        {
            contended.Step();
        }

        contended.Invoking(c => c.ResynchroniseFrame(0))
            .Should().Throw<InvalidOperationException>()
            .That.Should().HaveMessage("Cannot resynchronise frame while a delayed cycle is pending.");
    }

    [Test]
    public void ResynchroniseFrame_UpdatesFramePosition()
    {
        var contended = new ContendedZ80StepEmulator(new Z80StepEmulator(), tStatesInCurrentFrame: 1234);

        contended.TStatesInCurrentFrame.Should().Equal(1234);
        contended.ResynchroniseFrame(5678);
        contended.TStatesInCurrentFrame.Should().Equal(5678);
        contended.PendingDelay.Should().Equal(0);
        contended.HasPendingAction.Should().BeFalse();
    }

    [Test]
    public void Step_UsesExternalInterruptState()
    {
        var memory = new byte[65536];
        memory[0x0000] = 0x00;
        memory[0x0038] = 0x76;

        var z80 = new Z80StepEmulator();
        var contended = new ContendedZ80StepEmulator(z80);
        z80.Interrupts.IFF1 = true;
        z80.Interrupts.IM = 1;

        for (var i = 0; i < 3; i++)
        {
            PerformActionRequired(memory, z80, contended.Step());
        }

        z80.Registers.PC.Should().Equal(0x0001);
        z80.Interrupts.Interrupt = true;
        PerformActionRequired(memory, z80, contended.Step());
        z80.CurrentStep.Should().Equal(ContendedZ80StepEmulator.IM1Start);
    }

    private static void PerformActionRequired(byte[] memory, Z80StepEmulator emulator, ActionRequired actionRequired)
    {
        switch (actionRequired)
        {
            case ActionRequired.OpcodeRead:
            case ActionRequired.MemoryRead:
                emulator.Data = memory[emulator.Address];
                break;

            case ActionRequired.MemoryWrite:
                memory[emulator.Address] = emulator.Data;
                break;

            case ActionRequired.IoRead:
                emulator.Data = 0xFF;
                break;
        }
    }
}