using MrKWatkins.EmulatorTestSuites.Z80.Instruction;
using MrKWatkins.EmulatorTestSuites.Z80.Instruction.Fuse;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Explicit]
[Parallelizable(ParallelScope.All)]
public sealed class FuseTests
{
    private const TestAssertions DefaultExceptCycles = FuseTestSuite.DefaultAssertions & ~TestAssertions.Cycles;
    private const TestAssertions DefaultExceptPC = FuseTestSuite.DefaultAssertions & ~TestAssertions.PC;

    private static readonly IReadOnlyDictionary<string, TestAssertions> TestAssertionsToRunOverrides = new Dictionary<string, TestAssertions>
    {
        // Fuse skips the read of the offset when there is no jump. This means it is missing a MemoryRead event in the test. https://sourceforge.net/p/fuse-emulator/bugs/512/
        ["10"] = DefaultExceptCycles,        // DJNZ
        ["20_2"] = DefaultExceptCycles,      // JR NZ, d
        ["28_1"] = DefaultExceptCycles,      // JR Z, d
        ["30_2"] = DefaultExceptCycles,      // JR NC, d
        ["38_1"] = DefaultExceptCycles,      // JR C, d

        // Fuse does not move onto the next instruction on HALT so PC will differ. However, PC should move. See https://github.com/redcode/Z80/wiki/Z80-Special-Reset#halt-and-the-special-reset.
        ["76"] = DefaultExceptPC,

        // The following tests disagree with the Single Step tests.
        // TODO: Use the netlist simulator to work out which one is actually correct.
        ["edb2_1"] = FuseTestSuite.DefaultAssertions & ~TestAssertions.PV & ~TestAssertions.X & ~TestAssertions.F & ~TestAssertions.WZ,
        ["edb3_1"] = FuseTestSuite.DefaultAssertions & ~TestAssertions.PV & ~TestAssertions.H & ~TestAssertions.F & ~TestAssertions.WZ,
        ["edb9_2"] = FuseTestSuite.DefaultAssertions & ~TestAssertions.X & ~TestAssertions.F,
        ["edba_1"] = FuseTestSuite.DefaultAssertions & ~TestAssertions.WZ,
        ["edbb_1"] = FuseTestSuite.DefaultAssertions & ~TestAssertions.PV & ~TestAssertions.H & ~TestAssertions.F & ~TestAssertions.WZ
    };

    [TestCaseSource(nameof(TestCases))]
    public void Fuse(FuseTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>();

    [Pure]
    public static IEnumerable<TestCaseData> TestCases() => FuseTestSuite.Instance.GetTestCases(TestAssertionsToRunOverrides).ToTestCaseData();
}