using MrKWatkins.EmulatorTestSuites.M6502.Instruction.SingleStep;
using MrKWatkins.OakCpu.M6502.Testing;

namespace MrKWatkins.OakCpu.M6502.Tests;

[Parallelizable(ParallelScope.All)]
[TestFixture]
public sealed class M6502InstructionSingleStepTests
{
    [TestCaseSource(typeof(SingleStepTestCases), nameof(SingleStepTestCases.Create))]
    public void SingleStepTest(SingleStepTestCase testCase) => testCase.Execute<M6502InstructionEmulatorTestHarness>(TestContext.Progress);
}

[Parallelizable(ParallelScope.All)]
[TestFixture]
public sealed class M6502StepSingleStepTests
{
    [TestCaseSource(typeof(SingleStepTestCases), nameof(SingleStepTestCases.Create))]
    public void SingleStepTest(SingleStepTestCase testCase) => testCase.Execute<M6502StepEmulatorTestHarness>(TestContext.Progress);
}

internal static class SingleStepTestCases
{
    private static readonly HashSet<string> SupportedOpcodes = ["a9", "ea"];

    [Pure]
    public static IEnumerable<TestCaseData> Create() =>
        SingleStepTestSuite.Instance
            .GetTestCases()
            .Where(testCase => SupportedOpcodes.Contains(testCase.Id))
            .Select(testCase => new TestCaseData(testCase).SetName(testCase.Name));
}