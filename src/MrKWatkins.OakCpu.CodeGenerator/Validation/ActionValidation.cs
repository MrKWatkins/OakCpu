using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Validation;

internal static class ActionValidation
{
    [Pure]
    public static IEnumerable<ValidationError> Validate(IReadOnlyList<ActionYaml> actions) =>
        ValidationHelpers.ValidateDuplicateNames(
            actions.Indexed().Select(item => (item.Item.Name, $"cpu.actions[{item.Index}].name"))
                .Append((Action.None.Name, "builtin.actions.none")),
            "action");
}