namespace MrKWatkins.OakCpu.Z80.TestSuites.InstructionLevel;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed record TestEvent(TestEventType Type, int TState, ushort Address, byte Data)
{
    /// <summary>
    /// The T-state which generated the event.
    /// </summary>
    public int TState { get; } = TState;

    public override string ToString() => $"{TState}: {Type}, 0x{Address:X4} 0x{Data:X2}";
}