using MrKWatkins.OakCpu.Z80.Testing;

using CycleType = MrKWatkins.EmulatorTestSuites.Z80.CycleType;

namespace MrKWatkins.OakCpu.Z80.Tests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class Z80StepEmulatorTests
{
    static Z80StepEmulatorTests()
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
        var emulator = new Z80StepEmulator();

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
        var emulator = new Z80StepEmulator();

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
        var emulator = new Z80StepEmulator();

        emulator.Registers.Shadow.AF.Should().Equal(0x0000);

        emulator.Shadow_AF = 0x1234;
        emulator.Registers.Shadow.AF.Should().Equal(0x1234);
        emulator.Shadow_AF.Should().Equal(0x1234);
    }

    [Test]
    public void Flags()
    {
        var emulator = new Z80StepEmulator();

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
    public void IsAtInstructionBoundary()
    {
        var z80 = new Z80StepEmulatorTestHarness();

        // LD B, $42
        z80.WriteByteToMemory(0x0000, 0x06);
        z80.WriteByteToMemory(0x0001, 0x42);

        z80.Emulator.IsAtInstructionBoundary.Should().BeTrue();

        z80.Step();
        z80.Emulator.IsAtInstructionBoundary.Should().BeFalse();

        z80.ExecuteInstruction();
        z80.Emulator.IsAtInstructionBoundary.Should().BeTrue();
        z80.RegisterB.Should().Equal(0x42);
    }

    [Test]
    public void IsAtInstructionBoundary_InterruptAccepted()
    {
        var z80 = new Z80StepEmulatorTestHarness
        {
            RecordCycles = true,
            IFF1 = true,
            IFF2 = true,
            IM = 1
        };

        z80.WriteByteToMemory(0x0000, 0x00);

        z80.Step(3);
        z80.Emulator.IsAtInstructionBoundary.Should().BeFalse();

        z80.Interrupt = true;
        z80.Step();
        z80.Cycles[^1].Type.Should().Equal(CycleType.None);
        z80.Emulator.IsAtInstructionBoundary.Should().BeTrue();
        z80.IFF1.Should().BeTrue();
        z80.IFF2.Should().BeTrue();

        z80.Step();
        z80.Emulator.IsAtInstructionBoundary.Should().BeFalse();
        z80.IFF1.Should().BeFalse();
        z80.IFF2.Should().BeFalse();
    }

    [Test]
    public void ExecuteInstruction_NoOverlappedRead()
    {
        var z80 = new Z80StepEmulatorTestHarness();

        // LD B, $42
        z80.WriteByteToMemory(0x0000, 0x06);
        z80.WriteByteToMemory(0x0001, 0x42);

        z80.ExecuteInstruction(TestContext.Progress);

        z80.TStates.Should().Equal(7);
        z80.RegisterPC.Should().Equal(0x0002);
        z80.RegisterB.Should().Equal(0x42);
    }

    [Test]
    public void ExecuteInstruction_OverlappedRead()
    {
        var z80 = new Z80StepEmulatorTestHarness();

        // INC B
        z80.WriteByteToMemory(0x0000, 0x04);

        z80.ExecuteInstruction(TestContext.Progress);

        z80.TStates.Should().Equal(4);
        z80.RegisterPC.Should().Equal(0x0001);
        z80.RegisterB.Should().Equal(0x01);
    }

    [Test]
    public void ExecuteInstruction_Halt()
    {
        var z80 = new Z80StepEmulatorTestHarness();

        // HALT
        z80.WriteByteToMemory(0x0000, 0x76);

        z80.ExecuteInstruction(TestContext.Progress);

        z80.TStates.Should().Equal(4);
        z80.Halted.Should().BeTrue();

        // PC does actually advance after a HALT.
        z80.RegisterPC.Should().Equal(0x0001);

        // Test the halted cycle.
        z80.ExecuteInstruction(TestContext.Progress);

        z80.TStates.Should().Equal(8);
        z80.Halted.Should().BeTrue();
        z80.RegisterPC.Should().Equal(0x0001);
    }
}