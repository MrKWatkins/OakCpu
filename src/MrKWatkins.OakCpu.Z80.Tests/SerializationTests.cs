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
        copy.Should().BeEquivalentTo(original);
    }

    [Test]
    public void Serialize_Deserialize()
    {
        var original = CreateRandomEmulator();

        using var stream = new MemoryStream();
        original.Serialize(stream);
        stream.Position = 0;

        var copy = Z80Emulator.Deserialize(stream);
        copy.Should().BeEquivalentTo(original);
    }

    [Test]
    public void Serialization_includes_internal_state()
    {
        var original = new Z80Emulator();
        var originalHarness = new Z80EmulatorTestHarness(original);

        // IM 1.
        originalHarness.SetByteInMemory(0x0000, 0xED);
        originalHarness.SetByteInMemory(0x0001, 0x56);

        // 5 steps to read the 0xED and request the next opcode.
        originalHarness.Step(5);
        original.Address.Should().Be(0x0001);

        using var stream = new MemoryStream();
        original.Serialize(stream);
        stream.Position = 0;

        var copy = Z80Emulator.Deserialize(stream);
        var copyHarness = new Z80EmulatorTestHarness(copy);
        copyHarness.SetByteInMemory(0x0000, 0xED);
        copyHarness.SetByteInMemory(0x0000, 0x56);

        // 4 steps to finish reading the 0x56 and perform the instruction. (Instruction is performed with an overlapped read)
        copyHarness.Step(4);
        copy.Address.Should().Be(0x0002);
        copy.Interrupts.IM.Should().Be(1);
    }

    [Pure]
    private static Z80Emulator CreateRandomEmulator() =>
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