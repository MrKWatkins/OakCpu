using MrKWatkins.EmulatorTestSuites.Z80.Instruction;
using MrKWatkins.EmulatorTestSuites.Z80.Instruction.SingleStep;
using MrKWatkins.OakCpu.Z80.Testing;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Explicit]
[Parallelizable(ParallelScope.All)]
public sealed class SingleStepTests
{
    private const TestAssertions DefaultExceptPC = SingleStepTestSuite.DefaultAssertions & ~TestAssertions.PC;

    private static readonly IReadOnlyDictionary<string, TestAssertions> TestAssertionsToRunOverrides = new Dictionary<string, TestAssertions>
    {
        // Single step tests do not move onto the next instruction on HALT so PC will differ. However, PC should move. See https://github.com/redcode/Z80/wiki/Z80-Special-Reset#halt-and-the-special-reset.
        ["76"] = DefaultExceptPC,
        ["DD 76"] = DefaultExceptPC,
        ["FD 76"] = DefaultExceptPC
    };

    [TestCaseSource(nameof(TestCases))]
    public void SingleStepTest(SingleStepTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>(TestContext.Progress);

    [Pure]
    public static IEnumerable<TestCaseData> TestCases() => SingleStepTestSuite.Instance.GetTestCases(TestAssertionsToRunOverrides).ToTestCaseData();
}