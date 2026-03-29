using MrKWatkins.EmulatorTestSuites.Z80;
using MrKWatkins.EmulatorTestSuites.Z80.Program.ZEXALL;
using MrKWatkins.OakCpu.Z80.Testing;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Explicit]
[Parallelizable(ParallelScope.All)]
[TestFixture(typeof(Z80StepEmulatorTestHarness))]
[TestFixture(typeof(Z80InstructionEmulatorTestHarness))]
[TestFixture(typeof(ContendedZ80StepEmulatorTestHarness))]
public sealed class ZEXALLTests<THarness>
    where THarness : Z80TestHarness, new()
{
    [TestCaseSource(nameof(TestCases), [ZEXALLTestType.ZEXALL])]
    public void ZEXALL(ZEXALLTestCase testCase) => testCase.Execute<THarness>(TestContext.Progress);

    [TestCaseSource(nameof(TestCases), [ZEXALLTestType.ZEXDOC])]
    public void ZEXDOC(ZEXALLTestCase testCase) => testCase.Execute<THarness>(TestContext.Progress);

    [Pure]
    public static IEnumerable<ZEXALLTestCase> TestCases(ZEXALLTestType type) => ZEXALLTestSuite.Get(type).TestCases;
}