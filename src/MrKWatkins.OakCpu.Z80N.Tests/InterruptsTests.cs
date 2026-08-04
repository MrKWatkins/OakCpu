using MrKWatkins.EmulatorTestSuites.Z80.Interrupt;
using MrKWatkins.OakCpu.Z80N.Testing;

namespace MrKWatkins.OakCpu.Z80N.Tests;

public sealed class InterruptsTests
{
    [TestCaseSource(nameof(TestCases))]
    public void Interrupts(InterruptTestCase testCase) => testCase.Execute<Z80NStepEmulatorTestHarness>(TestContext.Progress);

    [Pure]
    public static IEnumerable<TestCaseData> TestCases() => InterruptTestSuite.Instance.TestCases.ToTestCaseData();
}