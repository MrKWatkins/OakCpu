using System.Collections.Frozen;

namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction;

public sealed class InstructionTestSuiteOptions
{
    public Assertions AssertionsToRun { get; init; } = Assertions.All;

    public MemoryCycleMethod MemoryCycleMethod { get; init; } = MemoryCycleMethod.Start;

    public IReadOnlyDictionary<string, Assertions> AssertionsToRunOverrides { get; init; } = FrozenDictionary<string, Assertions>.Empty;

    [Pure]
    public Assertions GetAssertionsToRunFor(string testName) => AssertionsToRunOverrides.GetValueOrDefault(testName, AssertionsToRun);
}