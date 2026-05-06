using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Validation;

internal static class FunctionValidation
{
    [Pure]
    public static IEnumerable<ValidationError> Validate(IReadOnlyList<FunctionYaml> functions) =>
        ValidationHelpers.ValidateDuplicateNames(
            functions.Select((function, index) => (function.Name, $"functions[{index}].name")),
            "function");
}