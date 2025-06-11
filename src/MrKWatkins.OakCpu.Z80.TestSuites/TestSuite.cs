namespace MrKWatkins.OakCpu.Z80.TestSuites;

public abstract class TestSuite<TTestCase>
    where TTestCase : TestCase
{
    private protected TestSuite(string name, Uri source)
    {
        Name = name;
        Source = source;
    }

    public string Name { get; }

    public Uri Source { get; }

    public abstract IEnumerable<TTestCase> TestCases { get; }

    [MustDisposeResource]
    protected Stream OpenResource(string resource) => GetType().Assembly.GetManifestResourceStream(GetType(), resource) ?? throw new InvalidOperationException($"Resource {resource} not found.");
}