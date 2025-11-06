using MrKWatkins.EmulatorTestSuites.Z80.Program.Timing;
using MrKWatkins.OakCpu.Z80.Testing;

namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class TimingTests
{
    [TestCaseSource(nameof(TestCases))]
    public void Timing(TimingTestCase testCase) => testCase.Execute<Z80EmulatorWithContentionTestHarness>(TestContext.Progress);

    [Pure]
    public static IEnumerable<TestCaseData> TestCases() => TimingTestSuite.Instance.TestCases.ToTestCaseData();
}