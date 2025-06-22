namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction.Fuse;

public sealed class FuseTestSuite : InstructionTestSuite<FuseTestCase>
{
    public static readonly FuseTestSuite Instance = new();

    private FuseTestSuite()
        : base("Fuse", new Uri("https://fuse-emulator.sourceforge.net/"))
    {
    }

    public override InstructionTestSuiteOptions DefaultOptions { get; } = new() { AssertionsToRun = Assertions.All & ~Assertions.Q };

    public override IEnumerable<FuseTestCase> GetTestCases(InstructionTestSuiteOptions options)
    {
        var inputs = ParseInput();
        var expected = ParseExpected();

        foreach (var (name, input) in inputs)
        {
            yield return new FuseTestCase(name, options, input, expected[name]);
        }
    }

    [Pure]
    private IReadOnlyDictionary<string, Input> ParseInput()
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
    private IReadOnlyDictionary<string, Expected> ParseExpected()
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
}