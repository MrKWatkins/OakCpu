namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction.SingleStep;

public sealed class SingleStepTestSuite : InstructionTestSuite<SingleStepTestCase>
{
    internal const string ResourcePrefix = "MrKWatkins.OakCpu.Z80.TestSuites.Instruction.SingleStep.TestCases.";

    public static readonly SingleStepTestSuite Instance = new();

    private SingleStepTestSuite()
        : base("Single Step", new Uri("https://github.com/SingleStepTests/z80"))
    {
    }

    protected override InstructionTestSuiteOptions DefaultOptions { get; } = new() { AssertionsToRun = Assertions.All & ~Assertions.Halted };

    public override IEnumerable<SingleStepTestCase> GetTestCases(InstructionTestSuiteOptions options)
    {
        foreach (var resource in typeof(SingleStepTestSuite).Assembly.GetManifestResourceNames().Where(n => n.StartsWith(ResourcePrefix, StringComparison.Ordinal)))
        {
            var name = resource[ResourcePrefix.Length..];

            yield return new SingleStepTestCase(name, options);
        }
    }
}