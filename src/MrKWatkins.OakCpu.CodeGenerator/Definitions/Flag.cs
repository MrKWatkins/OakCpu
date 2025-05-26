
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Flag
{
    internal Flag(string name, int index)
    {
        Name = name;
        Index = index;
    }

    public string Name { get; }

    public int Index { get; }

    public override string ToString() => Name;

    [Pure]
    public static IReadOnlyList<Flag> Create(IReadOnlyList<FlagYaml> yamls) => yamls.Select(y => new Flag(y.Name, y.Index)).OrderBy(f => f.Index).ToList();
}