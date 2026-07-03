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

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void Pins_InterruptAcknowledge_InterruptMode(byte interruptMode)
    {
        var memory = new byte[65536];
        memory[0x0000] = 0x00;

        var emulator = new Z80StepEmulator
        {
            Interrupts =
            {
                IFF1 = true,
                IFF2 = true,
                IM = interruptMode
            }
        };

        var actionRequired = ActionRequired.None;
        while (!emulator.IsAtInstructionBoundary || emulator.Registers.PC != 0x0001)
        {
            actionRequired = Step(emulator, memory);
            (emulator.Pins & Z80Pins.IORQ).Should().Equal(Z80Pins.None);
        }

        emulator.Interrupts.Interrupt = true;
        while (actionRequired != ActionRequired.IORead)
        {
            actionRequired = Step(emulator, memory);
        }

        emulator.Pins.Should().Equal(Z80Pins.M1 | Z80Pins.IORQ);
    }

    [Test]
    public void Pins_IORead_IN_A_N()
    {
        var memory = new byte[65536];
        memory[0x0000] = 0xDB;
        memory[0x0001] = 0xFE;

        var emulator = new Z80StepEmulator();

        StepUntilIORead(emulator, memory);

        emulator.Pins.Should().Equal(Z80Pins.IORQ | Z80Pins.RD);
    }

    [Test]
    public void Pins_IORead_INI()
    {
        var memory = new byte[65536];
        memory[0x0000] = 0xED;
        memory[0x0001] = 0xA2;

        var emulator = new Z80StepEmulator
        {
            Registers =
            {
                BC = 0x0100,
                HL = 0x4000
            }
        };

        StepUntilIORead(emulator, memory);

        emulator.Pins.Should().Equal(Z80Pins.IORQ | Z80Pins.RD);
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

    private static void StepUntilIORead(Z80StepEmulator emulator, byte[] memory)
    {
        ActionRequired actionRequired;
        do
        {
            actionRequired = Step(emulator, memory);
        }
        while (actionRequired != ActionRequired.IORead);
    }

    [Test]
    public void Pins()
    {
        var emulator = new Z80StepEmulator();

        // Opcode fetch of LD (BC), A: an M1 cycle followed by a memory refresh.
        emulator.Step().Should().Equal(ActionRequired.OpcodeRead);
        emulator.Pins.Should().Equal(Z80Pins.M1);
        emulator.Data = 0x02;
        emulator.Step().Should().Equal(ActionRequired.None);
        emulator.Pins.Should().Equal(Z80Pins.M1 | Z80Pins.MREQ | Z80Pins.RD);
        emulator.Step().Should().Equal(ActionRequired.None);
        emulator.Pins.Should().Equal(Z80Pins.RFSH);
        emulator.Step().Should().Equal(ActionRequired.None);
        emulator.Pins.Should().Equal(Z80Pins.RFSH | Z80Pins.MREQ);

        // The memory write cycle: the action fires at T1 before any control pins are asserted, with WR following at T3.
        emulator.Step().Should().Equal(ActionRequired.MemoryWrite);
        emulator.Pins.Should().Equal(Z80Pins.None);
        emulator.Step().Should().Equal(ActionRequired.None);
        emulator.Pins.Should().Equal(Z80Pins.MREQ);
        emulator.Step().Should().Equal(ActionRequired.None);
        emulator.Pins.Should().Equal(Z80Pins.MREQ | Z80Pins.WR);
    }

    [Test]
    public void Pins_IORead()
    {
        var emulator = new Z80StepEmulator();

        // Opcode fetch of IN A, (n).
        emulator.Step().Should().Equal(ActionRequired.OpcodeRead);
        emulator.Data = 0xDB;
        emulator.Step();
        emulator.Step();
        emulator.Step();

        // Operand read: the action fires at T1; MREQ and RD are asserted for T2 and T3.
        emulator.Step().Should().Equal(ActionRequired.MemoryRead);
        emulator.Pins.Should().Equal(Z80Pins.None);
        emulator.Data = 0xFE;
        emulator.Step().Should().Equal(ActionRequired.None);
        emulator.Pins.Should().Equal(Z80Pins.MREQ | Z80Pins.RD);
        emulator.Step().Should().Equal(ActionRequired.None);
        emulator.Pins.Should().Equal(Z80Pins.MREQ | Z80Pins.RD);

        // I/O read: T1 puts the port on the address bus with no control pins asserted; IORQ and RD assert from T2 to the
        // end of the cycle, with the action firing at T2.
        emulator.Step().Should().Equal(ActionRequired.None);
        emulator.Pins.Should().Equal(Z80Pins.None);
        emulator.Address.Should().Equal(0x00FE);
        emulator.Step().Should().Equal(ActionRequired.IORead);
        emulator.Pins.Should().Equal(Z80Pins.IORQ | Z80Pins.RD);
        emulator.Data = 0xAA;
        emulator.Step().Should().Equal(ActionRequired.None);
        emulator.Pins.Should().Equal(Z80Pins.IORQ | Z80Pins.RD);
        emulator.Step().Should().Equal(ActionRequired.None);
        emulator.Pins.Should().Equal(Z80Pins.IORQ | Z80Pins.RD);
    }

    [Test]
    public void Pins_InterruptAcknowledge()
    {
        var emulator = new Z80StepEmulator
        {
            Interrupts =
            {
                IM = 1,
                IFF1 = true,
                IFF2 = true
            }
        };

        // Fetch a NOP with the interrupt line asserted; the interrupt is accepted at the instruction boundary.
        emulator.Step().Should().Equal(ActionRequired.OpcodeRead);
        emulator.Data = 0x00;
        emulator.Interrupts.Interrupt = true;
        emulator.Step();
        emulator.Step();
        emulator.Step();

        // The interrupt acknowledge is an M1 cycle with IORQ instead of MREQ and the interrupted PC on the address bus.
        emulator.Step().Should().Equal(ActionRequired.None);
        emulator.Pins.Should().Equal(Z80Pins.M1);
        emulator.Address.Should().Equal(emulator.Registers.PC);
        emulator.Step().Should().Equal(ActionRequired.IORead);
        emulator.Pins.Should().Equal(Z80Pins.M1 | Z80Pins.IORQ);
        emulator.Data = 0xFF;
        emulator.Step().Should().Equal(ActionRequired.None);
        emulator.Pins.Should().Equal(Z80Pins.M1 | Z80Pins.IORQ);
        emulator.Step().Should().Equal(ActionRequired.None);
        emulator.Pins.Should().Equal(Z80Pins.RFSH);
        emulator.Step().Should().Equal(ActionRequired.None);
        emulator.Pins.Should().Equal(Z80Pins.RFSH | Z80Pins.MREQ);
    }

    private static ActionRequired Step(Z80StepEmulator emulator, byte[] memory)
    {
        var actionRequired = emulator.Step();
        switch (actionRequired)
        {
            case ActionRequired.OpcodeRead:
            case ActionRequired.MemoryRead:
                emulator.Data = memory[emulator.Address];
                break;

            case ActionRequired.MemoryWrite:
                memory[emulator.Address] = emulator.Data;
                break;

            case ActionRequired.IORead:
                emulator.Data = 0xA5;
                break;
        }

        return actionRequired;
    }
}