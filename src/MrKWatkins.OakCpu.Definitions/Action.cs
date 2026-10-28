using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.Definitions;

public sealed class Action(string name, int value, Documentation documentation, IReadOnlyList<byte>? pinPattern = null)
{
    public static Action None { get; } = new("none", 0, new Documentation("No external action is required."));

    public string Name { get; } = name;

    public int Value { get; } = value;

    public Documentation Documentation { get; } = documentation;

    /// <summary>
    /// The pins asserted during the T-states of the machine cycle the action belongs to, one entry per T-state starting at the
    /// step that requests the action, or <c>null</c> if the CPU does not define pins for the action.
    /// </summary>
    public IReadOnlyList<byte>? PinPattern { get; } = pinPattern;

    public string EnumName => Name.ToUpperCamelCaseFromSnakeCase();

    public override string ToString() => Name;

    [Pure]
    public static IReadOnlyDictionary<string, Action> Create(CpuYaml yaml, IReadOnlyDictionary<string, Pin> pins) =>
        yaml.Actions
            .Select((action, index) => new Action(
                action.Name,
                index + 1,
                Documentation.Create(action.Documentation, $"action {action.Name}"),
                CreatePinPattern(pins, action)))
            .Prepend(None)
            .ToDictionary(a => a.Name, a => a);

    [Pure]
    private static IReadOnlyList<byte>? CreatePinPattern(IReadOnlyDictionary<string, Pin> pins, ActionYaml action) =>
        action.Pins?.Select(row => Pin.ResolveMask(pins, row, $"action {action.Name}")).ToList();
}