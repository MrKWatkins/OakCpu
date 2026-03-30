using MrKWatkins.OakCpu.Z80.Testing;

namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class ContendedZ80StepEmulatorTestHarnessTests
{
    [Test]
    public void Step_AdvancesOneTState_WhenContentionDelayIsPending()
    {
        var z80 = new ContendedZ80StepEmulatorTestHarness(new Z80StepEmulator(), tStatesInCurrentFrame: 14335)
        {
            RegisterPC = 0x4000
        };
        z80.WriteByteToMemory(0x4000, 0x3E);

        for (var step = 1UL; step <= 6; step++)
        {
            z80.Step();
            z80.TStates.Should().Equal(step);
            z80.Emulator.Data.Should().Equal(0x00);
        }

        z80.Step();
        z80.TStates.Should().Equal(7UL);
        z80.Emulator.Data.Should().Equal(0x3E);
    }

}