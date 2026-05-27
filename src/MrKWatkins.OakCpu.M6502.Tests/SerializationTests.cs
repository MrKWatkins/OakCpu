using System.Reflection;
using NUnit.Framework.Internal;

namespace MrKWatkins.OakCpu.M6502.Tests;

public sealed class SerializationTests
{
    [Test]
    public void Stream_only_restore_Step() => AssertStreamOnlyRestoreRoundTrip(
        CreateRandomStepEmulator(),
        CreateRandomStepEmulator,
        static (emulator, stream) => emulator.Serialize(stream),
        static (emulator, stream) => emulator.Restore(stream),
        AssertEqual,
        M6502StepEmulator.SerializedSize);

    [Test]
    public void Stream_only_deserialize_Step() => AssertStreamOnlyDeserializeRoundTrip(
        CreateRandomStepEmulator(),
        static (emulator, stream) => emulator.Serialize(stream),
        static stream => M6502StepEmulator.Deserialize(stream),
        AssertEqual,
        M6502StepEmulator.SerializedSize);

    [Test]
    public void Span_only_restore_Step() => AssertSpanOnlyRestoreRoundTrip(
        CreateRandomStepEmulator(),
        CreateRandomStepEmulator,
        static (emulator, destination) => emulator.Serialize(destination),
        static (emulator, source) => emulator.Restore(source),
        AssertEqual,
        M6502StepEmulator.SerializedSize);

    [Test]
    public void Span_only_deserialize_Step() => AssertSpanOnlyDeserializeRoundTrip(
        CreateRandomStepEmulator(),
        static (emulator, destination) => emulator.Serialize(destination),
        static source => M6502StepEmulator.Deserialize(source),
        AssertEqual,
        M6502StepEmulator.SerializedSize);

    [Test]
    public void Stream_to_span_restore_Step() => AssertStreamToSpanRestoreRoundTrip(
        CreateRandomStepEmulator(),
        CreateRandomStepEmulator,
        static (emulator, stream) => emulator.Serialize(stream),
        static (emulator, source) => emulator.Restore(source),
        AssertEqual,
        M6502StepEmulator.SerializedSize);

    [Test]
    public void Stream_to_span_deserialize_Step() => AssertStreamToSpanDeserializeRoundTrip(
        CreateRandomStepEmulator(),
        static (emulator, stream) => emulator.Serialize(stream),
        static source => M6502StepEmulator.Deserialize(source),
        AssertEqual,
        M6502StepEmulator.SerializedSize);

    [Test]
    public void Span_to_stream_restore_Step() => AssertSpanToStreamRestoreRoundTrip(
        CreateRandomStepEmulator(),
        CreateRandomStepEmulator,
        static (emulator, destination) => emulator.Serialize(destination),
        static (emulator, stream) => emulator.Restore(stream),
        AssertEqual,
        M6502StepEmulator.SerializedSize);

    [Test]
    public void Span_to_stream_deserialize_Step() => AssertSpanToStreamDeserializeRoundTrip(
        CreateRandomStepEmulator(),
        static (emulator, destination) => emulator.Serialize(destination),
        static stream => M6502StepEmulator.Deserialize(stream),
        AssertEqual,
        M6502StepEmulator.SerializedSize);

    [Test]
    public void Serialize_span_throws_for_short_destination_Step() => Assert.Throws<ArgumentException>(
        () => CreateRandomStepEmulator().Serialize(new byte[M6502StepEmulator.SerializedSize - 1]));

    [Test]
    public void Restore_span_throws_for_short_source_Step() => Assert.Throws<ArgumentException>(
        () => CreateRandomStepEmulator().Restore(new byte[M6502StepEmulator.SerializedSize - 1]));

    [Test]
    public void Stream_only_restore_Instruction() => AssertStreamOnlyRestoreRoundTrip(
        CreateRandomInstructionEmulator(),
        CreateRandomInstructionEmulator,
        static (emulator, stream) => emulator.Serialize(stream),
        static (emulator, stream) => emulator.Restore(stream),
        AssertEqual,
        M6502InstructionEmulator.SerializedSize);

    [Test]
    public void Stream_only_deserialize_Instruction() => AssertStreamOnlyDeserializeRoundTrip(
        CreateRandomInstructionEmulator(),
        static (emulator, stream) => emulator.Serialize(stream),
        static stream => M6502InstructionEmulator.Deserialize(stream),
        AssertEqual,
        M6502InstructionEmulator.SerializedSize);

