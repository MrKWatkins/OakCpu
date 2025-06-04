using MrKWatkins.Z80TestSuites.Fuse;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Explicit]
[Parallelizable(ParallelScope.All)]
public sealed class FuseTests
{
    private static readonly IReadOnlyDictionary<string, FuseAssertions> AssertionsOverrides = new Dictionary<string, FuseAssertions>
    {
        // Fuse skips the read of the offset when there is no jump. This means it is missing a MemoryRead event in the test. https://sourceforge.net/p/fuse-emulator/bugs/512/
        ["10"] = FuseAssertions.AllExceptEvents
    };

    [TestCaseSource(nameof(TestCases))]
    public void FuseTest(FuseTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>();

    [Pure]
    public static IEnumerable<FuseTestCase> TestCases() => FuseTestSuite.TestCasesWithOverriddenAssertions(AssertionsOverrides);
}