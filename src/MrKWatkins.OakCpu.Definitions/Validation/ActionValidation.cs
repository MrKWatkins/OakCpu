using MrKWatkins.OakCpu.Definitions.Yaml;
using Action = MrKWatkins.OakCpu.Definitions.Action;

namespace MrKWatkins.OakCpu.Definitions.Validation;

internal static class ActionValidation
{
    [Pure]
    public static IEnumerable<ValidationError> Validate(IReadOnlyList<ActionYaml> actions) =>
        ValidationHelpers.ValidateDuplicateNames(
            actions.Indexed().Select(item => (item.Item.Name, $"cpu.actions[{item.Index}].name"))
                .Append((Action.None.Name, "builtin.actions.none")),
            "action");
}