    [Test]
    public void Span_only_restore_Instruction() => AssertSpanOnlyRestoreRoundTrip(
        CreateRandomInstructionEmulator(),
        CreateRandomInstructionEmulator,
        static (emulator, destination) => emulator.Serialize(destination),
        static (emulator, source) => emulator.Restore(source),
        AssertEqual,
        M6502InstructionEmulator.SerializedSize);

    [Test]
    public void Span_only_deserialize_Instruction() => AssertSpanOnlyDeserializeRoundTrip(
        CreateRandomInstructionEmulator(),
        static (emulator, destination) => emulator.Serialize(destination),
        static source => M6502InstructionEmulator.Deserialize(source),
        AssertEqual,
        M6502InstructionEmulator.SerializedSize);

    [Test]
    public void Stream_to_span_restore_Instruction() => AssertStreamToSpanRestoreRoundTrip(
        CreateRandomInstructionEmulator(),
        CreateRandomInstructionEmulator,
        static (emulator, stream) => emulator.Serialize(stream),
        static (emulator, source) => emulator.Restore(source),
        AssertEqual,
        M6502InstructionEmulator.SerializedSize);

    [Test]
    public void Stream_to_span_deserialize_Instruction() => AssertStreamToSpanDeserializeRoundTrip(
        CreateRandomInstructionEmulator(),
        static (emulator, stream) => emulator.Serialize(stream),
        static source => M6502InstructionEmulator.Deserialize(source),
        AssertEqual,
        M6502InstructionEmulator.SerializedSize);

    [Test]
    public void Span_to_stream_restore_Instruction() => AssertSpanToStreamRestoreRoundTrip(
        CreateRandomInstructionEmulator(),
        CreateRandomInstructionEmulator,
        static (emulator, destination) => emulator.Serialize(destination),
        static (emulator, stream) => emulator.Restore(stream),
        AssertEqual,
        M6502InstructionEmulator.SerializedSize);

    [Test]
    public void Span_to_stream_deserialize_Instruction() => AssertSpanToStreamDeserializeRoundTrip(
        CreateRandomInstructionEmulator(),
        static (emulator, destination) => emulator.Serialize(destination),
        static stream => M6502InstructionEmulator.Deserialize(stream),
        AssertEqual,
        M6502InstructionEmulator.SerializedSize);

    [Test]
    public void Serialize_span_throws_for_short_destination_Instruction() => Assert.Throws<ArgumentException>(
        () => CreateRandomInstructionEmulator().Serialize(new byte[M6502InstructionEmulator.SerializedSize - 1]));

    [Test]
    public void Restore_span_throws_for_short_source_Instruction() => Assert.Throws<ArgumentException>(
        () => CreateRandomInstructionEmulator().Restore(new byte[M6502InstructionEmulator.SerializedSize - 1]));

    private static void AssertEqual(M6502StepEmulator actual, M6502StepEmulator expected)
    {
        actual.Address.Should().Equal(expected.Address);
        actual.Data.Should().Equal(expected.Data);
        actual.Registers.A.Should().Equal(expected.Registers.A);
        actual.Registers.P.Should().Equal(expected.Registers.P);
        actual.Registers.PC.Should().Equal(expected.Registers.PC);
        actual.Registers.S.Should().Equal(expected.Registers.S);
        actual.Registers.X.Should().Equal(expected.Registers.X);
        actual.Registers.Y.Should().Equal(expected.Registers.Y);
        actual.Interrupts.IRQ.Should().Equal(expected.Interrupts.IRQ);
        actual.Interrupts.NMI.Should().Equal(expected.Interrupts.NMI);
        GetFieldValue<ushort>(actual, "ad").Should().Equal(GetFieldValue<ushort>(expected, "ad"));
        GetFieldValue<ushort>(actual, "currentStep").Should().Equal(GetFieldValue<ushort>(expected, "currentStep"));
        GetFieldValue<ushort>(actual, "interruptvector").Should().Equal(GetFieldValue<ushort>(expected, "interruptvector"));
        GetFieldValue<bool>(actual, "pendingnmi").Should().Equal(GetFieldValue<bool>(expected, "pendingnmi"));
        GetFieldValue<bool>(actual, "previousnmi").Should().Equal(GetFieldValue<bool>(expected, "previousnmi"));
        GetFieldValue<bool>(actual, "sampledirq").Should().Equal(GetFieldValue<bool>(expected, "sampledirq"));
    }

