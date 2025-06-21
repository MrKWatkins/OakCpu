using MrKWatkins.OakCpu.Z80.TestSuites.Program.Raxoft;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Explicit]
public sealed class RaxoftTests
{
    // Tests all flags and registers.
    [TestCaseSource(nameof(TestCases), [RaxoftTestType.Full, RaxoftTestVersion.V1_2A])]
    public void Full(RaxoftTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>(TestContext.Progress);

    // Tests all registers but only the officially documented flags.
    [TestCaseSource(nameof(TestCases), [RaxoftTestType.Doc, RaxoftTestVersion.V1_2A])]
    public void Doc(RaxoftTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>(TestContext.Progress);

    // Tests all flags, ignores registers.
    [TestCaseSource(nameof(TestCases), [RaxoftTestType.Flags, RaxoftTestVersion.V1_2A])]
    public void Flags(RaxoftTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>(TestContext.Progress);

    // Tests documented flags only, ignores registers.
    [TestCaseSource(nameof(TestCases), [RaxoftTestType.DocFlags, RaxoftTestVersion.V1_2A])]
    public void DocFlags(RaxoftTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>(TestContext.Progress);

    // Tests all flags after executing CCF after each instruction tested.
    [TestCaseSource(nameof(TestCases), [RaxoftTestType.Ccf, RaxoftTestVersion.V1_2A])]
    public void Ccf(RaxoftTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>(TestContext.Progress);

    // Tests all flags after executing BIT N,(HL) after each instruction tested.
    [TestCaseSource(nameof(TestCases), [RaxoftTestType.Memptr, RaxoftTestVersion.V1_2A])]
    public void Memptr(RaxoftTestCase testCase) => testCase.Execute<Z80EmulatorTestHarness>(TestContext.Progress);

    [Pure]
    public static IEnumerable<RaxoftTestCase> TestCases(RaxoftTestType type, RaxoftTestVersion version) => RaxoftTestSuite.Get(type, version).TestCases;
}