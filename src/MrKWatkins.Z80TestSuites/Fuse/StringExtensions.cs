using System.Globalization;

namespace MrKWatkins.Z80TestSuites.Fuse;

internal static class StringExtensions
{
    [Pure]
    internal static bool ToBool(this string value) => value == "1";

    [Pure]
    internal static byte ToByte(this string value) => byte.Parse(value, NumberStyles.AllowHexSpecifier);

    [Pure]
    internal static ushort ToWord(this string value) => ushort.Parse(value, NumberStyles.AllowHexSpecifier);

    [Pure]
    internal static EventType ToEventType(this string value) => value switch
    {
        "MR" => EventType.MemoryRead,
        "MW" => EventType.MemoryWrite,
        "MC" => EventType.MemoryContend,
        "PR" => EventType.PortRead,
        "PW" => EventType.PortWrite,
        "PC" => EventType.PortContend,
        _ => throw new InvalidOperationException($"Unknown event type \"{value}\".")
    };
}