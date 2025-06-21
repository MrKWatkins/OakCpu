namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction.Fuse;

public static class Extensions
{
    /*
    [Pure]
    public static FuseEvent ToFuse(this TestEvent testEvent)
    {
        var (type, length) = testEvent.Type switch
        {
            TestEventType.MemoryContend => (FuseEventType.MemoryContend, 0),
            TestEventType.OpcodeRead => (FuseEventType.MemoryRead, 4),
            TestEventType.MemoryRead => (FuseEventType.MemoryRead, 3),
            TestEventType.MemoryWrite => (FuseEventType.MemoryWrite, 3),
            TestEventType.IOContend => (FuseEventType.PortContend, 0),
            TestEventType.IORead => (FuseEventType.PortRead, 0),
            TestEventType.IOWrite => (FuseEventType.PortWrite, 0),
            _ => throw new NotSupportedException($"The {nameof(TestEventType)} {testEvent.Type} is not supported.")
        };

        var tStatesAfter = (int)testEvent.TState + length;
        var data = testEvent.Type is TestEventType.MemoryContend or TestEventType.IOContend ? (byte?)null : testEvent.Data;

        return new FuseEvent(type, tStatesAfter, testEvent.Address, data);
    }*/
}