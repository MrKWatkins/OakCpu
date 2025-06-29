using MrKWatkins.OakCpu.Z80.TestSuites.Program.MarkWoodmass;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Explicit]
public sealed class MarkWoodmassTests
{
    [TestCaseSource(nameof(TestCases), [MarkWoodmassTestType.Flags])]
    public void Flags(MarkWoodmassTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>(TestContext.Progress);

    [TestCaseSource(nameof(TestCases), [MarkWoodmassTestType.Memptr])]
    public void Memptr(MarkWoodmassTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>(TestContext.Progress);

    [Pure]
    public static IEnumerable<TestCaseData> TestCases(MarkWoodmassTestType type) => MarkWoodmassTestSuite.Get(type).TestCases.ToTestCaseData();
}