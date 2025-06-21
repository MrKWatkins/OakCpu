using MrKWatkins.OakCpu.Z80.TestSuites.Instruction;
using MrKWatkins.OakCpu.Z80.TestSuites.Instruction.Fuse;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Explicit]
[Parallelizable(ParallelScope.All)]
public sealed class FuseTests
{
    private static readonly IReadOnlyDictionary<string, Assertions> AssertionsToRunOverrides = new Dictionary<string, Assertions>
    {
        // Fuse skips the read of the offset when there is no jump. This means it is missing a MemoryRead event in the test. https://sourceforge.net/p/fuse-emulator/bugs/512/
        ["10"] = Assertions.AllExceptCycles,        // DJNZ
        ["20_2"] = Assertions.AllExceptCycles,      // JR NZ, d
        ["28_1"] = Assertions.AllExceptCycles,      // JR Z, d
        ["30_2"] = Assertions.AllExceptCycles,      // JR NC, d
        ["38_1"] = Assertions.AllExceptCycles,      // JR C, d

        // Fuse does not move onto the next instruction on HALT so PC will differ. However, PC should move. See https://github.com/redcode/Z80/wiki/Z80-Special-Reset#halt-and-the-special-reset.
        ["76"] = Assertions.All & ~Assertions.PC
    };

    [TestCaseSource(nameof(TestCases))]
    public void FuseTest(FuseTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>();

    [Pure]
    public static IEnumerable<TestCaseData> TestCases() => FuseTestSuite.Instance.GetTestCases(new InstructionTestSuiteOptions { AssertionsToRunOverrides = AssertionsToRunOverrides }).ToTestCaseData();
}