using MrKWatkins.Z80TestSuites.Fuse;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Explicit]
[Parallelizable(ParallelScope.All)]
public sealed class FuseTests
{
    [TestCaseSource(nameof(TestCases))]
    public void FuseTest(FuseTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>();

    [Pure]
    public static IEnumerable<FuseTestCase> TestCases() => FuseTestSuite.TestCases;
}