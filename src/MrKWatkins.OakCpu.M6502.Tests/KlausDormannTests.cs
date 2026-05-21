using MrKWatkins.EmulatorTestSuites.M6502;
using MrKWatkins.EmulatorTestSuites.M6502.Program.KlausDormann;
using MrKWatkins.OakCpu.M6502.Testing;

namespace MrKWatkins.OakCpu.M6502.Tests;

[TestFixture(typeof(M6502StepEmulatorTestHarness))]
[TestFixture(typeof(M6502InstructionEmulatorTestHarness))]
public sealed class KlausDormannTests<THarness>
    where THarness : M6502TestHarness, new()
{
    [TestCaseSource(nameof(TestCases))]
    public void Test(KlausDormannTestCase testCase) => testCase.Execute<THarness>(TestContext.Progress);

    [Pure]
    public static IEnumerable<TestCaseData> TestCases() => KlausDormannTestSuite.Instance.TestCases.Select(testCase => new TestCaseData(testCase).SetName(testCase.Name));
}