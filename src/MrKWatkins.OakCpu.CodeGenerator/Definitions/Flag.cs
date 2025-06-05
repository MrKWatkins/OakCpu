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

    public override string ToString() => Name;

    [Pure]
    public static IReadOnlyList<Flag> Create(IReadOnlyList<FlagYaml> yamls) => yamls.Select(y => new Flag(y.Name, y.Index, y.Condition, y.NotCondition)).OrderBy(f => f.Index).ToList();
}