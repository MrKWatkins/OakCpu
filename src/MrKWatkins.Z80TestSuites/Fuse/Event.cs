namespace MrKWatkins.Z80TestSuites.Fuse;

public sealed class Event
{
    private Event(int time, EventType type, ushort address, byte? data)
    {
        Time = time;
        Type = type;
        Address = address;
        Data = data;
    }

    public EventType Type { get; }

    public int Time { get; }

    public ushort Address { get; }

    public byte? Data { get; }

    public override string ToString() => $"{Type}: {Time} T-States After, 0x{Address:X4} {Data?.ToString("X2") ?? ""}";

    [Pure]
    internal static Event Parse(string line)
    {
        var components = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var time = int.Parse(components[0]);
        var type = components[1].ToEventType();
        var address = components[2].ToWord();
        byte? data = components.Length == 4 ? components[3].ToByte() : null;

        return new Event(time, type, address, data);
    }
}