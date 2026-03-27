using MrKWatkins.EmulatorTestSuites.Z80;
using MrKWatkins.EmulatorTestSuites.Z80.Program.Raxoft;
using MrKWatkins.OakCpu.Z80.Testing;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Explicit]
[TestFixture(typeof(Z80StepEmulatorTestHarness))]
[TestFixture(typeof(Z80InstructionEmulatorTestHarness))]
public sealed class RaxoftTests<THarness>
    where THarness : Z80TestHarness, new()
{
    // Tests all flags and registers.
    [TestCaseSource(nameof(TestCases), [RaxoftTestType.Full, RaxoftTestVersion.V1_2A])]
    public void Full(RaxoftTestCase testCase) => testCase.Execute<THarness>(TestContext.Progress);

    // Tests all registers but only the officially documented flags.
    [TestCaseSource(nameof(TestCases), [RaxoftTestType.Doc, RaxoftTestVersion.V1_2A])]
    public void Doc(RaxoftTestCase testCase) => testCase.Execute<THarness>(TestContext.Progress);

    // Tests all flags, ignores registers.
    [TestCaseSource(nameof(TestCases), [RaxoftTestType.Flags, RaxoftTestVersion.V1_2A])]
    public void Flags(RaxoftTestCase testCase) => testCase.Execute<THarness>(TestContext.Progress);

    // Tests documented flags only, ignores registers.
    [TestCaseSource(nameof(TestCases), [RaxoftTestType.DocFlags, RaxoftTestVersion.V1_2A])]
    public void DocFlags(RaxoftTestCase testCase) => testCase.Execute<THarness>(TestContext.Progress);

    // Tests all flags after executing CCF after each instruction tested.
    [TestCaseSource(nameof(TestCases), [RaxoftTestType.Ccf, RaxoftTestVersion.V1_2A])]
    public void Ccf(RaxoftTestCase testCase) => testCase.Execute<THarness>(TestContext.Progress);

    // Tests all flags after executing BIT N,(HL) after each instruction tested.
    [TestCaseSource(nameof(TestCases), [RaxoftTestType.Memptr, RaxoftTestVersion.V1_2A])]
    public void Memptr(RaxoftTestCase testCase) => testCase.Execute<THarness>(TestContext.Progress);

    [Pure]
    public static IEnumerable<TestCaseData> TestCases(RaxoftTestType type, RaxoftTestVersion version) => RaxoftTestSuite.Get(type, version).TestCases.ToTestCaseData();
}