namespace MrKWatkins.OakCpu.Z80.TestSuites;

public abstract class TestCase
{
    private protected TestCase(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public abstract void Execute<TTestHarness>(TextWriter? testOutput = null)
        where TTestHarness : Z80TestHarness, new();

    public sealed override string ToString() => Name;
}