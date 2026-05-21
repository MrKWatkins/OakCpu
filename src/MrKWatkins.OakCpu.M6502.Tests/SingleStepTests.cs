using MrKWatkins.EmulatorTestSuites.M6502;
using MrKWatkins.EmulatorTestSuites.M6502.Instruction.SingleStep;
using MrKWatkins.OakCpu.M6502.Testing;

namespace MrKWatkins.OakCpu.M6502.Tests;

[Explicit]
[Parallelizable(ParallelScope.All)]
[TestFixture(typeof(M6502StepEmulatorTestHarness))]
[TestFixture(typeof(M6502InstructionEmulatorTestHarness))]
public sealed class SingleStepTests<THarness>
    where THarness : M6502TestHarness, new()
{
    [TestCaseSource(nameof(TestCases))]
    public void SingleStepTest(SingleStepTestCase testCase) => testCase.Execute<THarness>(TestContext.Progress);

    [Pure]
    public static IEnumerable<TestCaseData> TestCases() =>
        SingleStepTestSuite.Instance
            .GetTestCases()
            .Select(testCase => new TestCaseData(testCase).SetName(testCase.Name));
}