using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.Definitions;

/// <summary>
/// A CPU bus control pin, represented as a single bit in the generated pins enum.
/// </summary>
public sealed class Pin(string name, byte value, Documentation documentation)
{
    public string Name { get; } = name;

    public byte Value { get; } = value;

    public Documentation Documentation { get; } = documentation;

    public override string ToString() => Name;

    [Pure]
    public static IReadOnlyDictionary<string, Pin> Create(CpuYaml yaml)
    {
        if (yaml.Pins.Count > 8)
        {
            throw new InvalidOperationException("A maximum of 8 pins can be defined.");
        }

        return yaml.Pins
            .Select((pin, index) => new Pin(pin.Name, (byte)(1 << index), Documentation.Create(pin.Documentation, $"pin {pin.Name}")))
            .ToDictionary(p => p.Name, p => p, StringComparer.Ordinal);
    }

    [Pure]
    public static byte ResolveMask(IReadOnlyDictionary<string, Pin> pins, IEnumerable<string> names, string owner)
    {
        byte mask = 0;
        foreach (var name in names)
        {
            if (!pins.TryGetValue(name, out var pin))
            {
                throw new InvalidOperationException($"Unknown pin {name} for {owner}.");
            }

            mask |= pin.Value;
        }

        return mask;
    }

    [Pure]
    public static IReadOnlyList<byte> ResolveMasks(IReadOnlyDictionary<string, Pin> pins, IEnumerable<IReadOnlyList<string>> names, string owner) =>
        names.Select(row => ResolveMask(pins, row, owner)).ToList();
}