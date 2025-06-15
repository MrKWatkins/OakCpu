namespace MrKWatkins.OakCpu.Z80.TestSuites.InstructionLevel.Fuse;

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
        var z80 = new TTestHarness();

        Input.Setup(z80);

        var events = new List<TestEvent>();
        while (z80.TStates <= (ulong)Expected.TStates)
        {
            events.AddRange(z80.ExecuteStepRecordingEvents());
        }

        // If the last event was an OpcodeRead, then we've had an overlapped read. The Fuse tests assume instruction level
        // execution so won't take this into account. We need to adjust the PC and remove the event, plus the associated
        // MemoryContend event.
        if (events.LastOrDefault()?.Type == TestEventType.OpcodeRead)
        {
            z80.RegisterPC--;
            events.RemoveAt(events.Count - 1);
            events.RemoveAt(events.Count - 1);
        }

        Expected.Assert(AssertionsToRun, z80, events);
    }

    public override string ToString() => Name;
}