using MrKWatkins.EmulatorTestSuites.Z80.Interrupt;
using MrKWatkins.OakCpu.Z80.Testing;

namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class InterruptsTests
{
    [TestCaseSource(nameof(TestCases))]
    public void Interrupts(InterruptTestCase testCase) => testCase.Execute<Z80StepEmulatorTestHarness>(TestContext.Progress);

    [Pure]
    public static IEnumerable<TestCaseData> TestCases() => InterruptTestSuite.Instance.TestCases.ToTestCaseData();
}