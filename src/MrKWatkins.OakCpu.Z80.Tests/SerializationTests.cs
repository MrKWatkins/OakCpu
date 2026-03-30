using MrKWatkins.OakCpu.Z80.Testing;
using NUnit.Framework.Internal;

namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class SerializationTests
{
    [Test]
    public void Serialize_Restore()
    {
        var original = CreateRandomEmulator();

        using var stream = new MemoryStream();
        original.Serialize(stream);
        stream.Position = 0;

        var copy = CreateRandomEmulator();
        copy.Restore(stream);
        AssertEqual(copy, original);
    }

    [Test]
    public void Serialize_Deserialize()
    {
        var original = CreateRandomEmulator();

        using var stream = new MemoryStream();
        original.Serialize(stream);
        stream.Position = 0;

        var copy = Z80StepEmulator.Deserialize(stream);
        AssertEqual(copy, original);
    }

    [Test]
    public void Serialization_includes_internal_state()
    {
        var original = new Z80StepEmulator();
        var originalHarness = new Z80StepEmulatorTestHarness(original);

        // IM 1.
        originalHarness.WriteByteToMemory(0x0000, 0xED);
        originalHarness.WriteByteToMemory(0x0001, 0x56);

        // 5 steps to read the 0xED and request the next opcode.
        originalHarness.Step(5);
        original.Address.Should().Equal(0x0001);

        using var stream = new MemoryStream();
        original.Serialize(stream);
        stream.Position = 0;

        var copy = Z80StepEmulator.Deserialize(stream);
        var copyHarness = new Z80StepEmulatorTestHarness(copy);
        copyHarness.WriteByteToMemory(0x0000, 0xED);
        copyHarness.WriteByteToMemory(0x0000, 0x56);

        // 4 steps to finish reading the 0x56 and perform the instruction. (Instruction is performed with an overlapped read)
        copyHarness.Step(4);
        copy.Address.Should().Equal(0x0002);
        copy.Interrupts.IM.Should().Equal(1);
    }

    [Test]
    public void Serialization_includes_pending_overlap_pipeline()
    {
        var original = new Z80StepEmulator();
        var originalHarness = new Z80StepEmulatorTestHarness(original);
        original.Registers.BC = 0x1234;

        originalHarness.WriteByteToMemory(0x0000, 0x04);
        originalHarness.WriteByteToMemory(0x0001, 0x00);

        originalHarness.Step(4);
        original.Registers.B.Should().Equal(0x12);

        using var stream = new MemoryStream();
        original.Serialize(stream);
        stream.Position = 0;

        var copy = Z80StepEmulator.Deserialize(stream);
        var copyHarness = new Z80StepEmulatorTestHarness(copy);
        copyHarness.WriteByteToMemory(0x0000, 0x04);
        copyHarness.WriteByteToMemory(0x0001, 0x00);

        copy.Registers.B.Should().Equal(0x12);
        copyHarness.Step();
        copy.Registers.B.Should().Equal(0x13);
        copy.Address.Should().Equal(0x0001);
    }

    [Test]
    public void Contended_serialize_deserialize()
    {
        var original = new ContendedZ80StepEmulator(new Z80StepEmulator(), tStatesInCurrentFrame: 14335)
        {
            Registers =
            {
                PC = 0x4000
            }
        };

        using var stream = new MemoryStream();
        original.Serialize(stream);
        stream.Position = 0;

        var copy = ContendedZ80StepEmulator.Deserialize(stream);
        copy.TStatesInCurrentFrame.Should().Equal(original.TStatesInCurrentFrame);
        copy.Registers.PC.Should().Equal(original.Registers.PC);
        copy.PendingDelay.Should().Equal(original.PendingDelay);
        copy.HasPendingAction.Should().Equal(original.HasPendingAction);
    }

    private static void AssertEqual(Z80StepEmulator actual, Z80StepEmulator expected)
    {
        actual.Address.Should().Equal(expected.Address);
        actual.Data.Should().Equal(expected.Data);
        actual.Registers.AF.Should().Equal(expected.Registers.AF);
        actual.Registers.BC.Should().Equal(expected.Registers.BC);
        actual.Registers.DE.Should().Equal(expected.Registers.DE);
        actual.Registers.HL.Should().Equal(expected.Registers.HL);
        actual.Registers.IX.Should().Equal(expected.Registers.IX);
        actual.Registers.IY.Should().Equal(expected.Registers.IY);
        actual.Registers.IR.Should().Equal(expected.Registers.IR);
        actual.Registers.PC.Should().Equal(expected.Registers.PC);
        actual.Registers.SP.Should().Equal(expected.Registers.SP);
        actual.Registers.WZ.Should().Equal(expected.Registers.WZ);
        actual.Registers.Q.Should().Equal(expected.Registers.Q);
        actual.Registers.Shadow.AF.Should().Equal(expected.Registers.Shadow.AF);
        actual.Registers.Shadow.BC.Should().Equal(expected.Registers.Shadow.BC);
        actual.Registers.Shadow.DE.Should().Equal(expected.Registers.Shadow.DE);
        actual.Registers.Shadow.HL.Should().Equal(expected.Registers.Shadow.HL);
        actual.Interrupts.IM.Should().Equal(expected.Interrupts.IM);
        actual.Interrupts.IFF1.Should().Equal(expected.Interrupts.IFF1);
        actual.Interrupts.IFF2.Should().Equal(expected.Interrupts.IFF2);
        actual.Interrupts.Halted.Should().Equal(expected.Interrupts.Halted);
        actual.Interrupts.Interrupt.Should().Equal(expected.Interrupts.Interrupt);
    }

    [Pure]
    private static Z80StepEmulator CreateRandomEmulator() =>
        new()
        {
            Data = Rng.NextByte(),
            Registers =
            {
                AF = Rng.NextUShort(),
                BC = Rng.NextUShort(),
                DE = Rng.NextUShort(),
                HL = Rng.NextUShort(),
                IR = Rng.NextUShort(),
                IX = Rng.NextUShort(),
                IY = Rng.NextUShort(),
                PC = Rng.NextUShort(),
                SP = Rng.NextUShort(),
                WZ = Rng.NextUShort(),
                Q = Rng.NextByte(),
                Shadow =
                {
                    AF = Rng.NextUShort(),
                    BC = Rng.NextUShort(),
                    DE = Rng.NextUShort(),
                    HL = Rng.NextUShort()
                }
            },
            Interrupts =
            {
                IM = Rng.NextByte(0, 2),
                IFF1 = Rng.NextBool(),
                IFF2 = Rng.NextBool(),
                Halted = Rng.NextBool(),
                Interrupt = Rng.NextBool()
            }
        };

    private static Randomizer Rng => TestContext.CurrentContext.Random;
}