using MrKWatkins.EmulatorTestSuites.Z80.Program.ZEXALL;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Explicit]
[Parallelizable(ParallelScope.All)]
public sealed class ZEXALLTests
{
    [TestCaseSource(nameof(TestCases), [ZEXALLTestType.ZEXALL])]
    public void ZEXALL(ZEXALLTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>(TestContext.Progress);

    [TestCaseSource(nameof(TestCases), [ZEXALLTestType.ZEXDOC])]
    public void ZEXDOC(ZEXALLTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>(TestContext.Progress);

    [Pure]
    public static IEnumerable<ZEXALLTestCase> TestCases(ZEXALLTestType type) => ZEXALLTestSuite.Get(type).TestCases;
}