using MrKWatkins.EmulatorTestSuites.Z80;
using MrKWatkins.EmulatorTestSuites.Z80.Instruction;
using MrKWatkins.EmulatorTestSuites.Z80N.Instruction.Extended;
using MrKWatkins.OakCpu.Z80N.Testing;

namespace MrKWatkins.OakCpu.Z80N.Tests;

[Parallelizable(ParallelScope.All)]
[TestFixture(typeof(Z80NStepEmulatorTestHarness))]
[TestFixture(typeof(Z80NInstructionEmulatorTestHarness))]
public sealed class ExtendedInstructionsTests<THarness>
    where THarness : Z80TestHarness, new()
{
    private const TestAssertions DefaultExceptCycles = ExtendedInstructionsTestSuite.DefaultAssertions & ~TestAssertions.Cycles;

    [TestCaseSource(nameof(TestCases))]
    public void ExtendedInstructions(ExtendedInstructionsTestCase testCase) => testCase.Execute<THarness>();

    [Pure]
    public static IEnumerable<TestCaseData> TestCases() =>
        ExtendedInstructionsTestSuite.Instance
            .GetTestCases(IsInstructionHarness ? DefaultExceptCycles : ExtendedInstructionsTestSuite.DefaultAssertions)
            .ToTestCaseData();

    [Pure]
    private static bool IsInstructionHarness => typeof(THarness) == typeof(Z80NInstructionEmulatorTestHarness);
}