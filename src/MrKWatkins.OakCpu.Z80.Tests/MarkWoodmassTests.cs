using MrKWatkins.EmulatorTestSuites.Z80;
using MrKWatkins.EmulatorTestSuites.Z80.Program.MarkWoodmass;
using MrKWatkins.OakCpu.Z80.Testing;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Explicit]
[TestFixture(typeof(Z80StepEmulatorTestHarness))]
[TestFixture(typeof(Z80InstructionEmulatorTestHarness))]
public sealed class MarkWoodmassTests<THarness>
    where THarness : Z80TestHarness, new()
{
    [TestCaseSource(nameof(TestCases), [MarkWoodmassTestType.Flags])]
    public void Flags(MarkWoodmassTestCase testCase) => testCase.Execute<THarness>(TestContext.Progress);

    [TestCaseSource(nameof(TestCases), [MarkWoodmassTestType.Memptr])]
    public void Memptr(MarkWoodmassTestCase testCase) => testCase.Execute<THarness>(TestContext.Progress);

    [Pure]
    public static IEnumerable<TestCaseData> TestCases(MarkWoodmassTestType type) => MarkWoodmassTestSuite.Get(type).TestCases.ToTestCaseData();
}