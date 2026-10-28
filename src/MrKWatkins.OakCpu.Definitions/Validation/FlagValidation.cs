using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.Definitions.Validation;

internal static class FlagValidation
{
    [Pure]
    public static IEnumerable<ValidationError> Validate(IReadOnlyList<FlagYaml> flags) =>
        ValidationHelpers.ValidateDuplicateNames(
                flags.Indexed().Select(item => (item.Item.Name, $"flags[{item.Index}].name")),
                "flag")
            .Concat(ValidateDuplicateIndexes(flags))
            .Concat(ValidateDuplicateConditions(flags))
            .Concat(ValidateModes(flags));

    [Pure]
    private static IEnumerable<ValidationError> ValidateDuplicateIndexes(IReadOnlyList<FlagYaml> flags)
    {
        foreach (var duplicate in flags
                     .Indexed()
                     .GroupBy(item => item.Item.Index)
                     .Where(group => group.Count() > 1)
                     .OrderBy(group => group.Key))
        {
            var names = ValidationHelpers.FormatNames(duplicate.Select(item => item.Item.Name));
            yield return new ValidationError(
                $"The flag index {duplicate.Key} is defined multiple times by flags {names}.",
                duplicate.Select(item => $"flags[{item.Index}].index").OrderBy(path => path, StringComparer.Ordinal).ToArray());
        }
    }

    [Pure]
    private static IEnumerable<ValidationError> ValidateDuplicateConditions(IReadOnlyList<FlagYaml> flags)
    {
        foreach (var duplicate in flags
                     .SelectMany(GetConditionNames)
                     .GroupBy(item => item.Name, StringComparer.Ordinal)
                     .Where(group => group.Count() > 1)
                     .OrderBy(group => group.Key, StringComparer.Ordinal))
        {
            var flagsText = ValidationHelpers.FormatNames(duplicate.Select(item => item.FlagName));
            yield return new ValidationError(
                $"The condition {duplicate.Key} is defined multiple times by flags {flagsText}.",
                duplicate.Select(item => item.Path).OrderBy(path => path, StringComparer.Ordinal).ToArray());
        }
    }

    [Pure]
    private static IEnumerable<(string Name, string FlagName, string Path)> GetConditionNames(FlagYaml flag, int index)
    {
        if (flag.Condition != null)
        {
            yield return (flag.Condition, flag.Name, $"flags[{index}].condition");
        }

        if (flag.NotCondition != null)
        {
            yield return (flag.NotCondition, flag.Name, $"flags[{index}].not_condition");
        }
    }

    [Pure]
    private static IEnumerable<ValidationError> ValidateModes(IReadOnlyList<FlagYaml> flags)
    {
        foreach (var (flag, index) in flags.Indexed())
        {
            if (flag.Mode == null)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(flag.Mode.Set))
            {
                yield return new ValidationError($"The flag {flag.Name} has a mode with no name for when it is set.", $"flags[{index}].mode.set");
            }

            if (string.IsNullOrWhiteSpace(flag.Mode.Reset))
            {
                yield return new ValidationError($"The flag {flag.Name} has a mode with no name for when it is reset.", $"flags[{index}].mode.reset");
            }
        }
    }
}