    private static void AssertEqual(M6502InstructionEmulator actual, M6502InstructionEmulator expected)
    {
        actual.Address.Should().Equal(expected.Address);
        actual.Data.Should().Equal(expected.Data);
        actual.Registers.A.Should().Equal(expected.Registers.A);
        actual.Registers.P.Should().Equal(expected.Registers.P);
        actual.Registers.PC.Should().Equal(expected.Registers.PC);
        actual.Registers.S.Should().Equal(expected.Registers.S);
        actual.Registers.X.Should().Equal(expected.Registers.X);
        actual.Registers.Y.Should().Equal(expected.Registers.Y);
        actual.Interrupts.IRQ.Should().Equal(expected.Interrupts.IRQ);
        actual.Interrupts.NMI.Should().Equal(expected.Interrupts.NMI);
        GetFieldValue<ushort>(actual, "ad").Should().Equal(GetFieldValue<ushort>(expected, "ad"));
        GetFieldValue<ushort>(actual, "interruptvector").Should().Equal(GetFieldValue<ushort>(expected, "interruptvector"));
        GetFieldValue<bool>(actual, "pendingnmi").Should().Equal(GetFieldValue<bool>(expected, "pendingnmi"));
        GetFieldValue<bool>(actual, "previousnmi").Should().Equal(GetFieldValue<bool>(expected, "previousnmi"));
        GetFieldValue<bool>(actual, "sampledirq").Should().Equal(GetFieldValue<bool>(expected, "sampledirq"));
        GetFieldValue<ushort>(actual, "nextSequenceStep").Should().Equal(GetFieldValue<ushort>(expected, "nextSequenceStep"));
    }

    [Pure]
    private static M6502StepEmulator CreateRandomStepEmulator()
    {
        var emulator = new M6502StepEmulator
        {
            Data = Rng.NextByte(),
            Registers =
            {
                A = Rng.NextByte(),
                P = Rng.NextByte(),
                PC = Rng.NextUShort(),
                S = Rng.NextByte(),
                X = Rng.NextByte(),
                Y = Rng.NextByte()
            },
            Interrupts =
            {
                IRQ = Rng.NextBool(),
                NMI = Rng.NextBool()
            }
        };

        SetFieldValue(emulator, "address", Rng.NextUShort());
        SetFieldValue(emulator, "ad", Rng.NextUShort());
        SetFieldValue(emulator, "currentStep", Rng.NextUShort());
        SetFieldValue(emulator, "interruptvector", Rng.NextUShort());
        SetFieldValue(emulator, "pendingnmi", Rng.NextBool());
        SetFieldValue(emulator, "previousnmi", Rng.NextBool());
        SetFieldValue(emulator, "sampledirq", Rng.NextBool());

        return emulator;
    }

    [Pure]
    private static M6502InstructionEmulator CreateRandomInstructionEmulator()
    {
        var emulator = new M6502InstructionEmulator
        {
            Data = Rng.NextByte(),
            Registers =
            {
                A = Rng.NextByte(),
                P = Rng.NextByte(),
                PC = Rng.NextUShort(),
                S = Rng.NextByte(),
                X = Rng.NextByte(),
                Y = Rng.NextByte()
            },
            Interrupts =
            {
                IRQ = Rng.NextBool(),
                NMI = Rng.NextBool()
            }
        };

        SetFieldValue(emulator, "address", Rng.NextUShort());
        SetFieldValue(emulator, "ad", Rng.NextUShort());
        SetFieldValue(emulator, "interruptvector", Rng.NextUShort());
        SetFieldValue(emulator, "pendingnmi", Rng.NextBool());
        SetFieldValue(emulator, "previousnmi", Rng.NextBool());
        SetFieldValue(emulator, "sampledirq", Rng.NextBool());
        SetFieldValue(emulator, "nextSequenceStep", Rng.NextUShort());

        return emulator;
    }

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
        (TValue)instance.GetType()
            .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(instance)!;

    private static void SetFieldValue<TValue>(object instance, string fieldName, TValue value) =>
        instance.GetType()
            .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(instance, value);

    private delegate void SpanSerializer<in T>(T emulator, Span<byte> destination);

    private delegate T SpanDeserializer<out T>(ReadOnlySpan<byte> source);

    private delegate void SpanRestorer<in T>(T emulator, ReadOnlySpan<byte> source);

    private static Randomizer Rng => TestContext.CurrentContext.Random;
}