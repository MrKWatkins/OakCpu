using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Validation;

internal static class DataMemberValidation
{
    [Pure]
    public static IEnumerable<ValidationError> Validate(IReadOnlyList<FieldYaml> cpuFields, IReadOnlyList<FieldYaml> interruptProperties) =>
        ValidationHelpers.ValidateDuplicateNames(
            cpuFields.Select((field, index) => (field.Name, $"cpu.fields[{index}].name"))
                .Concat(interruptProperties.Select((field, index) => (field.Name, $"interrupts.properties[{index}].name"))),
            "data member");
}