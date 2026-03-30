using MrKWatkins.EmulatorTestSuites.Z80;
using MrKWatkins.OakCpu.Z80.Testing;

namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class InterruptBoundaryTests
{
    [TestCase((byte)0, (byte)0x46)]
    [TestCase((byte)1, (byte)0x56)]
    [TestCase((byte)2, (byte)0x5E)]
    public void Step_InterruptAfterOverlappedInstruction_CompletesOverlapBeforeInterruptHandling(byte mode, byte interruptModeOpcode)
    {
        var z80 = new Z80StepEmulatorTestHarness
        {
            RecordCycles = true,
            RegisterSP = 0x0100
        };

        Load(z80, 0x0000,
        [
            0xFB,             // EI
            0xED, interruptModeOpcode,
            0x3C,             // INC A
            0xE7              // RST 0x20
        ]);

        Load(z80, 0x0038,
        [
            0xED, 0x4D // RETI
        ]);

        z80.Step(4); // EI
        z80.Step(8); // IM mode
        z80.TStates.Should().Equal(12UL);
        z80.IFF1.Should().BeTrue();
        z80.IFF2.Should().BeTrue();

        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);

        z80.Interrupt = true;

        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterA.Should().Equal(0);

        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterA.Should().Equal(1);
        z80.IFF1.Should().BeFalse();
        z80.IFF2.Should().BeFalse();

        StepAndAssertEvent(z80, CycleType.IORead);
    }

    [Test]
    public void ExecuteInstruction_InterruptHeldWhileDisabled_PersistsUntilInstructionAfterEiCompletes()
    {
        var z80 = new Z80StepEmulatorTestHarness
        {
            IM = 1
        };

        Load(z80, 0x0000,
        [
            0xF3,             // DI
            0xFB,             // EI
            0xC3, 0x05, 0x00, // JP 0x0005
            0x00              // NOP
        ]);

        z80.Interrupt = true;

        z80.ExecuteInstruction(); // DI
        z80.Interrupt.Should().BeTrue();
        z80.IFF1.Should().BeFalse();
        z80.IFF2.Should().BeFalse();

        z80.ExecuteInstruction(); // EI
        z80.Interrupt.Should().BeTrue();
        z80.IFF1.Should().BeTrue();
        z80.IFF2.Should().BeTrue();

        z80.ExecuteInstruction(); // JP 0x0005
        z80.Interrupt.Should().BeTrue();
        z80.Emulator.CurrentStep.Should().Equal(Z80StepEmulator.IM1Start);
        z80.RegisterPC.Should().Equal(0x0005);

        z80.Step();
        z80.IFF1.Should().BeFalse();
        z80.IFF2.Should().BeFalse();
    }

    private static void Load(Z80StepEmulatorTestHarness z80, ushort address, ReadOnlySpan<byte> bytes)
    {
        for (var i = 0; i < bytes.Length; i++)
        {
            z80.WriteByteToMemory((ushort)(address + i), bytes[i]);
        }
    }

    private static void StepAndAssertEvent(Z80StepEmulatorTestHarness z80, CycleType expectedType)
    {
        z80.Step();
        z80.Cycles[^1].Type.Should().Equal(expectedType);
    }
}