namespace MrKWatkins.Z80TestSuites.Fuse;

public sealed class FuseTestCase
{
    internal FuseTestCase(string name, Input input, Expected expected)
    {
        Name = name;
        Input = input;
        Expected = expected;
    }

    public string Name { get; }

    public Input Input { get; }

    public Expected Expected { get; }

    public void Execute<TTestHarness>()
        where TTestHarness : Z80TestHarness, new()
    {
        var testHarness = new TTestHarness();

        Input.Setup(testHarness);

        while (testHarness.TStates < Input.TStates)
        {
            testHarness.ExecuteInstruction();
        }

        Expected.Assert(testHarness);
    }

    public override string ToString() => Name;
}