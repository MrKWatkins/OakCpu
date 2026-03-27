using MrKWatkins.EmulatorTestSuites.Z80;
using MrKWatkins.EmulatorTestSuites.Z80.Instruction;
using MrKWatkins.EmulatorTestSuites.Z80.Instruction.DAA;
using MrKWatkins.OakCpu.Z80.Testing;

namespace MrKWatkins.OakCpu.Z80.Tests;

[Parallelizable(ParallelScope.All)]
[TestFixture(typeof(Z80StepEmulatorTestHarness))]
[TestFixture(typeof(Z80InstructionEmulatorTestHarness))]
public sealed class DAATests<THarness>
    where THarness : Z80TestHarness, new()
{
    [TestCaseSource(nameof(TestCases))]
    public void DAA(DAATestCase testCase) => testCase.Execute<THarness>();

    [Pure]
    public static IEnumerable<TestCaseData> TestCases() =>
        DAATestSuite.Instance.GetTestCases(IsInstructionHarness ? TestAssertions.AllExceptCycles : TestAssertions.All).ToTestCaseData();

    [Pure]
    private static bool IsInstructionHarness => typeof(THarness) == typeof(Z80InstructionEmulatorTestHarness);
}