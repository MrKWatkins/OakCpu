namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class InstructionTests
{
    [Test]
    public void PrefixedAlternativesResetOpcodeTable()
    {
        var emulator = new Z80EmulatorTestHarness
        {
            RegisterPC = 0x8335,
            RegisterAF = 0xA3C5,
            RegisterQ = 0xC5
        };

        // Prefixed NOP alternative.
        emulator.WriteByteToMemory(0x8335, 0xED);
        emulator.WriteByteToMemory(0x8336, 0x06);

        // CCF.
        emulator.WriteByteToMemory(0x8337, 0x3F);

        // Execute NOP.
        emulator.ExecuteInstruction(TestContext.Progress);

        // PC should have moved 2 bytes and Q reset.
        emulator.RegisterPC.Should().Equal(0x8337);
        emulator.RegisterQ.Should().Equal(0x00);

        // Execute CCF.
        emulator.ExecuteInstruction(TestContext.Progress);

        // PC should have moved 1 byte, flags and Q should have been updated.
        emulator.RegisterPC.Should().Equal(0x8338);
        emulator.RegisterAF.Should().Equal(0xA3F4);
        emulator.RegisterQ.Should().Equal(0xF4);
    }
}