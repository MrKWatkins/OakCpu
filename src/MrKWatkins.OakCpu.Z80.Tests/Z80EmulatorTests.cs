namespace MrKWatkins.OakCpu.Z80.Tests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class Z80EmulatorTests
{
    [Test]
    public void AF()
    {
        var emulator = new Z80Emulator();

        emulator.A.Should().Be(0x00);
        emulator.F.Should().Be(0x00);
        emulator.AF.Should().Be(0x0000);

        emulator.A = 0x12;
        emulator.AF.Should().Be(0x1200);

        emulator.F = 0x34;
        emulator.AF.Should().Be(0x1234);

        emulator.AF = 0x5678;
        emulator.A.Should().Be(0x56);
        emulator.F.Should().Be(0x78);
    }
}