namespace MrKWatkins.Z80TestSuites.Fuse;

public static class Extensions
{
    [Pure]
    public static FuseEvent ToFuse(this TestEvent testEvent)
    {
        var type = testEvent.Type switch
        {
            TestEventType.MemoryContend => FuseEventType.MemoryContend,
            TestEventType.OpcodeRead => FuseEventType.MemoryRead,
            TestEventType.MemoryRead => FuseEventType.MemoryRead,
            TestEventType.MemoryWrite => FuseEventType.MemoryWrite,
            TestEventType.IOContend => FuseEventType.PortContend,
            TestEventType.IORead => FuseEventType.PortRead,
            TestEventType.IOWrite => FuseEventType.PortWrite,
            _ => throw new NotSupportedException($"The {nameof(TestEventType)} {testEvent.Type} is not supported.")
        };

        var tStatesAfter = testEvent.TState + testEvent.Length;
        var data = testEvent.Type is TestEventType.MemoryContend or TestEventType.IOContend ? (byte?)null : testEvent.Data;

        return new FuseEvent(type, tStatesAfter, testEvent.Address, data);
    }
}