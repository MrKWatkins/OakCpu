using MrKWatkins.EmulatorTestSuites;
using MrKWatkins.EmulatorTestSuites.Z80;

namespace MrKWatkins.OakCpu.Z80.Tests;

public static class Extensions
{
    [Pure]
    public static IEnumerable<TestCaseData> ToTestCaseData<TTestCase>(this IEnumerable<TTestCase> testCases)
        where TTestCase : TestCase<Z80TestHarness>
        => testCases.Select(x => new TestCaseData(x).SetName(x.Name));
}