using System.IO.Compression;

namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction.SingleStep;

public sealed class Step
{
    private Step(int index, SingleStepZ80State input, SingleStepZ80State expected, IReadOnlyList<Cycle> cycles)
    {
        Index = index;
        Input = input;
        Expected = expected;
        Cycles = cycles;
    }

    public int Index { get; }

    public SingleStepZ80State Input { get; }

    public SingleStepZ80State Expected { get; }

    public IReadOnlyList<Cycle> Cycles { get; }

    // ReSharper disable once InconsistentNaming
    public int TStates => Cycles.Count;

    [Pure]
    internal static IEnumerable<Step> Load(SingleStepTestCase testCase)
    {
        using var stream = typeof(Step).Assembly.GetManifestResourceStream(SingleStepTestSuite.ResourcePrefix + testCase.Name) ?? throw new InvalidOperationException($"Resource {testCase.Name} not found.");
        using var brotli = new BrotliStream(stream, CompressionMode.Decompress);
        using var reader = new BinaryReader(brotli);

        var count = reader.Read7BitEncodedInt();

        for (var f = 0; f < count; f++)
        {
            yield return LoadStep(testCase, f, reader);
        }
    }

    [MustUseReturnValue]
    private static Step LoadStep(SingleStepTestCase testCase, int index, BinaryReader reader) => new(
        index,
        SingleStepZ80State.Load(reader),
        SingleStepZ80State.Load(reader),
        CycleAdjustor.AdjustTo(testCase.MemoryCycleMethod, LoadCycles(reader)).ToArray());

    [MustUseReturnValue]
    private static IEnumerable<Cycle> LoadCycles(BinaryReader reader)
    {
        var cyclesSize = reader.Read7BitEncodedInt();
        for (var f = 0; f < cyclesSize; f++)
        {
            var address = reader.ReadUInt16();
            var data = reader.ReadByte();
            var hasDataAndPins = reader.ReadByte();

            var hasData = (hasDataAndPins & 0b10000000) != 0;
            var pins = (Pins)(hasDataAndPins & 0b01111111);

            yield return new Cycle(ToCycleType(pins), address, hasData ? data : null);
        }
    }

    [Pure]
    private static CycleType ToCycleType(Pins pins) => pins switch
    {
        Pins.None => CycleType.None,
        Pins.MemoryRead => CycleType.MemoryRead,
        Pins.MemoryWrite => CycleType.MemoryWrite,
        Pins.IORead => CycleType.IORead,
        Pins.IOWrite => CycleType.IOWrite,
        _ => throw new NotSupportedException($"The {nameof(Pins)} combination {pins} is not supported.")
    };
}