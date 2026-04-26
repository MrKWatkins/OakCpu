using MrKWatkins.EmulatorTestSuites.Z80;
using MrKWatkins.EmulatorTestSuites.ZXSpectrum;

namespace MrKWatkins.OakCpu.Z80.Tests;

public static class Extensions
{
    [Pure]
    public static IEnumerable<TestCaseData> ToTestCaseData<TTestCase>(this IEnumerable<TTestCase> testCases)
        where TTestCase : TestCase
        => testCases.Select(x => new TestCaseData(x).SetName(x.Name));

    [Pure]
    public static IEnumerable<TestCaseData> ToTestCaseData(this IEnumerable<ZXSpectrumTestCase> testCases)
        => testCases.Select(x => new TestCaseData(x).SetName(x.Name));
}
