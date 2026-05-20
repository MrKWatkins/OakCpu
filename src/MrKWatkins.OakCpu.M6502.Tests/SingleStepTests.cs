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
        "06",
        "0a",
        "0e",
        "01",
        "05",
        "08",
        "09",
        "16",
        "0d",
        "11",
        "15",
        "18",
        "1e",
        "19",
        "1d",
        "21",
        "24",
        "25",
        "26",
        "29",
        "2a",
        "2c",
        "2d",
        "2e",
        "28",
        "31",
        "35",
        "36",
        "38",
        "39",
        "3e",
        "3d",
        "41",
        "45",
        "46",
        "48",
        "49",
        "4a",
        "4d",
        "4e",
        "51",
        "55",
        "56",
        "58",
        "59",
        "5e",
        "5d",
        "61",
        "65",
        "66",
        "68",
        "69",
        "6a",
        "6d",
        "6e",
        "71",
        "75",
        "76",
        "78",
        "79",
        "7e",
        "81",
        "84",
        "85",
        "86",
        "88",
        "8a",
        "8c",
        "8d",
        "8e",
        "91",
        "94",
        "95",
        "96",
        "98",
        "99",
        "9a",
        "9d",
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
        "c6",
        "c8",
        "c9",
        "ca",
        "cc",
        "cd",
        "ce",
        "d1",
        "d5",
        "d6",
        "d8",
        "d9",
        "dd",
        "de",
        "e0",
        "e1",
        "e4",
        "e5",
        "e6",
        "e8",
        "e9",
        "ea",
        "ec",
        "ed",
        "ee",
        "f1",
        "f5",
        "f6",
        "f8",
        "fe"
    ];

    [Pure]
    public static IEnumerable<TestCaseData> Create() =>
        SingleStepTestSuite.Instance
            .GetTestCases()
            .Where(testCase => SupportedOpcodes.Contains(testCase.Id))
            .Select(testCase => new TestCaseData(testCase).SetName(testCase.Name));
}