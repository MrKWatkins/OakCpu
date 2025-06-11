using MrKWatkins.OakCpu.Z80.TestSuites.Fuse;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Explicit]
[Parallelizable(ParallelScope.All)]
public sealed class FuseTests
{
    private static readonly IReadOnlyDictionary<string, FuseAssertions> AssertionsOverrides = new Dictionary<string, FuseAssertions>
    {
        // Fuse skips the read of the offset when there is no jump. This means it is missing a MemoryRead event in the test. https://sourceforge.net/p/fuse-emulator/bugs/512/
        ["10"] = FuseAssertions.AllExceptEvents,        // DJNZ
        ["20_2"] = FuseAssertions.AllExceptEvents,      // JR NZ, d
        ["28_1"] = FuseAssertions.AllExceptEvents,      // JR Z, d
        ["30_2"] = FuseAssertions.AllExceptEvents,      // JR NC, d
        ["38_1"] = FuseAssertions.AllExceptEvents,      // JR C, d

        // Fuse does not move onto the next instruction on HALT so PC will differ. However, PC should move. See https://github.com/redcode/Z80/wiki/Z80-Special-Reset#halt-and-the-special-reset.
        ["76"] = FuseAssertions.AllExceptRegisters,
    };

    [TestCaseSource(nameof(TestCases))]
    public void FuseTest(FuseTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>();

    [Pure]
    public static IEnumerable<FuseTestCase> TestCases() => FuseTestSuite.TestCasesWithOverriddenAssertions(AssertionsOverrides);
}