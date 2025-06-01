namespace MrKWatkins.Z80TestSuites;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed record TestEvent(TestEventType Type, int TStates, ushort Address, byte? Data)
{
    public override string ToString() => $"{TStates}: {Type}, 0x{Address:X4}{(Data.HasValue ? $" 0x{Data:X2}" : "")}";
}