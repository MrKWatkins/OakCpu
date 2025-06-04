namespace MrKWatkins.Z80TestSuites.Fuse;

public sealed class FuseTestCase
{
    internal FuseTestCase(string name, Input input, Expected expected, FuseAssertions assertionsToRun = FuseAssertions.All)
    {
        Name = name;
        Input = input;
        Expected = expected;
        AssertionsToRun = assertionsToRun;
    }

    public string Name { get; }

    public Input Input { get; }

    public Expected Expected { get; }

    public FuseAssertions AssertionsToRun { get; set; }

    public void Execute<TTestHarness>()
        where TTestHarness : Z80TestHarness, new()
    {
        var testHarness = new TTestHarness();

        Input.Setup(testHarness);

        while (testHarness.TStates <= Expected.TStates)
        {
            testHarness.ExecuteStep();
        }

        // If the last event was an OpcodeRead, then we've had an overlapped read. The Fuse tests assume instruction level
        // execution so won't take this into account. We need to adjust the PC and remove the event, plus the associated
        // MemoryContend event.
        if (testHarness.Events.LastOrDefault()?.Type == TestEventType.OpcodeRead)
        {
            testHarness.RegisterPC--;
            testHarness.RemoveLastEvent();
            testHarness.RemoveLastEvent();
        }

        Expected.Assert(AssertionsToRun, testHarness);
    }

    public override string ToString() => Name;
}