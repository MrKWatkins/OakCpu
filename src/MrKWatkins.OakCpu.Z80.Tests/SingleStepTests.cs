using MrKWatkins.EmulatorTestSuites.Z80;
using MrKWatkins.EmulatorTestSuites.Z80.Instruction;
using MrKWatkins.EmulatorTestSuites.Z80.Instruction.SingleStep;
using MrKWatkins.OakCpu.Z80.Testing;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Explicit]
[Parallelizable(ParallelScope.All)]
[TestFixture(typeof(Z80StepEmulatorTestHarness))]
[TestFixture(typeof(Z80InstructionEmulatorTestHarness))]
public sealed class SingleStepTests<THarness>
    where THarness : Z80TestHarness, new()
{
    private const TestAssertions DefaultExceptPC = SingleStepTestSuite.DefaultAssertions & ~TestAssertions.PC;
    private const TestAssertions DefaultExceptCycles = SingleStepTestSuite.DefaultAssertions & ~TestAssertions.Cycles;
    private const TestAssertions DefaultExceptCyclesAndPC = SingleStepTestSuite.DefaultAssertions & ~(TestAssertions.Cycles | TestAssertions.PC);

    // ReSharper disable once StaticMemberInGenericType
    private static readonly IReadOnlyDictionary<string, TestAssertions> StepTestAssertionsToRunOverrides = new Dictionary<string, TestAssertions>
    {
        // Single step tests do not move onto the next instruction on HALT so PC will differ. However, PC should move. See https://github.com/redcode/Z80/wiki/Z80-Special-Reset#halt-and-the-special-reset.
        ["76"] = DefaultExceptPC,
        ["DD 76"] = DefaultExceptPC,
        ["FD 76"] = DefaultExceptPC
    };

    // ReSharper disable once StaticMemberInGenericType
    private static readonly IReadOnlyDictionary<string, TestAssertions> InstructionTestAssertionsToRunOverrides = new Dictionary<string, TestAssertions>
    {
        ["76"] = DefaultExceptCyclesAndPC,
        ["DD 76"] = DefaultExceptCyclesAndPC,
        ["FD 76"] = DefaultExceptCyclesAndPC
    };

    [TestCaseSource(nameof(TestCases))]
    public void SingleStepTest(SingleStepTestCase testCase) => testCase.Execute<THarness>(TestContext.Progress);

    [Pure]
    public static IEnumerable<TestCaseData> TestCases() =>
        IsInstructionHarness
            ? SingleStepTestSuite.Instance.GetTestCases(
                SingleStepTestSuite.Instance.DefaultOptions with
                {
                    AssertionsToRun = DefaultExceptCycles,
                    AssertionsToRunOverrides = InstructionTestAssertionsToRunOverrides
                }).ToTestCaseData()
            : SingleStepTestSuite.Instance.GetTestCases(StepTestAssertionsToRunOverrides).ToTestCaseData();

    [Pure]
    private static bool IsInstructionHarness => typeof(THarness) == typeof(Z80InstructionEmulatorTestHarness);
}