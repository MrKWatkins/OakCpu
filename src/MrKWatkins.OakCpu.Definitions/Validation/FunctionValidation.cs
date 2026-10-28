using System.Text.RegularExpressions;
using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.Definitions.Validation;

internal static partial class FunctionValidation
{
    [GeneratedRegex(@"\{([^}]*)\}")]
    private static partial Regex PlaceholderRegex();

    [Pure]
    public static IEnumerable<ValidationError> Validate(IReadOnlyList<FunctionYaml> functions) =>
        ValidationHelpers.ValidateDuplicateNames(
                functions.Indexed().Select(item => (item.Item.Name, $"functions[{item.Index}].name")),
                "function")
            .Concat(ValidateDocumentation(functions));

    /// <summary>
    /// The placeholders in a function's documentation must be its parameters.
    /// </summary>
    [Pure]
    private static IEnumerable<ValidationError> ValidateDocumentation(IReadOnlyList<FunctionYaml> functions)
    {
        foreach (var (function, index) in functions.Indexed())
        {
            var documentation = new (string? Text, string Key)[]
            {
                (function.Documentation, "documentation"),
                (function.NotDocumentation, "not_documentation"),
                (function.ValueDocumentation, "value_documentation")
            };

            foreach (var (text, key) in documentation)
            {
                if (text == null)
                {
                    continue;
                }

                var unknown = PlaceholderRegex().Matches(text)
                    .Select(match => match.Groups[1].Value)
                    .Where(placeholder => !function.Parameters.Contains(placeholder, StringComparer.Ordinal))
                    .Distinct(StringComparer.Ordinal)
                    .ToList();

                if (unknown.Count > 0)
                {
                    yield return new ValidationError(
                        $"The {key} of the function {function.Name} uses placeholders that are not its parameters: {ValidationHelpers.FormatNames(unknown)}.",
                        $"functions[{index}].{key}");
                }
            }
        }
    }
}