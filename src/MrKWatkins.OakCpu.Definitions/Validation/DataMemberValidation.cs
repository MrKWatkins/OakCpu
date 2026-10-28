using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.Definitions.Validation;

internal static class DataMemberValidation
{
    [Pure]
    public static IEnumerable<ValidationError> Validate(IReadOnlyList<FieldYaml> cpuFields, IReadOnlyList<FieldYaml> interruptProperties) =>
        ValidationHelpers.ValidateDuplicateNames(
            cpuFields.Indexed().Select(item => (item.Item.Name, $"cpu.fields[{item.Index}].name"))
                .Concat(interruptProperties.Indexed().Select(item => (item.Item.Name, $"interrupts.properties[{item.Index}].name"))),
            "data member");
}