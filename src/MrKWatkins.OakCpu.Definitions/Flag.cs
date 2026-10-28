using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.Definitions;

public sealed class Flag
{
    internal Flag(string name, int index, Documentation documentation, string? condition, string? notCondition, FlagMode? mode = null)
    {
        Mode = mode;
        Name = name;
        Index = index;
        Documentation = documentation;
        Condition = condition;
        NotCondition = notCondition;
    }

    public string Name { get; }

    public int Index { get; }

    public Documentation Documentation { get; }

    public string? Condition { get; }

    public string? NotCondition { get; }

    /// <summary>
    /// The modes the flag selects, if it selects a mode of operation rather than recording a result.
    /// </summary>
    public FlagMode? Mode { get; }

    public byte BitMask => (byte)(1 << Index);

    public override string ToString() => $"flag.{Name}";

    [Pure]
    public static IReadOnlyDictionary<string, Flag> Create(IReadOnlyList<FlagYaml> yamls) =>
        yamls.Select(y => new Flag(y.Name, y.Index, Documentation.Create(y.Documentation, $"flag {y.Name}"), y.Condition, y.NotCondition, FlagMode.Create(y.Mode))).ToDictionary(f => f.Name);
}