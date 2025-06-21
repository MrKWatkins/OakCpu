namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction;

public abstract class InstructionTestSuite<TTestCase> : TestSuite
    where TTestCase : InstructionTestCase
{
    private protected InstructionTestSuite(string name, Uri source)
        : base(name, source)
    {
    }

    protected virtual InstructionTestSuiteOptions DefaultOptions { get; } = new();

    [Pure]
    public IEnumerable<TTestCase> GetTestCases() => GetTestCases(DefaultOptions);

    [Pure]
    public abstract IEnumerable<TTestCase> GetTestCases(InstructionTestSuiteOptions options);
}