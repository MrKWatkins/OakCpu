using MrKWatkins.OakCpu.Z80.Testing;
using MrKWatkins.EmulatorTestSuites.Z80;

namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class InstructionEmulatorEquivalenceTests
{
    [Test]
    public void ExecuteInstruction_NoOverlappedRead() =>
        AssertEquivalentAfterInstructions(
            setup =>
            {
                setup.WriteByteToMemory(0x0000, 0x06);
                setup.WriteByteToMemory(0x0001, 0x42);
            });

    [Test]
    public void ExecuteInstruction_OverlappedRead() =>
        AssertEquivalentAfterInstructions(
            setup => setup.WriteByteToMemory(0x0000, 0x04));

    [Test]
    public void ExecuteInstruction_HaltAndHaltedCycle() =>
        AssertEquivalentAfterInstructions(
            setup => setup.WriteByteToMemory(0x0000, 0x76),
            instructionCount: 2);

    [Test]
    public void ExecuteInstruction_PrefixedAlternativeFollowedByFlagInstruction() =>
        AssertEquivalentAfterInstructions(
            setup =>
            {
                setup.RegisterPC = 0x8335;
                setup.RegisterAF = 0xA3C5;
                setup.RegisterQ = 0xC5;
                setup.WriteByteToMemory(0x8335, 0xED);
                setup.WriteByteToMemory(0x8336, 0x06);
                setup.WriteByteToMemory(0x8337, 0x3F);
            },
            instructionCount: 2);

    [Test]
    public void ExecuteInstruction_IndexedHighRegisterInstruction() =>
        AssertEquivalentAfterInstructions(
            setup =>
            {
                setup.RegisterAF = 0x1200;
                setup.RegisterIX = 0x3400;
                setup.WriteByteToMemory(0x0000, 0xDD);
                setup.WriteByteToMemory(0x0001, 0x84);
            });

    [Test]
    public void ExecuteInstruction_IndexedDisplacementRead() =>
        AssertEquivalentAfterInstructions(
            setup =>
            {
                setup.RegisterAF = 0x1200;
                setup.RegisterIX = 0x4000;
                setup.WriteByteToMemory(0x0000, 0xDD);
                setup.WriteByteToMemory(0x0001, 0x86);
                setup.WriteByteToMemory(0x0002, 0x05);
                setup.WriteByteToMemory(0x4005, 0x34);
            });

    [Test]
    public void ExecuteInstruction_InterruptAfterOverlappedInstruction_CompletesOverlapBeforeInterruptHandling() =>
        AssertEquivalentAfterInstructions(
            setup =>
            {
                setup.RegisterSP = 0x0100;
                setup.WriteByteToMemory(0x0000, 0xFB);
                setup.WriteByteToMemory(0x0001, 0xED);
                setup.WriteByteToMemory(0x0002, 0x56);
                setup.WriteByteToMemory(0x0003, 0x3C);
                setup.WriteByteToMemory(0x0004, 0x00);
                setup.WriteByteToMemory(0x0038, 0xED);
                setup.WriteByteToMemory(0x0039, 0x4D);
            },
            instructionCount: 2,
            setInterruptAfterInstruction: 2);

    [Test]
    public void ExecuteInstruction_PrefixTableResetAfterDDPrefixedInstruction() =>
        AssertEquivalentAfterInstructions(
            setup =>
            {
                setup.RegisterAF = 0x1200;
                setup.RegisterIX = 0x3400;
                setup.WriteByteToMemory(0x0000, 0xDD);
                setup.WriteByteToMemory(0x0001, 0x84);
                setup.WriteByteToMemory(0x0002, 0x3C);
            },
            instructionCount: 2);

    [Test]
    public void ExecuteInstruction_PrefixTableResetAfterFDPrefixedInstruction() =>
        AssertEquivalentAfterInstructions(
            setup =>
            {
                setup.RegisterAF = 0x1200;
                setup.RegisterIY = 0x5600;
                setup.WriteByteToMemory(0x0000, 0xFD);
                setup.WriteByteToMemory(0x0001, 0x85);
                setup.WriteByteToMemory(0x0002, 0x04);
            },
            instructionCount: 2);

    [Test]
    public void ExecuteInstruction_PrefixTableResetAfterCBPrefixedInstruction() =>
        AssertEquivalentAfterInstructions(
            setup =>
            {
                setup.RegisterAF = 0x0100;
                setup.WriteByteToMemory(0x0000, 0xCB);
                setup.WriteByteToMemory(0x0001, 0x00);
                setup.WriteByteToMemory(0x0002, 0x3C);
            },
            instructionCount: 2);

    [Test]
    public void ExecuteInstruction_OverlapFoldingAcrossInstructionBoundary() =>
        AssertEquivalentAfterInstructions(
            setup =>
            {
                setup.WriteByteToMemory(0x0000, 0x3C);
                setup.WriteByteToMemory(0x0001, 0x04);
            },
            instructionCount: 2);

    [Test]
    public void ExecuteInstruction_HaltFollowedByInterrupt() =>
        AssertEquivalentAfterInstructions(
            setup =>
            {
                setup.RegisterSP = 0x0100;
                setup.WriteByteToMemory(0x0000, 0xFB);
                setup.WriteByteToMemory(0x0001, 0xED);
                setup.WriteByteToMemory(0x0002, 0x56);
                setup.WriteByteToMemory(0x0003, 0x76);
                setup.WriteByteToMemory(0x0038, 0xED);
                setup.WriteByteToMemory(0x0039, 0x4D);
            },
            instructionCount: 4,
            setInterruptAfterInstruction: 3);

    private static void AssertEquivalentAfterInstructions(Action<Z80TestHarness> setup, int instructionCount = 1, int? setInterruptAfterInstruction = null)
    {
        var step = new Z80StepEmulatorTestHarness { RecordCycles = true };
        var instruction = new Z80InstructionEmulatorTestHarness { RecordCycles = true };

        setup(step);
        setup(instruction);

        for (var i = 0; i < instructionCount; i++)
        {
            if (setInterruptAfterInstruction.HasValue && i == setInterruptAfterInstruction.Value)
            {
                step.Interrupt = true;
                instruction.Interrupt = true;
            }

            var instructionStartTStates = instruction.TStates;
            instruction.ExecuteInstruction();

            ((ulong)instruction.LastInstructionTStates).Should().Equal(instruction.TStates - instructionStartTStates);

            while (step.TStates < instruction.TStates)
            {
                step.ExecuteInstruction();
            }

            step.TStates.Should().Equal(instruction.TStates);
        }

        AssertEqual(instruction, step);
        AssertCyclesEqual(instruction, step);
        AssertMemoryEqual(instruction, step);
    }

    private static void AssertEqual(Z80InstructionEmulatorTestHarness actual, Z80StepEmulatorTestHarness expected)
    {
        actual.RegisterAF.Should().Equal(expected.RegisterAF);
        actual.RegisterBC.Should().Equal(expected.RegisterBC);
        actual.RegisterDE.Should().Equal(expected.RegisterDE);
        actual.RegisterHL.Should().Equal(expected.RegisterHL);
        actual.RegisterIX.Should().Equal(expected.RegisterIX);
        actual.RegisterIY.Should().Equal(expected.RegisterIY);
        actual.RegisterI.Should().Equal(expected.RegisterI);
        actual.RegisterR.Should().Equal(expected.RegisterR);
        actual.RegisterPC.Should().Equal(expected.RegisterPC);
        actual.RegisterSP.Should().Equal(expected.RegisterSP);
        actual.RegisterWZ.Should().Equal(expected.RegisterWZ);
        actual.RegisterQ.Should().Equal(expected.RegisterQ);
        actual.ShadowRegisterAF.Should().Equal(expected.ShadowRegisterAF);
        actual.ShadowRegisterBC.Should().Equal(expected.ShadowRegisterBC);
        actual.ShadowRegisterDE.Should().Equal(expected.ShadowRegisterDE);
        actual.ShadowRegisterHL.Should().Equal(expected.ShadowRegisterHL);
        actual.IFF1.Should().Equal(expected.IFF1);
        actual.IFF2.Should().Equal(expected.IFF2);
        actual.IM.Should().Equal(expected.IM);
        actual.Halted.Should().Equal(expected.Halted);
        actual.Interrupt.Should().Equal(expected.Interrupt);
    }

    private static void AssertMemoryEqual(Z80InstructionEmulatorTestHarness actual, Z80StepEmulatorTestHarness expected)
    {
        for (ushort address = 0x0000; address < ushort.MaxValue; address++)
        {
            Assert.That(actual.ReadByteFromMemory(address), Is.EqualTo(expected.ReadByteFromMemory(address)), $"Mismatch at 0x{address:X4}");
        }

        actual.ReadByteFromMemory(ushort.MaxValue).Should().Equal(expected.ReadByteFromMemory(ushort.MaxValue));
    }

    private static void AssertCyclesEqual(Z80InstructionEmulatorTestHarness actual, Z80StepEmulatorTestHarness expected)
    {
        actual.TStates.Should().Equal(expected.TStates);

        var expectedCycles = expected.Cycles.Where(cycle => cycle.Type != CycleType.None).ToList();

        actual.Cycles.Count.Should().Equal(expectedCycles.Count);

        for (var i = 0; i < actual.Cycles.Count; i++)
        {
            var actualCycle = actual.Cycles[i];
            var expectedCycle = expectedCycles[i];

            actualCycle.Type.Should().Equal(expectedCycle.Type);
            actualCycle.Address.Should().Equal(expectedCycle.Address);
            actualCycle.Data.Should().Equal(expectedCycle.Data);
            actualCycle.IsOpcodeRead.Should().Equal(expectedCycle.IsOpcodeRead);
        }
    }
}