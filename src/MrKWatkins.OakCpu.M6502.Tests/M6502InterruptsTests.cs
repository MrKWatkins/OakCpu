namespace MrKWatkins.OakCpu.M6502.Tests;

public sealed class M6502InterruptsTests
{
    [Test]
    public void Step_NMITaken()
    {
        var rig = new StepRig([0xEA, 0xEA, 0xEA, 0xEA], [0x40]);

        rig.AssertStep(ActionRequired.OpcodeRead, 0x0000, 0xEA);
        rig.AssertStep(ActionRequired.None);

        rig.Emulator.Interrupts.NMI = true;
        rig.AssertStep(ActionRequired.OpcodeRead, 0x0001, 0xEA);
        rig.Emulator.Interrupts.NMI = false;
        rig.AssertStep(ActionRequired.None);

        rig.AssertStep(ActionRequired.MemoryRead, 0x0002, 0xEA);
        rig.AssertStep(ActionRequired.MemoryWrite, 0x01BD, 0x00);
        rig.AssertStep(ActionRequired.MemoryWrite, 0x01BC, 0x02);
        rig.AssertStep(ActionRequired.MemoryWrite, 0x01BB, 0x24);
        rig.AssertStep(ActionRequired.MemoryRead, 0xFFFA, 0x00);
        rig.AssertStep(ActionRequired.MemoryRead, 0xFFFB, 0x01);
        rig.AssertStep(ActionRequired.None);
        rig.AssertStep(ActionRequired.OpcodeRead, 0x0100, 0x40);
        rig.AssertStep(ActionRequired.None);
        rig.AssertStep(ActionRequired.MemoryRead, 0x0101, 0x00);
        rig.AssertStep(ActionRequired.MemoryRead, 0x01BA, 0x00);
        rig.AssertStep(ActionRequired.MemoryRead, 0x01BB, 0x24);
        rig.AssertStep(ActionRequired.MemoryRead, 0x01BC, 0x02);
        rig.AssertStep(ActionRequired.MemoryRead, 0x01BD, 0x00);
        rig.AssertStep(ActionRequired.OpcodeRead, 0x0002, 0xEA);
    }
    [Test]
    public void Step_NMIDelayed()
    {
        var rig = new StepRig([0xEA, 0xEA, 0xEA, 0xEA], [0x40]);

        rig.AssertStep(ActionRequired.OpcodeRead, 0x0000, 0xEA);
        rig.AssertStep(ActionRequired.None);
        rig.AssertStep(ActionRequired.OpcodeRead, 0x0001, 0xEA);
        rig.Emulator.Interrupts.NMI = true;
        rig.AssertStep(ActionRequired.None);
        rig.Emulator.Interrupts.NMI = false;

        rig.AssertStep(ActionRequired.OpcodeRead, 0x0002, 0xEA);
        rig.AssertStep(ActionRequired.None);

        rig.AssertStep(ActionRequired.MemoryRead, 0x0003, 0xEA);
        rig.AssertStep(ActionRequired.MemoryWrite, 0x01BD, 0x00);
        rig.AssertStep(ActionRequired.MemoryWrite, 0x01BC, 0x03);
        rig.AssertStep(ActionRequired.MemoryWrite, 0x01BB, 0x24);
        rig.AssertStep(ActionRequired.MemoryRead, 0xFFFA, 0x00);
        rig.AssertStep(ActionRequired.MemoryRead, 0xFFFB, 0x01);
        rig.AssertStep(ActionRequired.None);
        rig.AssertStep(ActionRequired.OpcodeRead, 0x0100, 0x40);
        rig.AssertStep(ActionRequired.None);
        rig.AssertStep(ActionRequired.MemoryRead, 0x0101, 0x00);
        rig.AssertStep(ActionRequired.MemoryRead, 0x01BA, 0x00);
        rig.AssertStep(ActionRequired.MemoryRead, 0x01BB, 0x24);
        rig.AssertStep(ActionRequired.MemoryRead, 0x01BC, 0x03);
        rig.AssertStep(ActionRequired.MemoryRead, 0x01BD, 0x00);
        rig.AssertStep(ActionRequired.OpcodeRead, 0x0003, 0xEA);
    }

