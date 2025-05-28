namespace MrKWatkins.Z80TestSuites.Fuse;

public sealed class Expected : Z80State
{
    private Expected(IReadOnlyList<Event> events)
    {
        Events = events;
    }

    public IReadOnlyList<Event> Events { get; }

    public void Assert(Z80TestHarness testHarness)
    {
        using (testHarness.CreateAssertionScope())
        {
            testHarness.AssertEqual(testHarness.AF, AF, "register AF should match");
            testHarness.AssertEqual(testHarness.BC, BC, "register BC should match");
            testHarness.AssertEqual(testHarness.DE, DE, "register DE should match");
            testHarness.AssertEqual(testHarness.HL, HL, "register HL should match");
            testHarness.AssertEqual(testHarness.I, I, "register I should match");
            testHarness.AssertEqual(testHarness.R, R, "register R should match");
            testHarness.AssertEqual(testHarness.PC, PC, "register PC should match");
            testHarness.AssertEqual(testHarness.SP, SP, "register SP should match");
            testHarness.AssertEqual(testHarness.IX, IX, "register IX should match");
            testHarness.AssertEqual(testHarness.IY, IY, "register IY should match");
            testHarness.AssertEqual(testHarness.WZ, WZ, "register WZ should match");
            testHarness.AssertEqual(testHarness.ShadowAF, ShadowAF, "register AF' should match");
            testHarness.AssertEqual(testHarness.ShadowBC, ShadowBC, "register BC' should match");
            testHarness.AssertEqual(testHarness.ShadowDE, ShadowDE, "register DE' should match");
            testHarness.AssertEqual(testHarness.ShadowHL, ShadowHL, "register HL' should match");
            testHarness.AssertEqual(testHarness.IM, IM, "interrupt IM should match");
            testHarness.AssertEqual(testHarness.IFF1, IFF1, "interrupt IFF should match");
            testHarness.AssertEqual(testHarness.IFF2, IFF2, "interrupt IFF should match");
            testHarness.AssertEqual(testHarness.IsHalted, IsHalted, "IsHalted should match");

            foreach (var memory in Memory)
            {
                var address = memory.Address;
                foreach (var expected in memory.Data)
                {
                    var actual = testHarness.GetMemory(address);
                    testHarness.AssertEqual(actual, expected, $"memory at 0x{address:X4} should match");
                    address++;
                }
            }
        }
    }

    [Pure]
    internal static Expected Parse(StreamReader reader)
    {
        var events = new List<Event>();
        while (true)
        {
            var line = reader.ReadLine()!;
            if (char.IsWhiteSpace(line[0]))
            {
                events.Add(Event.Parse(line));
                continue;
            }

            var state = new Expected(events);
            Parse(line, reader, state);
            return state;
        }
    }
}