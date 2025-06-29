using MrKWatkins.OakCpu.Z80.TestSuites.Instruction;
using MrKWatkins.OakCpu.Z80.TestSuites.Instruction.Fuse;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Explicit]
[Parallelizable(ParallelScope.All)]
public sealed class FuseTests
{
    private const Assertions DefaultExceptCycles = FuseTestSuite.DefaultAssertions & ~Assertions.Cycles;
    private const Assertions DefaultExceptPC = FuseTestSuite.DefaultAssertions & ~Assertions.PC;

    private static readonly IReadOnlyDictionary<string, Assertions> AssertionsToRunOverrides = new Dictionary<string, Assertions>
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
        ["edb9_2"] = FuseTestSuite.DefaultAssertions & ~Assertions.X & ~Assertions.F,
        ["edb2_1"] = FuseTestSuite.DefaultAssertions & ~Assertions.PV & ~Assertions.X & ~Assertions.F & ~Assertions.WZ,
        ["edb3_1"] = FuseTestSuite.DefaultAssertions & ~Assertions.PV & ~Assertions.H & ~Assertions.F & ~Assertions.WZ,
        ["edba_1"] = FuseTestSuite.DefaultAssertions & ~Assertions.WZ,
        ["edbb_1"] = FuseTestSuite.DefaultAssertions & ~Assertions.PV & ~Assertions.H & ~Assertions.F & ~Assertions.WZ
    };

    [TestCaseSource(nameof(TestCases))]
    public void Fuse(FuseTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>();

    [Pure]
    public static IEnumerable<TestCaseData> TestCases() => FuseTestSuite.Instance.GetTestCases(AssertionsToRunOverrides).ToTestCaseData();
}