    [Test]
    public void Step_IRQTaken()
    {
        var rig = new StepRig([0x58, 0xEA, 0xEA, 0xEA, 0xEA], [0x40]);

        rig.AssertStep(ActionRequired.OpcodeRead, 0x0000, 0x58);
        rig.AssertStep(ActionRequired.None);
        rig.AssertStep(ActionRequired.OpcodeRead, 0x0001, 0xEA);
        rig.AssertStep(ActionRequired.None);

        rig.Emulator.Interrupts.IRQ = true;
        rig.AssertStep(ActionRequired.OpcodeRead, 0x0002, 0xEA);
        rig.Emulator.Interrupts.IRQ = false;
        rig.AssertStep(ActionRequired.None);

        rig.AssertStep(ActionRequired.MemoryRead, 0x0003, 0xEA);
        rig.AssertStep(ActionRequired.MemoryWrite, 0x01BD, 0x00);
        rig.AssertStep(ActionRequired.MemoryWrite, 0x01BC, 0x03);
        rig.AssertStep(ActionRequired.MemoryWrite, 0x01BB, 0x20);
        rig.AssertStep(ActionRequired.MemoryRead, 0xFFFE, 0x00);
        rig.AssertStep(ActionRequired.MemoryRead, 0xFFFF, 0x01);
        rig.AssertStep(ActionRequired.None);
        rig.AssertStep(ActionRequired.OpcodeRead, 0x0100, 0x40);
        rig.AssertStep(ActionRequired.None);
        rig.AssertStep(ActionRequired.MemoryRead, 0x0101, 0x00);
        rig.AssertStep(ActionRequired.MemoryRead, 0x01BA, 0x00);
        rig.AssertStep(ActionRequired.MemoryRead, 0x01BB, 0x20);
        rig.AssertStep(ActionRequired.MemoryRead, 0x01BC, 0x03);
        rig.AssertStep(ActionRequired.MemoryRead, 0x01BD, 0x00);
        rig.AssertStep(ActionRequired.OpcodeRead, 0x0003, 0xEA);
    }

    [Test]
    public void Step_IRQNotTaken()
    {
        var rig = new StepRig([0x58, 0xEA, 0xEA, 0xEA, 0xEA], [0x40]);

        rig.AssertStep(ActionRequired.OpcodeRead, 0x0000, 0x58);
        rig.AssertStep(ActionRequired.None);
        rig.AssertStep(ActionRequired.OpcodeRead, 0x0001, 0xEA);
        rig.AssertStep(ActionRequired.None);
        rig.AssertStep(ActionRequired.OpcodeRead, 0x0002, 0xEA);
        rig.Emulator.Interrupts.IRQ = true;
        rig.AssertStep(ActionRequired.None);
        rig.Emulator.Interrupts.IRQ = false;
        rig.AssertStep(ActionRequired.OpcodeRead, 0x0003, 0xEA);
        rig.AssertStep(ActionRequired.None);
        rig.AssertStep(ActionRequired.OpcodeRead, 0x0004, 0xEA);
        rig.AssertStep(ActionRequired.None);

        rig.Emulator.Registers.S.Should().Equal(0xBD);
        rig.Emulator.Registers.PC.Should().Equal(0x0005);
    }

    private sealed class StepRig
    {
        private readonly byte[] memory = new byte[65536];

        internal StepRig(ReadOnlySpan<byte> program, ReadOnlySpan<byte> isr)
        {
            Emulator = new M6502StepEmulator();
            Emulator.Reset();
            Emulator.Registers.P = 0x24;
            Emulator.Registers.S = 0xBD;
            Load(0x0000, program);
            Load(0x0100, isr);
            WriteWord(0xFFFA, 0x0100);
            WriteWord(0xFFFE, 0x0100);
        }

        internal M6502StepEmulator Emulator { get; }

        internal void AssertStep(ActionRequired expectedActionRequired, ushort? expectedAddress = null, byte? expectedData = null)
        {
            var actionRequired = Emulator.Step();
            byte? actualData = null;

            switch (actionRequired)
            {
                case ActionRequired.OpcodeRead:
                case ActionRequired.MemoryRead:
                    Emulator.Data = memory[Emulator.Address];
                    actualData = Emulator.Data;
                    break;

                case ActionRequired.MemoryWrite:
                    memory[Emulator.Address] = Emulator.Data;
                    actualData = Emulator.Data;
                    break;
            }

            actionRequired.Should().Equal(expectedActionRequired);
            if (expectedAddress.HasValue)
            {
                Emulator.Address.Should().Equal(expectedAddress.Value);
            }

            if (expectedData.HasValue)
            {
                actualData.Should().Equal(expectedData.Value);
            }
        }

        private void Load(ushort startAddress, ReadOnlySpan<byte> bytes)
        {
            bytes.CopyTo(memory.AsSpan(startAddress));
        }

        private void WriteWord(ushort address, ushort value)
        {
            memory[address] = (byte)value;
            memory[(address + 1) & 0xFFFF] = (byte)(value >> 8);
        }
    }
}