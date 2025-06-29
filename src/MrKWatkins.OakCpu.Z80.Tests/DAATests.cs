using MrKWatkins.OakCpu.Z80.TestSuites.Instruction.DAA;

namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class DAATests
{
    [TestCaseSource(nameof(TestCases))]
    public void DAA(DAATestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>();

    [Pure]
    public static IEnumerable<TestCaseData> TestCases() => DAATestSuite.Instance.GetTestCases().ToTestCaseData();
}