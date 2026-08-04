namespace MrKWatkins.OakCpu.Z80N.Tests;

/// <summary>
/// Tests for the Z80N instructions, checking the behaviour and T-states decoded from the ZX Spectrum Next FPGA core. The T-states
/// are cross-checked against https://wiki.specnext.dev/Extended_Z80_instruction_set.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class InstructionTests
{
    [Test]
    public void InheritedZ80Instructions()
    {
        // The Z80N inherits everything the Z80 does; NOP and LDI should be untouched.
        var machine = new TestMachine();
        machine.Load(0x0000, 0x00);
        machine.ExecuteInstruction().Should().Equal(4);

        machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0xA0);
        machine.Emulator.HL = 0x1234;
        machine.Emulator.DE = 0x5678;
        machine.Emulator.BC = 0x0002;
        machine[0x1234] = 0x42;
        machine.ExecuteInstruction().Should().Equal(16);
        machine[0x5678].Should().Equal(0x42);
    }

    [Test]
    public void SWAPNIB()
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0x23);
        machine.Emulator.A = 0x12;

        machine.ExecuteInstruction().Should().Equal(8);
        machine.Emulator.A.Should().Equal(0x21);
    }

    [TestCase((byte)0x01, (byte)0x80)]
    [TestCase((byte)0x80, (byte)0x01)]
    [TestCase((byte)0b1010_0000, (byte)0b0000_0101)]
    [TestCase((byte)0xFF, (byte)0xFF)]
    public void MIRROR_A(byte value, byte expected)
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0x24);
        machine.Emulator.A = value;

        machine.ExecuteInstruction().Should().Equal(8);
        machine.Emulator.A.Should().Equal(expected);
    }

    [TestCase((byte)0x00, (byte)0x05, (ushort)0x0000)]
    [TestCase((byte)0x05, (byte)0x00, (ushort)0x0000)]
    [TestCase((byte)0x03, (byte)0x04, (ushort)0x000C)]
    [TestCase((byte)0xFF, (byte)0xFF, (ushort)0xFE01)]
    public void MUL_D_E(byte d, byte e, ushort expected)
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0x30);
        machine.Emulator.D = d;
        machine.Emulator.E = e;

        machine.ExecuteInstruction().Should().Equal(8);
        machine.Emulator.DE.Should().Equal(expected);
    }

    [TestCase((byte)0x31, (ushort)0x1234, (ushort)0x1244)]
    [TestCase((byte)0x32, (ushort)0xFFFF, (ushort)0x000F)]
    [TestCase((byte)0x33, (ushort)0x0000, (ushort)0x0010)]
    public void ADD_RegisterPair_A(byte opcode, ushort registerPair, ushort expected)
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, opcode);
        machine.Emulator.A = 0x10;
        machine.Emulator.F = 0xFF;
        SetRegisterPair(machine, opcode, registerPair);

        machine.ExecuteInstruction().Should().Equal(8);
        GetRegisterPair(machine, opcode).Should().Equal(expected);

        // The core reads the carry out from a bit its sixteen bit addition never sets, so the carry flag is always cleared.
        machine.Emulator.Flags.C.Should().BeFalse();
        machine.Emulator.Flags.Z.Should().BeTrue();
        machine.Emulator.Flags.S.Should().BeTrue();
    }

    [TestCase((byte)0x34, (ushort)0x1234, (ushort)0x1357)]
    [TestCase((byte)0x35, (ushort)0xFFFF, (ushort)0x0122)]
    [TestCase((byte)0x36, (ushort)0x0000, (ushort)0x0123)]
    public void ADD_RegisterPair_Immediate(byte opcode, ushort registerPair, ushort expected)
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, opcode, 0x23, 0x01);
        machine.Emulator.F = 0xFF;
        SetRegisterPair(machine, opcode, registerPair);

        machine.ExecuteInstruction().Should().Equal(16);
        GetRegisterPair(machine, opcode).Should().Equal(expected);

        // The immediate is loaded into WZ on the way through, and no flags are written.
        machine.Emulator.WZ.Should().Equal(0x0123);
        machine.Emulator.F.Should().Equal(0xFF);
    }

    [Test]
    public void TEST()
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0x27, 0x0F);
        machine.Emulator.A = 0x33;

        machine.ExecuteInstruction().Should().Equal(11);

        // A is left alone; the flags are those of AND with the result discarded.
        machine.Emulator.A.Should().Equal(0x33);
        machine.Emulator.Flags.C.Should().BeFalse();
        machine.Emulator.Flags.N.Should().BeFalse();
        machine.Emulator.Flags.H.Should().BeTrue();
        machine.Emulator.Flags.Z.Should().BeFalse();
        machine.Emulator.Flags.S.Should().BeFalse();
        machine.Emulator.Flags.PV.Should().BeTrue();
    }

    [Test]
    public void TEST_Zero()
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0x27, 0xF0);
        machine.Emulator.A = 0x0F;

        machine.ExecuteInstruction().Should().Equal(11);
        machine.Emulator.Flags.Z.Should().BeTrue();
    }

    [TestCase((byte)0x28, (ushort)0x1234, (byte)4, (ushort)0x2340)]  // BSLA.
    [TestCase((byte)0x28, (ushort)0x1234, (byte)16, (ushort)0x0000)] // Shifts of sixteen or more shift everything out.
    [TestCase((byte)0x28, (ushort)0x1234, (byte)0, (ushort)0x1234)]
    [TestCase((byte)0x29, (ushort)0x8000, (byte)4, (ushort)0xF800)]  // BSRA fills with the sign bit.
    [TestCase((byte)0x29, (ushort)0x8000, (byte)16, (ushort)0xFFFF)]
    [TestCase((byte)0x29, (ushort)0x4000, (byte)4, (ushort)0x0400)]
    [TestCase((byte)0x29, (ushort)0x8000, (byte)0, (ushort)0x8000)]
    [TestCase((byte)0x2A, (ushort)0x8000, (byte)4, (ushort)0x0800)]  // BSRL fills with zeroes.
    [TestCase((byte)0x2A, (ushort)0x8000, (byte)16, (ushort)0x0000)]
    [TestCase((byte)0x2B, (ushort)0x0000, (byte)4, (ushort)0xF000)]  // BSRF fills with ones.
    [TestCase((byte)0x2B, (ushort)0x0000, (byte)16, (ushort)0xFFFF)]
    [TestCase((byte)0x2B, (ushort)0x1234, (byte)0, (ushort)0x1234)]
    [TestCase((byte)0x2C, (ushort)0x1234, (byte)4, (ushort)0x2341)]  // BRLC rotates.
    [TestCase((byte)0x2C, (ushort)0x1234, (byte)16, (ushort)0x1234)] // Rotating a sixteen bit value is modulo sixteen.
    [TestCase((byte)0x2C, (ushort)0x1234, (byte)0, (ushort)0x1234)]
    public void BarrelShifts(byte opcode, ushort de, byte b, ushort expected)
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, opcode);
        machine.Emulator.DE = de;
        machine.Emulator.B = b;
        machine.Emulator.F = 0xFF;

        machine.ExecuteInstruction().Should().Equal(8);
        machine.Emulator.DE.Should().Equal(expected);
        machine.Emulator.F.Should().Equal(0xFF);
    }

    [TestCase((byte)0x00, (byte)0x80)]
    [TestCase((byte)0x07, (byte)0x01)]
    [TestCase((byte)0x0A, (byte)0x20)]
    public void SETAE(byte e, byte expected)
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0x95);
        machine.Emulator.E = e;

        machine.ExecuteInstruction().Should().Equal(8);
        machine.Emulator.A.Should().Equal(expected);
    }

    [Test]
    public void PIXELAD()
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0x94);
        machine.Emulator.D = 0;
        machine.Emulator.E = 0;

        machine.ExecuteInstruction().Should().Equal(8);
        machine.Emulator.HL.Should().Equal(0x4000);

        // Row 128 is the start of the third third of the display, column 8 the second byte along.
        machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0x94);
        machine.Emulator.D = 128;
        machine.Emulator.E = 8;
        machine.ExecuteInstruction().Should().Equal(8);
        machine.Emulator.HL.Should().Equal(0x5001);
    }

    [Test]
    public void PIXELDN()
    {
        // Within a character cell moving down a line adds 0x0100.
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0x93);
        machine.Emulator.HL = 0x4000;

        machine.ExecuteInstruction().Should().Equal(8);
        machine.Emulator.HL.Should().Equal(0x4100);

        // At the bottom of a character cell it moves to the top of the next one down.
        machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0x93);
        machine.Emulator.HL = 0x4700;
        machine.ExecuteInstruction().Should().Equal(8);
        machine.Emulator.HL.Should().Equal(0x4020);

        // At the bottom of a third it moves into the next third.
        machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0x93);
        machine.Emulator.HL = 0x47E0;
        machine.ExecuteInstruction().Should().Equal(8);
        machine.Emulator.HL.Should().Equal(0x4800);
    }

    [Test]
    public void PUSH_Immediate()
    {
        var machine = new TestMachine();

        // The immediate is big endian, unlike every other word the Z80 reads.
        machine.Load(0x0000, 0xED, 0x8A, 0x12, 0x34);
        machine.Emulator.SP = 0x8000;

        machine.ExecuteInstruction().Should().Equal(23);
        machine.Emulator.SP.Should().Equal(0x7FFE);
        machine[0x7FFF].Should().Equal(0x12);
        machine[0x7FFE].Should().Equal(0x34);
        machine.Emulator.PC.Should().Equal(0x0004);
    }

    [Test]
    public void NEXTREG_Immediate_Immediate()
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0x91, 0x07, 0x03);

        machine.ExecuteInstruction().Should().Equal(20);
        machine.NextRegisterWrites.Should().HaveCount(1);
        machine.NextRegisterWrites[0].Register.Should().Equal(0x07);
        machine.NextRegisterWrites[0].Value.Should().Equal(0x03);
        machine.Emulator.PC.Should().Equal(0x0004);
    }

    [Test]
    public void NEXTREG_Immediate_A()
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0x92, 0x07);
        machine.Emulator.A = 0x03;

        machine.ExecuteInstruction().Should().Equal(17);
        machine.NextRegisterWrites.Should().HaveCount(1);
        machine.NextRegisterWrites[0].Register.Should().Equal(0x07);
        machine.NextRegisterWrites[0].Value.Should().Equal(0x03);
        machine.Emulator.PC.Should().Equal(0x0003);
    }

    [Test]
    public void JP_C()
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0x98);
        machine.Emulator.BC = 0x1234;
        machine.Emulator.PC = 0x0000;
        machine.IOReadValue = 0x03;

        machine.ExecuteInstruction().Should().Equal(13);

        // The top two bits of PC are kept and the byte read supplies bits 13 to 6.
        machine.Emulator.PC.Should().Equal(0x00C0);
    }

    [Test]
    public void OUTINB()
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0x90);
        machine.Emulator.HL = 0x1234;
        machine.Emulator.BC = 0x0FFE;
        machine.Emulator.F = 0xFF;
        machine[0x1234] = 0x42;

        machine.ExecuteInstruction().Should().Equal(16);
        machine.IOWrites.Should().HaveCount(1);
        machine.IOWrites[0].Port.Should().Equal(0x0FFE);
        machine.IOWrites[0].Value.Should().Equal(0x42);

        // HL moves on but B is left alone, unlike OUTI, and no flags are written.
        machine.Emulator.HL.Should().Equal(0x1235);
        machine.Emulator.B.Should().Equal(0x0F);
        machine.Emulator.F.Should().Equal(0xFF);
    }

    [Test]
    public void LDIX()
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0xA4);
        machine.Emulator.HL = 0x1234;
        machine.Emulator.DE = 0x5678;
        machine.Emulator.BC = 0x0003;
        machine.Emulator.A = 0x00;
        machine[0x1234] = 0x42;
        machine[0x5678] = 0xFF;

        machine.ExecuteInstruction().Should().Equal(16);
        machine[0x5678].Should().Equal(0x42);
        machine.Emulator.HL.Should().Equal(0x1235);
        machine.Emulator.DE.Should().Equal(0x5679);
        machine.Emulator.BC.Should().Equal(0x0002);
    }

    [Test]
    public void LDIX_SkipsBytesMatchingA()
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0xA4);
        machine.Emulator.HL = 0x1234;
        machine.Emulator.DE = 0x5678;
        machine.Emulator.BC = 0x0003;
        machine.Emulator.A = 0x42;
        machine[0x1234] = 0x42;
        machine[0x5678] = 0xFF;

        // The transparent byte is not written, but everything else still happens and it still takes sixteen T-states.
        machine.ExecuteInstruction().Should().Equal(16);
        machine[0x5678].Should().Equal(0xFF);
        machine.Actions.Should().NotContain(ActionRequired.MemoryWrite);
        machine.Emulator.HL.Should().Equal(0x1235);
        machine.Emulator.DE.Should().Equal(0x5679);
        machine.Emulator.BC.Should().Equal(0x0002);
    }

    [Test]
    public void LDDX()
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0xAC);
        machine.Emulator.HL = 0x1234;
        machine.Emulator.DE = 0x5678;
        machine.Emulator.BC = 0x0003;
        machine.Emulator.A = 0x00;
        machine[0x1234] = 0x42;

        machine.ExecuteInstruction().Should().Equal(16);
        machine[0x5678].Should().Equal(0x42);

        // HL goes down but DE still goes up.
        machine.Emulator.HL.Should().Equal(0x1233);
        machine.Emulator.DE.Should().Equal(0x5679);
    }

    [Test]
    public void LDIRX()
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0xB4);
        machine.Emulator.HL = 0x1234;
        machine.Emulator.DE = 0x5678;
        machine.Emulator.BC = 0x0002;
        machine.Emulator.A = 0x00;
        machine[0x1234] = 0x42;
        machine[0x1235] = 0x43;

        // Iterations that repeat take five T-states longer than the last one.
        machine.ExecuteInstruction().Should().Equal(21);
        machine.Emulator.BC.Should().Equal(0x0001);
        machine.Emulator.PC.Should().Equal(0x0000);

        machine.ExecuteInstruction().Should().Equal(16);
        machine.Emulator.BC.Should().Equal(0x0000);
        machine.Emulator.PC.Should().Equal(0x0002);

        machine[0x5678].Should().Equal(0x42);
        machine[0x5679].Should().Equal(0x43);
    }

    [Test]
    public void LDPIRX()
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0xB7);

        // The byte read comes from the aligned eight byte block at HL, at the offset within it that E gives.
        machine.Emulator.HL = 0x1234;
        machine.Emulator.DE = 0x567D;
        machine.Emulator.BC = 0x0001;
        machine.Emulator.A = 0x00;
        machine[0x1235] = 0x42;

        machine.ExecuteInstruction().Should().Equal(16);
        machine[0x567D].Should().Equal(0x42);

        // HL is left alone.
        machine.Emulator.HL.Should().Equal(0x1234);
        machine.Emulator.DE.Should().Equal(0x567E);
    }

    [Test]
    public void LDWS()
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0xA5);
        machine.Emulator.HL = 0x1234;
        machine.Emulator.DE = 0x5678;
        machine.Emulator.BC = 0x9999;
        machine[0x1234] = 0x42;

        machine.ExecuteInstruction().Should().Equal(14);
        machine[0x5678].Should().Equal(0x42);

        // L and D are incremented, the rest of HL and DE and all of BC are left alone.
        machine.Emulator.HL.Should().Equal(0x1235);
        machine.Emulator.DE.Should().Equal(0x5778);
        machine.Emulator.BC.Should().Equal(0x9999);
    }

    [Test]
    public void LDWS_FlagsAreThoseOfIncrementingD()
    {
        var machine = new TestMachine();
        machine.Load(0x0000, 0xED, 0xA5);
        machine.Emulator.HL = 0x1234;
        machine.Emulator.DE = 0x7F78;
        machine[0x1234] = 0x42;

        machine.ExecuteInstruction().Should().Equal(14);

        // Incrementing D from 0x7F overflows and half carries.
        machine.Emulator.D.Should().Equal(0x80);
        machine.Emulator.Flags.PV.Should().BeTrue();
        machine.Emulator.Flags.H.Should().BeTrue();
        machine.Emulator.Flags.S.Should().BeTrue();
        machine.Emulator.Flags.Z.Should().BeFalse();
        machine.Emulator.Flags.N.Should().BeFalse();
    }

    private static void SetRegisterPair(TestMachine machine, byte opcode, ushort value)
    {
        switch (opcode)
        {
            case 0x31:
            case 0x34:
                machine.Emulator.HL = value;
                break;
            case 0x32:
            case 0x35:
                machine.Emulator.DE = value;
                break;
            default:
                machine.Emulator.BC = value;
                break;
        }
    }

    [Pure]
    private static ushort GetRegisterPair(TestMachine machine, byte opcode) =>
        opcode switch
        {
            0x31 or 0x34 => machine.Emulator.HL,
            0x32 or 0x35 => machine.Emulator.DE,
            _ => machine.Emulator.BC
        };
}