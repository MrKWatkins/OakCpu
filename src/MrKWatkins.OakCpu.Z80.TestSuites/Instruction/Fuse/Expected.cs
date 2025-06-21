namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction.Fuse;

public sealed class Expected : FuseZ80State
{
    private Expected(IReadOnlyList<FuseEvent> events)
    {
        Events = events;
    }

    public IReadOnlyList<FuseEvent> Events { get; }

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

            var expected = new Expected(events);
            expected.Parse(line, reader);
            return expected;
        }
    }
}