using MrKWatkins.EmulatorTestSuites.Z80.Instruction;
using MrKWatkins.EmulatorTestSuites.Z80.Instruction.SingleStep;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Explicit]
[Parallelizable(ParallelScope.All)]
public sealed class SingleStepTests
{
    private const TestAssertions DefaultExceptCycles = SingleStepTestSuite.DefaultAssertions & ~TestAssertions.Cycles;
    private const TestAssertions DefaultExceptPC = SingleStepTestSuite.DefaultAssertions & ~TestAssertions.PC;

    private static readonly IReadOnlyDictionary<string, TestAssertions> TestAssertionsToRunOverrides = new Dictionary<string, TestAssertions>
    {
        // SingleStep disagrees with Fuse and the netlist simulator for the following instructions: (https://github.com/SingleStepTests/z80/issues/3)
        ["DD 36"] = DefaultExceptCycles,        // LD (IY + d), n
        ["FD 36"] = DefaultExceptCycles,        // LD (IY + d), n,
        ["E3"] = DefaultExceptCycles,           // EX (SP), HL
        ["DD E3"] = DefaultExceptCycles,        // EX (SP), IX
        ["FD E3"] = DefaultExceptCycles,        // EX (SP), IY
        ["ED 67"] = DefaultExceptCycles,        // RRD
        ["ED 6F"] = DefaultExceptCycles,        // RLD

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