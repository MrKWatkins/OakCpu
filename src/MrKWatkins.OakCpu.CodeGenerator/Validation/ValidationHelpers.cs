using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Validation;

internal static class ValidationHelpers
{
    [Pure]
    public static IEnumerable<(T Item, int Index)> Indexed<T>(this IEnumerable<T> items) => items.Select((item, index) => (item, index));

    [Pure]
    public static IEnumerable<ValidationError> ValidateDuplicateNames(IEnumerable<(string Name, string Path)> items, string itemType)
    {
        foreach (var duplicate in items
                     .GroupBy(item => item.Name, StringComparer.Ordinal)
                     .Where(group => group.Count() > 1)
                     .OrderBy(group => group.Key, StringComparer.Ordinal))
        {
            yield return new ValidationError(
                $"The {itemType} {duplicate.Key} is defined multiple times.",
                duplicate.Select(item => item.Path).OrderBy(path => path, StringComparer.Ordinal).ToArray());
        }
    }

    [Pure]
    public static IEnumerable<ValidationError> ValidateDuplicateValues<T>(IEnumerable<(T Value, string Path)> items, Func<T, string> messageFactory)
        where T : notnull
    {
        foreach (var duplicate in items
                     .GroupBy(item => item.Value)
                     .Where(group => group.Count() > 1)
                     .OrderBy(group => group.Key))
        {
            yield return new ValidationError(
                messageFactory(duplicate.Key),
                duplicate.Select(item => item.Path).OrderBy(path => path, StringComparer.Ordinal).ToArray());
        }
    }

    [Pure]
    public static string FormatNames(IEnumerable<string> names) => string.Join(", ", names.Distinct(StringComparer.Ordinal).OrderBy(name => name, StringComparer.Ordinal));

    [Pure]
    public static HashSet<string> GetAvailableSequenceNames(YamlFile yaml)
    {
        var names = yaml.Sequences.Select(sequence => sequence.Name).ToHashSet(StringComparer.Ordinal);
        if (yaml.OpcodeRead.Any())
        {
            names.Add("opcode_read");
        }

        return names;
    }
}