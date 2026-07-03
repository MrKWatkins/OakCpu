using System.Reflection;
using MrKWatkins.OakCpu.Z80.Testing;
using NUnit.Framework.Internal;

namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class SerializationTests
{
    [Test]
    public void Stream_only_restore_Step() => AssertStreamOnlyRestoreRoundTrip(
        CreateRandomStepEmulator(),
        CreateRandomStepEmulator,
        static (emulator, stream) => emulator.Serialize(stream),
        static (emulator, stream) => emulator.Restore(stream),
        AssertEqual,
        Z80StepEmulator.SerializedSize);

    [Test]
    public void Stream_only_deserialize_Step() => AssertStreamOnlyDeserializeRoundTrip(
        CreateRandomStepEmulator(),
        static (emulator, stream) => emulator.Serialize(stream),
        static stream => Z80StepEmulator.Deserialize(stream),
        AssertEqual,
        Z80StepEmulator.SerializedSize);

    [Test]
    public void Span_only_restore_Step() => AssertSpanOnlyRestoreRoundTrip(
        CreateRandomStepEmulator(),
        CreateRandomStepEmulator,
        static (emulator, destination) => emulator.Serialize(destination),
        static (emulator, source) => emulator.Restore(source),
        AssertEqual,
        Z80StepEmulator.SerializedSize);

    [Test]
    public void Span_only_deserialize_Step() => AssertSpanOnlyDeserializeRoundTrip(
        CreateRandomStepEmulator(),
        static (emulator, destination) => emulator.Serialize(destination),
        static source => Z80StepEmulator.Deserialize(source),
        AssertEqual,
        Z80StepEmulator.SerializedSize);

    [Test]
    public void Stream_to_span_restore_Step() => AssertStreamToSpanRestoreRoundTrip(
        CreateRandomStepEmulator(),
        CreateRandomStepEmulator,
        static (emulator, stream) => emulator.Serialize(stream),
        static (emulator, source) => emulator.Restore(source),
        AssertEqual,
        Z80StepEmulator.SerializedSize);

    [Test]
    public void Stream_to_span_deserialize_Step() => AssertStreamToSpanDeserializeRoundTrip(
        CreateRandomStepEmulator(),
        static (emulator, stream) => emulator.Serialize(stream),
        static source => Z80StepEmulator.Deserialize(source),
        AssertEqual,
        Z80StepEmulator.SerializedSize);

    [Test]
    public void Span_to_stream_restore_Step() => AssertSpanToStreamRestoreRoundTrip(
        CreateRandomStepEmulator(),
        CreateRandomStepEmulator,
        static (emulator, destination) => emulator.Serialize(destination),
        static (emulator, stream) => emulator.Restore(stream),
        AssertEqual,
        Z80StepEmulator.SerializedSize);

    [Test]
    public void Span_to_stream_deserialize_Step() => AssertSpanToStreamDeserializeRoundTrip(
        CreateRandomStepEmulator(),
        static (emulator, destination) => emulator.Serialize(destination),
        static stream => Z80StepEmulator.Deserialize(stream),
        AssertEqual,
        Z80StepEmulator.SerializedSize);

    [Test]
    public void Serialize_span_throws_for_short_destination_Step() => Assert.Throws<ArgumentException>(
        () => CreateRandomStepEmulator().Serialize(new byte[Z80StepEmulator.SerializedSize - 1]));

    [Test]
    public void Restore_span_throws_for_short_source_Step() => Assert.Throws<ArgumentException>(
        () => CreateRandomStepEmulator().Restore(new byte[Z80StepEmulator.SerializedSize - 1]));

    [Test]
    public void Stream_only_restore_Instruction() => AssertStreamOnlyRestoreRoundTrip(
        CreateRandomInstructionEmulator(),
        CreateRandomInstructionEmulator,
        static (emulator, stream) => emulator.Serialize(stream),
        static (emulator, stream) => emulator.Restore(stream),
        AssertEqual,
        Z80InstructionEmulator.SerializedSize);

    [Test]
    public void Stream_only_deserialize_Instruction() => AssertStreamOnlyDeserializeRoundTrip(
        CreateRandomInstructionEmulator(),
        static (emulator, stream) => emulator.Serialize(stream),
        static stream => Z80InstructionEmulator.Deserialize(stream),
        AssertEqual,
        Z80InstructionEmulator.SerializedSize);

    [Test]
    public void Span_only_restore_Instruction() => AssertSpanOnlyRestoreRoundTrip(
        CreateRandomInstructionEmulator(),
        CreateRandomInstructionEmulator,
        static (emulator, destination) => emulator.Serialize(destination),
        static (emulator, source) => emulator.Restore(source),
        AssertEqual,
        Z80InstructionEmulator.SerializedSize);

    [Test]
    public void Span_only_deserialize_Instruction() => AssertSpanOnlyDeserializeRoundTrip(
        CreateRandomInstructionEmulator(),
        static (emulator, destination) => emulator.Serialize(destination),
        static source => Z80InstructionEmulator.Deserialize(source),
        AssertEqual,
        Z80InstructionEmulator.SerializedSize);

    [Test]
    public void Stream_to_span_restore_Instruction() => AssertStreamToSpanRestoreRoundTrip(
        CreateRandomInstructionEmulator(),
        CreateRandomInstructionEmulator,
        static (emulator, stream) => emulator.Serialize(stream),
        static (emulator, source) => emulator.Restore(source),
        AssertEqual,
        Z80InstructionEmulator.SerializedSize);

    [Test]
    public void Stream_to_span_deserialize_Instruction() => AssertStreamToSpanDeserializeRoundTrip(
        CreateRandomInstructionEmulator(),
        static (emulator, stream) => emulator.Serialize(stream),
        static source => Z80InstructionEmulator.Deserialize(source),
        AssertEqual,
        Z80InstructionEmulator.SerializedSize);

    [Test]
    public void Span_to_stream_restore_Instruction() => AssertSpanToStreamRestoreRoundTrip(
        CreateRandomInstructionEmulator(),
        CreateRandomInstructionEmulator,
        static (emulator, destination) => emulator.Serialize(destination),
        static (emulator, stream) => emulator.Restore(stream),
        AssertEqual,
        Z80InstructionEmulator.SerializedSize);

    [Test]
    public void Span_to_stream_deserialize_Instruction() => AssertSpanToStreamDeserializeRoundTrip(
        CreateRandomInstructionEmulator(),
        static (emulator, destination) => emulator.Serialize(destination),
        static stream => Z80InstructionEmulator.Deserialize(stream),
        AssertEqual,
        Z80InstructionEmulator.SerializedSize);

    [Test]
    public void Serialize_span_throws_for_short_destination_Instruction() => Assert.Throws<ArgumentException>(
        () => CreateRandomInstructionEmulator().Serialize(new byte[Z80InstructionEmulator.SerializedSize - 1]));

    [Test]
    public void Restore_span_throws_for_short_source_Instruction() => Assert.Throws<ArgumentException>(
        () => CreateRandomInstructionEmulator().Restore(new byte[Z80InstructionEmulator.SerializedSize - 1]));

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
    public void Instruction_serialization_includes_pending_interrupt_step()
    {
        var original = new Z80InstructionEmulator();
        var originalHarness = CreateInstructionHarness(original);

        originalHarness.WriteByteToMemory(0x0000, 0x00);
        original.Registers.PC = 0x0000;
        original.Registers.SP = 0x9000;
        original.Interrupts.IFF1 = true;
        original.Interrupts.IFF2 = true;
        original.Interrupts.IM = 1;
        original.Interrupts.Interrupt = true;

        originalHarness.ExecuteInstruction();

        using var stream = new MemoryStream();
        original.Serialize(stream);
        stream.Position = 0;

        var copy = Z80InstructionEmulator.Deserialize(stream);
        var copyHarness = CreateInstructionHarness(copy);
        copyHarness.WriteByteToMemory(0x0000, 0x00);

        copyHarness.ExecuteInstruction();

        copy.Registers.PC.Should().Equal(0x0038);
        copy.Registers.SP.Should().Equal(0x8FFE);
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
        GetFieldValue<ushort>(actual, "currentStep").Should().Equal(GetFieldValue<ushort>(expected, "currentStep"));
        GetFieldValue<byte>(actual, "latch").Should().Equal(GetFieldValue<byte>(expected, "latch"));
    }

    private static void AssertEqual(Z80InstructionEmulator actual, Z80InstructionEmulator expected)
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
        GetFieldValue<byte>(actual, "latch").Should().Equal(GetFieldValue<byte>(expected, "latch"));
        GetFieldValue<ushort>(actual, "nextSequenceStep").Should().Equal(GetFieldValue<ushort>(expected, "nextSequenceStep"));
    }

    [Pure]
    private static Z80StepEmulator CreateRandomStepEmulator()
    {
        var emulator = new Z80StepEmulator
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

        SetFieldValue(emulator, "address", Rng.NextUShort());
        SetFieldValue(emulator, "currentStep", Rng.NextUShort());
        SetFieldValue(emulator, "latch", Rng.NextByte());

        return emulator;
    }

    [Pure]
    private static Z80InstructionEmulator CreateRandomInstructionEmulator()
    {
        var emulator = new Z80InstructionEmulator
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

        SetFieldValue(emulator, "address", Rng.NextUShort());
        SetFieldValue(emulator, "latch", Rng.NextByte());
        // Must be a valid step index or the no-step sentinel as Restore validates it.
        SetFieldValue(emulator, "nextSequenceStep", Rng.NextBool() ? ushort.MaxValue : Rng.NextByte(0, 8));

        return emulator;
    }

    [Pure]
    private static Z80InstructionEmulatorTestHarness CreateInstructionHarness(Z80InstructionEmulator emulator) =>
        (Z80InstructionEmulatorTestHarness)typeof(Z80InstructionEmulatorTestHarness)
            .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, [typeof(Z80InstructionEmulator)], null)!
            .Invoke([emulator]);

    private static void AssertStreamOnlyRestoreRoundTrip<T>(T original, Func<T> createRandom, Action<T, Stream> serialize, Action<T, Stream> restore, Action<T, T> assertEqual, int serializedSize)
    {
        using var stream = new MemoryStream();
        serialize(original, stream);
        stream.Length.Should().Equal(serializedSize);
        stream.Position = 0;

        var copy = createRandom();
        restore(copy, stream);
        assertEqual(copy, original);
    }

    private static void AssertStreamOnlyDeserializeRoundTrip<T>(T original, Action<T, Stream> serialize, Func<Stream, T> deserialize, Action<T, T> assertEqual, int serializedSize)
    {
        using var stream = new MemoryStream();
        serialize(original, stream);
        stream.Length.Should().Equal(serializedSize);
        stream.Position = 0;

        var copy = deserialize(stream);
        assertEqual(copy, original);
    }

    private static void AssertSpanOnlyRestoreRoundTrip<T>(T original, Func<T> createRandom, SpanSerializer<T> serialize, SpanRestorer<T> restore, Action<T, T> assertEqual, int serializedSize)
    {
        var buffer = new byte[serializedSize];
        serialize(original, buffer);

        var copy = createRandom();
        restore(copy, buffer);
        assertEqual(copy, original);
    }

    private static void AssertSpanOnlyDeserializeRoundTrip<T>(T original, SpanSerializer<T> serialize, SpanDeserializer<T> deserialize, Action<T, T> assertEqual, int serializedSize)
    {
        var buffer = new byte[serializedSize];
        serialize(original, buffer);

        var copy = deserialize(buffer);
        assertEqual(copy, original);
    }

    private static void AssertStreamToSpanRestoreRoundTrip<T>(T original, Func<T> createRandom, Action<T, Stream> serialize, SpanRestorer<T> restore, Action<T, T> assertEqual, int serializedSize)
    {
        using var stream = new MemoryStream();
        serialize(original, stream);
        stream.Length.Should().Equal(serializedSize);

        var copy = createRandom();
        restore(copy, stream.ToArray());
        assertEqual(copy, original);
    }

    private static void AssertStreamToSpanDeserializeRoundTrip<T>(T original, Action<T, Stream> serialize, SpanDeserializer<T> deserialize, Action<T, T> assertEqual, int serializedSize)
    {
        using var stream = new MemoryStream();
        serialize(original, stream);
        stream.Length.Should().Equal(serializedSize);

        var copy = deserialize(stream.ToArray());
        assertEqual(copy, original);
    }

    private static void AssertSpanToStreamRestoreRoundTrip<T>(T original, Func<T> createRandom, SpanSerializer<T> serialize, Action<T, Stream> restore, Action<T, T> assertEqual, int serializedSize)
    {
        var buffer = new byte[serializedSize];
        serialize(original, buffer);

        using var stream = new MemoryStream(buffer, writable: false);
        var copy = createRandom();
        restore(copy, stream);
        assertEqual(copy, original);
    }

    private static void AssertSpanToStreamDeserializeRoundTrip<T>(T original, SpanSerializer<T> serialize, Func<Stream, T> deserialize, Action<T, T> assertEqual, int serializedSize)
    {
        var buffer = new byte[serializedSize];
        serialize(original, buffer);

        using var stream = new MemoryStream(buffer, writable: false);
        var copy = deserialize(stream);
        assertEqual(copy, original);
    }

    [Pure]
    private static TValue GetFieldValue<TValue>(object instance, string fieldName) =>
        (TValue)(typeof(object) != instance.GetType() ? instance.GetType() : throw new InvalidOperationException())
            .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(instance)!;

    private static void SetFieldValue<TValue>(object instance, string fieldName, TValue value) =>
        (typeof(object) != instance.GetType() ? instance.GetType() : throw new InvalidOperationException())
            .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(instance, value);

    private delegate void SpanSerializer<in T>(T emulator, Span<byte> destination);

    private delegate T SpanDeserializer<out T>(ReadOnlySpan<byte> source);

    private delegate void SpanRestorer<in T>(T emulator, ReadOnlySpan<byte> source);

    private static Randomizer Rng => TestContext.CurrentContext.Random;
}