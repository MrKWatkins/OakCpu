using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Flag
{
    internal Flag(string name, int index, string? condition, string? notCondition)
    {
        Name = name;
        Index = index;
        Condition = condition;
        NotCondition = notCondition;
    }

    public string Name { get; }

    public int Index { get; }

    public string? Condition { get; }

    public string? NotCondition { get; }

    public byte BitMask => (byte)(1 << Index);

    public override string ToString() => $"flag.{Name}";

    [Pure]
    public static IReadOnlyDictionary<string, Flag> Create(IReadOnlyList<FlagYaml> yamls) => yamls.Select(y => new Flag(y.Name, y.Index, y.Condition, y.NotCondition)).ToDictionary(f => f.Name);
}