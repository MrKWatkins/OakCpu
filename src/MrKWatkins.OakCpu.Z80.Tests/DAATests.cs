using FluentAssertions.Execution;

namespace MrKWatkins.OakCpu.Z80.Tests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class DAATests
{
    [TestCaseSource(typeof(DAATestCases), nameof(DAATestCases.TestCases))]
    public void Execute(bool n, bool c, bool h, byte a, bool expectedC, bool expectedH, byte expectedA)
    {
        var emulator = new Z80EmulatorTestHarness
        {
            FlagN = n,
            FlagC = c,
            FlagH = h,
            RegisterA = a
        };

        emulator.WriteByteToMemory(0x0000, 0x27);

        emulator.ExecuteInstruction();

        using (new AssertionScope())
        {
            emulator.FlagN.Should().Be(n);
            emulator.FlagC.Should().Be(expectedC);
            emulator.FlagH.Should().Be(expectedH);
            emulator.RegisterA.Should().Be(expectedA);
        }
    }
}