namespace MrKWatkins.OakCpu.Z80.Tests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class Z80EmulatorTests
{
    static Z80EmulatorTests()
    {
#pragma warning disable CA1065
        if (!BitConverter.IsLittleEndian)
        {
            throw new NotSupportedException("Only little endian systems are supported.");
        }
#pragma warning restore CA1065
    }

    [Test]
    public void AF()
    {
        var emulator = new Z80Emulator();

        emulator.A.Should().Equal(0x00);
        emulator.F.Should().Equal(0x00);
        emulator.AF.Should().Equal(0x0000);

        emulator.A = 0x12;
        emulator.AF.Should().Equal(0x1200);

        emulator.F = 0x34;
        emulator.AF.Should().Equal(0x1234);

        emulator.AF = 0x5678;
        emulator.A.Should().Equal(0x56);
        emulator.F.Should().Equal(0x78);
    }

    [Test]
    public void Registers()
    {
        var emulator = new Z80Emulator();

        emulator.Registers.A.Should().Equal(0x00);
        emulator.Registers.F.Should().Equal(0x00);
        emulator.Registers.AF.Should().Equal(0x0000);

        emulator.Registers.A = 0x12;
        emulator.Registers.AF.Should().Equal(0x1200);
        emulator.AF.Should().Equal(0x1200);

        emulator.Registers.F = 0x34;
        emulator.Registers.AF.Should().Equal(0x1234);
        emulator.AF.Should().Equal(0x1234);

        emulator.Registers.AF = 0x5678;
        emulator.Registers.A.Should().Equal(0x56);
        emulator.Registers.F.Should().Equal(0x78);
        emulator.AF.Should().Equal(0x5678);
    }

    [Test]
    public void ShadowRegisters()
    {
        var emulator = new Z80Emulator();

        emulator.Registers.Shadow.AF.Should().Equal(0x0000);

        emulator.Shadow_AF = 0x1234;
        emulator.Registers.Shadow.AF.Should().Equal(0x1234);
        emulator.Shadow_AF.Should().Equal(0x1234);
    }

    [Test]
    public void Flags()
    {
        var emulator = new Z80Emulator();

        emulator.Flags.X.Should().BeFalse();
        emulator.Flags.Y.Should().BeFalse();

        emulator.Flags.X = true;
        emulator.Flags.X.Should().BeTrue();
        emulator.Flags.Y.Should().BeFalse();
        emulator.F.Should().Equal(0b00001000);

        emulator.Flags.Y = true;
        emulator.Flags.X.Should().BeTrue();
        emulator.Flags.Y.Should().BeTrue();
        emulator.F.Should().Equal(0b00101000);

        emulator.Flags.X = false;
        emulator.Flags.X.Should().BeFalse();
        emulator.Flags.Y.Should().BeTrue();
        emulator.F.Should().Equal(0b00100000);

        emulator.Flags.Y = false;
        emulator.Flags.X.Should().BeFalse();
        emulator.Flags.Y.Should().BeFalse();
        emulator.F.Should().Equal(0b00000000);
    }
}