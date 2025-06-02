namespace MrKWatkins.Z80TestSuites.Fuse;

public sealed class Expected : Z80State
{
    private Expected(IReadOnlyList<FuseEvent> events)
    {
        Events = events;
    }

    public IReadOnlyList<FuseEvent> Events { get; }

    public void Assert(Z80TestHarness testHarness)
    {
        using (testHarness.CreateAssertionScope())
        {
            testHarness.AssertEqual(testHarness.RegisterAF, RegisterAF, "register AF should match");
            testHarness.AssertEqual(testHarness.RegisterBC, RegisterBC, "register BC should match");
            testHarness.AssertEqual(testHarness.RegisterDE, RegisterDE, "register DE should match");
            testHarness.AssertEqual(testHarness.RegisterHL, RegisterHL, "register HL should match");
            testHarness.AssertEqual(testHarness.RegisterI, RegisterI, "register I should match");
            testHarness.AssertEqual(testHarness.RegisterR, RegisterR, "register R should match");
            testHarness.AssertEqual(testHarness.RegisterPC, RegisterPC, "register PC should match");
            testHarness.AssertEqual(testHarness.RegisterSP, RegisterSP, "register SP should match");
            testHarness.AssertEqual(testHarness.RegisterIX, RegisterIX, "register IX should match");
            testHarness.AssertEqual(testHarness.RegisterIY, RegisterIY, "register IY should match");
            testHarness.AssertEqual(testHarness.RegisterWZ, RegisterWZ, "register WZ should match");
            testHarness.AssertEqual(testHarness.ShadowRegisterAF, ShadowRegisterAF, "register AF' should match");
            testHarness.AssertEqual(testHarness.ShadowRegisterBC, ShadowRegisterBC, "register BC' should match");
            testHarness.AssertEqual(testHarness.ShadowRegisterDE, ShadowRegisterDE, "register DE' should match");
            testHarness.AssertEqual(testHarness.ShadowRegisterHL, ShadowRegisterHL, "register HL' should match");
            testHarness.AssertEqual(testHarness.FlagC, FlagC, "flag C should match");
            testHarness.AssertEqual(testHarness.FlagN, FlagN, "flag N should match");
            testHarness.AssertEqual(testHarness.FlagPV, FlagPV, "flag PV should match");
            testHarness.AssertEqual(testHarness.FlagX, FlagX, "flag X should match");
            testHarness.AssertEqual(testHarness.FlagH, FlagH, "flag H should match");
            testHarness.AssertEqual(testHarness.FlagY, FlagY, "flag Y should match");
            testHarness.AssertEqual(testHarness.FlagZ, FlagZ, "flag Z should match");
            testHarness.AssertEqual(testHarness.FlagS, FlagS, "flag S should match");
            testHarness.AssertEqual(testHarness.IM, IM, "interrupt IM should match");
            testHarness.AssertEqual(testHarness.IFF1, IFF1, "interrupt IFF should match");
            testHarness.AssertEqual(testHarness.IFF2, IFF2, "interrupt IFF should match");
            testHarness.AssertEqual(testHarness.IsHalted, IsHalted, "IsHalted should match");

            AssertMemory(testHarness);

            AssertEvents(testHarness);
        }
    }

    private void AssertMemory(Z80TestHarness testHarness)
    {
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

    private void AssertEvents(Z80TestHarness testHarness)
    {
        var actualEvents = Events;
        var expectedEvents = testHarness.Events.Select(e => e.ToFuse()).ToList();
        testHarness.AssertEqual(actualEvents.Count, expectedEvents.Count, "number of events should match");

        for (var f = 0; f < Math.Min(actualEvents.Count, expectedEvents.Count); f++)
        {
            var actual = actualEvents[f];
            var expected = expectedEvents[f];

            testHarness.AssertEqual(actual, expected, $"event {f} should match");
        }
    }

    [Pure]
    internal static Expected Parse(StreamReader reader)
    {
        var events = new List<FuseEvent>();
        while (true)
        {
            var line = reader.ReadLine()!;
            if (char.IsWhiteSpace(line[0]))
            {
                events.Add(FuseEvent.Parse(line));
                continue;
            }

            var state = new Expected(events);
            Parse(line, reader, state);
            return state;
        }
    }
}