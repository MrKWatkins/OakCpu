using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Validation;

internal static class ActionValidation
{
    [Pure]
    public static IEnumerable<ValidationError> Validate(IReadOnlyList<ActionYaml> actions) =>
        ValidationHelpers.ValidateDuplicateNames(
            actions.Select((action, index) => (action.Name, $"cpu.actions[{index}].name"))
                .Append((Action.None.Name, "builtin.actions.none")),
            "action");
}