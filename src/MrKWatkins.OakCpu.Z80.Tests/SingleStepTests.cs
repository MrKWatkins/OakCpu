using MrKWatkins.OakCpu.Z80.TestSuites.Instruction;
using MrKWatkins.OakCpu.Z80.TestSuites.Instruction.SingleStep;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Explicit]
[Parallelizable(ParallelScope.All)]
public sealed class SingleStepTests
{
    private const Assertions DefaultExceptCycles = Assertions.All & ~Assertions.Halted & ~Assertions.Cycles;

    private static readonly IReadOnlyDictionary<string, Assertions> AssertionsToRunOverrides = new Dictionary<string, Assertions>
    {
        // SingleStep disagrees with Fuse and the netlist simulator for the following instructions: (https://github.com/SingleStepTests/z80/issues/3)
        ["DD 36"] = DefaultExceptCycles,        // LD (IY + d), n
        ["FD 36"] = DefaultExceptCycles,        // LD (IY + d), n,
        ["E3"] = DefaultExceptCycles,           // EX (SP), HL
        ["DD E3"] = DefaultExceptCycles,        // EX (SP), IX
        ["FD E3"] = DefaultExceptCycles,        // EX (SP), IY
        ["ED 67"] = DefaultExceptCycles,        // RRD
        ["ED 6F"] = DefaultExceptCycles,        // RLD
    };

    [TestCaseSource(nameof(TestCases))]
    public void SingleStepTest(SingleStepTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>(TestContext.Progress);

    [Pure]
    public static IEnumerable<TestCaseData> TestCases() => SingleStepTestSuite.Instance.GetTestCases(AssertionsToRunOverrides).ToTestCaseData();
}