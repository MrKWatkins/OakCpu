using MrKWatkins.OakCpu.Z80.TestSuites;

namespace MrKWatkins.OakCpu.Z80.Tests;

public static class Extensions
{
    [Pure]
    public static IEnumerable<TestCaseData> ToTestCaseData<TTestCase>(this IEnumerable<TTestCase> testCases)
        where TTestCase : TestCase
        => testCases.Select(x => new TestCaseData(x).SetName(x.Name));
}