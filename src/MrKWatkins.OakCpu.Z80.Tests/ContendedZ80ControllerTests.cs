namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class ContendedZ80ControllerTests
{
    [Test]
    public void Execute_PreservesPendingActionAcrossPartialDelayBudget()
    {
        var memory = new byte[65536];
        memory[0x4000] = 0x3E;

        var z80 = new Z80Emulator();
        z80.Registers.PC = 0x4000;

        var loop = new ContendedZ80Controller(z80, PerformActionRequired(memory), tStatesInCurrentFrame: 14335);

        var elapsed = 0;
        loop.Execute(1, e => elapsed += e).Should().BeTrue();
        elapsed.Should().Equal(1);
        loop.PendingDelay.Should().Equal(5);
        loop.HasPendingAction.Should().BeTrue();
        z80.Data.Should().Equal(0x00);

        loop.Execute(5, e => elapsed += e).Should().BeTrue();
        elapsed.Should().Equal(6);
        loop.PendingDelay.Should().Equal(0);
        loop.HasPendingAction.Should().BeTrue();
        z80.Data.Should().Equal(0x00);

        loop.Execute(1, e => elapsed += e).Should().BeTrue();
        elapsed.Should().Equal(7);
        loop.HasPendingAction.Should().BeFalse();
        z80.Data.Should().Equal(0x3E);
    }

    [Test]
    public void Execute_StopsEarlyAndRetainsPendingState()
    {
        var memory = new byte[65536];
        memory[0x4000] = 0x3E;

        var z80 = new Z80Emulator();
        z80.Registers.PC = 0x4000;

        var loop = new ContendedZ80Controller(z80, PerformActionRequired(memory), tStatesInCurrentFrame: 14335);

        var elapsed = 0;
        var stopChecks = 0;
        loop.Execute(100, e => elapsed += e, () =>
        {
            stopChecks++;
            return stopChecks == 1;
        }).Should().BeFalse();

        elapsed.Should().Equal(6);
        loop.PendingDelay.Should().Equal(0);
        loop.HasPendingAction.Should().BeTrue();
        z80.Data.Should().Equal(0x00);

        loop.Execute(1, e => elapsed += e).Should().BeTrue();
        elapsed.Should().Equal(7);
        loop.HasPendingAction.Should().BeFalse();
        z80.Data.Should().Equal(0x3E);
    }

    [Test]
    public void ResynchroniseFrame_ThrowsWhenDelayIsPending()
    {
        var memory = new byte[65536];
        memory[0x4000] = 0x3E;

        var z80 = new Z80Emulator();
        z80.Registers.PC = 0x4000;

        var loop = new ContendedZ80Controller(z80, PerformActionRequired(memory), tStatesInCurrentFrame: 14335);
        loop.Execute(1, _ => { }).Should().BeTrue();

        loop.Invoking(l => l.ResynchroniseFrame(0))
            .Should().Throw<InvalidOperationException>()
            .That.Should().HaveMessage("Cannot resynchronise frame while a delayed cycle is pending.");
    }

    [Test]
    public void ResynchroniseFrame_ThrowsWhenActionIsPending()
    {
        var memory = new byte[65536];
        memory[0x4000] = 0x3E;

        var z80 = new Z80Emulator();
        z80.Registers.PC = 0x4000;

        var loop = new ContendedZ80Controller(z80, PerformActionRequired(memory), tStatesInCurrentFrame: 14335);
        loop.Execute(6, _ => { }).Should().BeTrue();

        loop.Invoking(l => l.ResynchroniseFrame(0))
            .Should().Throw<InvalidOperationException>()
            .That.Should().HaveMessage("Cannot resynchronise frame while a delayed cycle is pending.");
    }

    [Test]
    public void ResynchroniseFrame_UpdatesFramePosition()
    {
        var memory = new byte[65536];
        var z80 = new Z80Emulator();
        var loop = new ContendedZ80Controller(z80, PerformActionRequired(memory), tStatesInCurrentFrame: 1234);

        loop.TStatesInCurrentFrame.Should().Equal(1234);
        loop.ResynchroniseFrame(5678);
        loop.TStatesInCurrentFrame.Should().Equal(5678);
        loop.PendingDelay.Should().Equal(0);
        loop.HasPendingAction.Should().BeFalse();
    }

    private static Action<Z80Emulator, ActionRequired> PerformActionRequired(byte[] memory) => (emulator, actionRequired) =>
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
    };
}