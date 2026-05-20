using MrKWatkins.EmulatorTestSuites.M6502.Instruction.SingleStep;
using MrKWatkins.OakCpu.M6502.Testing;

namespace MrKWatkins.OakCpu.M6502.Tests;

[Parallelizable(ParallelScope.All)]
[TestFixture]
public sealed class M6502InstructionSingleStepTests
{
    [TestCaseSource(typeof(SingleStepTestCases), nameof(SingleStepTestCases.Create))]
    public void SingleStepTest(SingleStepTestCase testCase) => testCase.Execute<M6502InstructionEmulatorTestHarness>(TestContext.Progress);
}

[Parallelizable(ParallelScope.All)]
[TestFixture]
public sealed class M6502StepSingleStepTests
{
    [TestCaseSource(typeof(SingleStepTestCases), nameof(SingleStepTestCases.Create))]
    public void SingleStepTest(SingleStepTestCase testCase) => testCase.Execute<M6502StepEmulatorTestHarness>(TestContext.Progress);
}

internal static class SingleStepTestCases
{
    private static readonly HashSet<string> SupportedOpcodes =
    [
        "01",
        "05",
        "08",
        "09",
        "0d",
        "11",
        "15",
        "18",
        "19",
        "1d",
        "21",
        "24",
        "25",
        "29",
        "2c",
        "2d",
        "28",
        "31",
        "35",
        "38",
        "39",
        "3d",
        "41",
        "45",
        "48",
        "49",
        "4d",
        "51",
        "55",
        "58",
        "59",
        "5d",
        "61",
        "65",
        "68",
        "69",
        "6d",
        "71",
        "75",
        "78",
        "79",
        "88",
        "8a",
        "98",
        "9a",
        "a0",
        "a1",
        "a2",
        "a4",
        "a5",
        "a6",
        "a8",
        "a9",
        "aa",
        "ac",
        "ad",
        "ae",
        "b1",
        "b4",
        "b5",
        "b8",
        "ba",
        "bc",
        "bd",
        "be",
        "c0",
        "c1",
        "c4",
        "c5",
        "c8",
        "c9",
        "ca",
        "cc",
        "cd",
        "d1",
        "d5",
        "d8",
        "d9",
        "dd",
        "e0",
        "e1",
        "e4",
        "e5",
        "e8",
        "e9",
        "ea",
        "ec",
        "ed",
        "f1",
        "f5",
        "f8"
    ];

    [Pure]
    public static IEnumerable<TestCaseData> Create() =>
        SingleStepTestSuite.Instance
            .GetTestCases()
            .Where(testCase => SupportedOpcodes.Contains(testCase.Id))
            .Select(testCase => new TestCaseData(testCase).SetName(testCase.Name));
}