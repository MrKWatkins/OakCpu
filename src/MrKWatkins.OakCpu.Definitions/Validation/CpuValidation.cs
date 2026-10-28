using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.Definitions.Validation;

internal static class CpuValidation
{
    [Pure]
    public static IEnumerable<ValidationError> Validate(CpuYaml cpu) =>
        ValidateCycles(cpu)
            .Concat(ValidateOperands(cpu))
            .Concat(ValidateReferences(cpu));

    [Pure]
    private static IEnumerable<ValidationError> ValidateCycles(CpuYaml cpu)
    {
        if (cpu.Cycles == null)
        {
            yield break;
        }

        if (cpu.Cycles.Name != null && string.IsNullOrWhiteSpace(cpu.Cycles.Name))
        {
            yield return new ValidationError("The cycles have no name.", "cpu.cycles.name");
        }

        if (cpu.Cycles.EffectiveMinimum < 1)
        {
            yield return new ValidationError($"The minimum cycles {cpu.Cycles.EffectiveMinimum} is less than one.", "cpu.cycles.minimum");
        }
    }

    [Pure]
    private static IEnumerable<ValidationError> ValidateOperands(CpuYaml cpu)
    {
        foreach (var error in ValidationHelpers.ValidateDuplicateNames(cpu.Operands.Indexed().Select(item => (item.Item.Name, $"cpu.operands[{item.Index}].name")), "operand"))
        {
            yield return error;
        }

        foreach (var (operand, index) in cpu.Operands.Indexed())
        {
            if (string.IsNullOrWhiteSpace(operand.Name) || !operand.Name.All(char.IsAsciiLetter))
            {
                yield return new ValidationError($"The operand name {operand.Name} is not one or more letters.", $"cpu.operands[{index}].name");
            }

            if (operand.EffectiveBytes < 1)
            {
                yield return new ValidationError($"The operand {operand.Name} has {operand.EffectiveBytes} bytes, which is less than one.", $"cpu.operands[{index}].bytes");
            }
        }
    }

    [Pure]
    private static IEnumerable<ValidationError> ValidateReferences(CpuYaml cpu)
    {
        foreach (var (reference, index) in cpu.References.Indexed())
        {
            if (string.IsNullOrWhiteSpace(reference.Name))
            {
                yield return new ValidationError("The reference has no name.", $"cpu.references[{index}].name");
            }

            if (!Uri.TryCreate(reference.Url, UriKind.Absolute, out _))
            {
                yield return new ValidationError($"The reference {reference.Name} has the invalid URL {reference.Url}.", $"cpu.references[{index}].url");
            }
        }
    }
}