using MrKWatkins.EmulatorTestSuites.M6502;
using MrKWatkins.OakCpu.M6502.Testing;

namespace MrKWatkins.OakCpu.M6502.Tests;

public sealed class M6502InstructionEmulatorTests
{
    static M6502InstructionEmulatorTests()
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
        var emulator = new M6502InstructionEmulator();

        emulator.Registers.A.Should().Equal(0x00);
        emulator.Registers.P.Should().Equal(0x00);
        emulator.Registers.PC.Should().Equal(0x0000);

        emulator.Registers.A = 0x12;
        emulator.A.Should().Equal(0x12);

        emulator.Registers.P = 0x34;
        emulator.P.Should().Equal(0x34);

        emulator.Registers.PC = 0x5678;
        emulator.PC.Should().Equal(0x5678);
    }

    [Test]
    public void Flags()
    {
        var emulator = new M6502InstructionEmulator();

        emulator.Flags.C.Should().BeFalse();
        emulator.Flags.Z.Should().BeFalse();
        emulator.Flags.N.Should().BeFalse();

        emulator.Flags.C = true;
        emulator.Flags.Z = true;
        emulator.Flags.N = true;

        emulator.P.Should().Equal(0b10000011);

        emulator.Flags.Z = false;
        emulator.P.Should().Equal(0b10000001);
    }

    [Test]
    public void ExecuteInstruction_Nop()
    {
        var m6502 = new M6502InstructionEmulatorTestHarness { RecordCycles = true };
        m6502.CopyToMemory(0x0000, [0xEA, 0x99]);

        m6502.ExecuteInstruction();

        m6502.TStates.Should().Equal(2);
        m6502.RegisterPC.Should().Equal(0x0001);
        m6502.RegisterA.Should().Equal(0x00);
        m6502.Cycles.Should().SequenceEqual(
        [
            new MrKWatkins.EmulatorTestSuites.M6502.Cycle(MrKWatkins.EmulatorTestSuites.M6502.CycleType.Read, 0, 0x0000, 0xEA),
            new MrKWatkins.EmulatorTestSuites.M6502.Cycle(MrKWatkins.EmulatorTestSuites.M6502.CycleType.Read, 1, 0x0001, 0x99)
        ]);
    }

    [Test]
    public void ExecuteInstruction_Lda_Immediate()
    {
        var m6502 = new M6502InstructionEmulatorTestHarness { RecordCycles = true };
        m6502.CopyToMemory(0x0000, [0xA9, 0xCC]);

        m6502.ExecuteInstruction();

        m6502.TStates.Should().Equal(2);
        m6502.RegisterA.Should().Equal(0xCC);
        m6502.RegisterPC.Should().Equal(0x0002);
        m6502.FlagN.Should().BeTrue();
        m6502.FlagZ.Should().BeFalse();
        m6502.Cycles.Should().SequenceEqual(new Cycle(CycleType.Read, 0, 0x0000, 0xA9), new Cycle(CycleType.Read, 1, 0x0001, 0xCC));
    }
}