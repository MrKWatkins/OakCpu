namespace MrKWatkins.Z80TestSuites;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed record TestEvent
{
    public TestEvent(TestEventType type, int tState, ushort address, byte data)
    {
        Type = type;
        TState = tState;
        Address = address;
        Data = data;
    }

    public TestEventType Type { get; }

    /// <summary>
    /// The T-state which generated the event.
    /// </summary>
    public int TState { get; }

    /// <summary>
    /// The T-state after the event has finished
    /// </summary>
    public int TStateAfter => TState + Length;

    /// <summary>
    /// The length of the event in T-States.
    /// </summary>
    public int Length => Type switch
    {
        TestEventType.MemoryContend => 0,
        TestEventType.OpcodeRead => 4,
        TestEventType.MemoryRead => 3,
        TestEventType.MemoryWrite => 3,
        _ => throw new NotSupportedException($"The {nameof(TestEventType)} {Type} is not supported.")
    };

    public ushort Address { get; }

    public byte Data { get; }

    public override string ToString() => $"{TState}:{Length} - {Type}, 0x{Address:X4} 0x{Data:X2}";
}