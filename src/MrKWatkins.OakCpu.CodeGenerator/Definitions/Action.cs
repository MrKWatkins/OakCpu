using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Action(string name, int value)
{
    public static Action None { get; } = new("none", 0);

    public string Name { get; } = name;

    public int Value { get; } = value;

    public string EnumName => Name.ToUpperCamelCaseFromSnakeCase();

    public override string ToString() => Name;

    [Pure]
    public static IReadOnlyDictionary<string, Action> Create(CpuYaml yaml) =>
        yaml.Actions.Select((action, index) => new Action(action, index + 1)).Prepend(None).ToDictionary(a => a.Name, a => a);
}