namespace MrKWatkins.OakCpu.Z80.TestSuites.Fuse;

public static class FuseTestSuite
{
    private static readonly Lazy<IReadOnlyList<FuseTestCase>> LazyTestCases = new(() => BuildTests().ToList());

    public static IReadOnlyList<FuseTestCase> TestCases => LazyTestCases.Value;

    [Pure]
    public static IEnumerable<FuseTestCase> TestCasesWithOverriddenAssertions(IReadOnlyDictionary<string, FuseAssertions> overrides)
    {
        foreach (var testCase in TestCases)
        {
            testCase.AssertionsToRun = overrides.GetValueOrDefault(testCase.Name, FuseAssertions.All);
            yield return testCase;
        }
    }

    [Pure]
    private static IEnumerable<FuseTestCase> BuildTests()
    {
        var inputs = ParseInput();
        var expected = ParseExpected();

        foreach (var (name, input) in inputs)
        {
            yield return new FuseTestCase(name, input, expected[name]);
        }
    }

    [Pure]
    private static IReadOnlyDictionary<string, Input> ParseInput()
    {
        using var stream = OpenResource("tests.in");
        using var reader = new StreamReader(stream);

        return ParseInput(reader);
    }

    [Pure]
    private static IReadOnlyDictionary<string, Input> ParseInput(StreamReader reader)
    {
        var input = new Dictionary<string, Input>();
        while (reader.ReadLine() is { } name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            input.Add(name, Input.Parse(reader));
        }
        return input;
    }

    [Pure]
    private static IReadOnlyDictionary<string, Expected> ParseExpected()
    {
        using var stream = OpenResource("tests.expected");
        using var reader = new StreamReader(stream);

        return ParseExpected(reader);
    }

    [Pure]
    private static IReadOnlyDictionary<string, Expected> ParseExpected(StreamReader reader)
    {
        var expected = new Dictionary<string, Expected>();
        while (reader.ReadLine() is { } name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            expected.Add(name, Expected.Parse(reader));
        }
        return expected;
    }
    [Pure]
    private static Stream OpenResource(string resource) =>
        typeof(FuseTestSuite).Assembly.GetManifestResourceStream(typeof(FuseTestSuite), resource)
        ?? throw new InvalidOperationException($"Resource {resource} not found.");
}