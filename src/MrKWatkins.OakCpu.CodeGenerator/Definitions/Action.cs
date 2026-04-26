using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Action(string name, int value, Documentation documentation)
{
    public static Action None { get; } = new("none", 0, new Documentation("No external action is required."));

    public string Name { get; } = name;

    public int Value { get; } = value;

    public Documentation Documentation { get; } = documentation;

    public string EnumName => Name.ToUpperCamelCaseFromSnakeCase();

    public override string ToString() => Name;

    [Pure]
    public static IReadOnlyDictionary<string, Action> Create(CpuYaml yaml) =>
        yaml.Actions
            .Select((action, index) => new Action(action.Name, index + 1, Documentation.Create(action.Documentation, $"action {action.Name}")))
            .Prepend(None)
            .ToDictionary(a => a.Name, a => a);
}