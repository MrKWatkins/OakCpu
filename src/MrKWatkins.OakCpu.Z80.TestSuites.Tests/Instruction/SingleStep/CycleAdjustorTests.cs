using MrKWatkins.OakCpu.Z80.TestSuites.Instruction;
using MrKWatkins.OakCpu.Z80.TestSuites.Instruction.SingleStep;

namespace MrKWatkins.OakCpu.Z80.TestSuites.Tests.Instruction.SingleStep;

public sealed class CycleAdjustorTests
{
    [TestCaseSource(nameof(AdjustToTestCases))]
    public void AdjustTo(IReadOnlyList<Cycle> cycles, MemoryCycleMethod method, IReadOnlyList<Cycle> expected)
    {
        var actual = CycleAdjustor.AdjustTo(method, cycles).ToList();
        actual.Should().BeEquivalentTo(expected, c => c.WithStrictOrdering());
    }

    [Pure]
    public static IEnumerable<TestCaseData> AdjustToTestCases()
    {
        var original34 = new[]
        {
            new Cycle(CycleType.None, 19604, null),
            new Cycle(CycleType.MemoryRead, 19604, null),
            new Cycle(CycleType.None, 37982, 52),
            new Cycle(CycleType.None, 37982, null),
            new Cycle(CycleType.None, 36112, null),
            new Cycle(CycleType.MemoryRead, 36112, null),
            new Cycle(CycleType.None, 36112, 94),
            new Cycle(CycleType.None, 36112, null),
            new Cycle(CycleType.None, 36112, null),
            new Cycle(CycleType.MemoryWrite, 36112, 95),
            new Cycle(CycleType.None, 36112, null),
        };

        yield return new TestCaseData(original34, MemoryCycleMethod.End, original34).SetArgDisplayNames($"0x34, {nameof(MemoryCycleMethod)}.{nameof(MemoryCycleMethod.End)}");

        var start34 = new[]
        {
            new Cycle(CycleType.MemoryRead, 19604, null),
            new Cycle(CycleType.None, 19604, 52),
            new Cycle(CycleType.None, 37982, null),
            new Cycle(CycleType.None, 37982, null),
            new Cycle(CycleType.MemoryRead, 36112, null),
            new Cycle(CycleType.None, 36112, 94),
            new Cycle(CycleType.None, 36112, null),
            new Cycle(CycleType.None, 36112, null),
            new Cycle(CycleType.MemoryWrite, 36112, 95),
            new Cycle(CycleType.None, 36112, null),
            new Cycle(CycleType.None, 36112, null),
        };

        yield return new TestCaseData(original34, MemoryCycleMethod.Start, start34).SetArgDisplayNames($"0x34, {nameof(MemoryCycleMethod)}.{nameof(MemoryCycleMethod.Start)}");

        var accurate34 = new[]
        {
            new Cycle(CycleType.MemoryRead, 19604, null),
            new Cycle(CycleType.MemoryRead, 19604, 52),
            new Cycle(CycleType.None, 37982, null),
            new Cycle(CycleType.None, 37982, null),
            new Cycle(CycleType.MemoryRead, 36112, null),
            new Cycle(CycleType.MemoryRead, 36112, 94),
            new Cycle(CycleType.None, 36112, null),
            new Cycle(CycleType.None, 36112, null),
            new Cycle(CycleType.MemoryWrite, 36112, 95),
            new Cycle(CycleType.MemoryWrite, 36112, 95),
            new Cycle(CycleType.None, 36112, null),
        };

        yield return new TestCaseData(original34, MemoryCycleMethod.Accurate, accurate34).SetArgDisplayNames($"0x34, {nameof(MemoryCycleMethod)}.{nameof(MemoryCycleMethod.Accurate)}");
    }
}