using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Validation;

internal static class FunctionValidation
{
    [Pure]
    public static IEnumerable<ValidationError> Validate(IReadOnlyList<FunctionYaml> functions) =>
        ValidationHelpers.ValidateDuplicateNames(
            functions.Indexed().Select(item => (item.Item.Name, $"functions[{item.Index}].name")),
            "function");
}