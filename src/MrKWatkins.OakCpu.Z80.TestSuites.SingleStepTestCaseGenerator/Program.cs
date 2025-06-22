using MrKWatkins.OakCpu.Z80.TestSuites.SingleStepTestCaseGenerator;
using MrKWatkins.OakCpu.Z80.TestSuites.SingleStepTestCaseGenerator.Json;

await Parallel.ForEachAsync(JsonTestCases.EnumerateTestCases(), (steps, _) => TestCaseGenerator.Generate(steps));