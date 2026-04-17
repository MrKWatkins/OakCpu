namespace MrKWatkins.OakCpu.Z80.Tests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class Z80InstructionEmulatorTests
{
    static Z80InstructionEmulatorTests()
    {
#pragma warning disable CA1065
        if (!BitConverter.IsLittleEndian)
        {
            throw new NotSupportedException("Only little endian systems are supported.");
        }
#pragma warning restore CA1065
    }

    [Test]
    public void Registers()
    {
        var emulator = new Z80InstructionEmulator();

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
        var emulator = new Z80InstructionEmulator();

        emulator.Registers.Shadow.AF.Should().Equal(0x0000);

        emulator.Shadow_AF = 0x1234;
        emulator.Registers.Shadow.AF.Should().Equal(0x1234);
        emulator.Shadow_AF.Should().Equal(0x1234);
    }

    [Test]
    public void Flags()
    {
        var emulator = new Z80InstructionEmulator();

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

    [Test]
    public void Interrupts()
    {
        var emulator = new Z80InstructionEmulator();

        emulator.Interrupts.IFF1.Should().BeFalse();
        emulator.Interrupts.IFF2.Should().BeFalse();
        emulator.Interrupts.IM.Should().Equal(0);
        emulator.Interrupts.Halted.Should().BeFalse();
        emulator.Interrupts.Interrupt.Should().BeFalse();

        emulator.Interrupts.IFF1 = true;
        emulator.Interrupts.IFF2 = true;
        emulator.Interrupts.IM = 2;
        emulator.Interrupts.Halted = true;
        emulator.Interrupts.Interrupt = true;

        emulator.iff1.Should().BeTrue();
        emulator.iff2.Should().BeTrue();
        emulator.im.Should().Equal(2);
        emulator.halted.Should().BeTrue();
        emulator.interrupt.Should().BeTrue();
    }
}