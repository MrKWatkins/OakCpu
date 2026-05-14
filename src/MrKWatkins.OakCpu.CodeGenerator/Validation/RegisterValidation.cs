using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Validation;

internal static class RegisterValidation
{
    [Pure]
    public static IEnumerable<ValidationError> Validate(IReadOnlyList<RegisterYaml> registers) =>
        ValidateRegisterTypes(registers)
            .Concat(ValidateSubRegisterTypes(registers))
            .Concat(ValidateDuplicateNames(registers));

    [Pure]
    private static IEnumerable<ValidationError> ValidateRegisterTypes(IReadOnlyList<RegisterYaml> registers)
    {
        foreach (var (register, index) in registers.Indexed()
                     .Where(item => item.Item.Type != DataType.U8 && item.Item.Type != DataType.U16)
                     .Select(item => (item.Item, item.Index)))
        {
            yield return new ValidationError($"Register {register.Name} must have type u8 or u16.", $"registers[{index}].type");
        }
    }

    [Pure]
    private static IEnumerable<ValidationError> ValidateSubRegisterTypes(IReadOnlyList<RegisterYaml> registers)
    {
        foreach (var (register, index) in registers.Indexed().Select(item => (item.Item, item.Index)))
        {
            if (register.High != null && register.High.Type != DataType.U8)
            {
                yield return new ValidationError($"High register {register.High.Name} of register {register.Name} must have type u8.", $"registers[{index}].high.type");
            }

            if (register.Low != null && register.Low.Type != DataType.U8)
            {
                yield return new ValidationError($"Low register {register.Low.Name} of register {register.Name} must have type u8.", $"registers[{index}].low.type");
            }
        }
    }

    [Pure]
    private static IEnumerable<ValidationError> ValidateDuplicateNames(IReadOnlyList<RegisterYaml> registers) =>
        ValidationHelpers.ValidateDuplicateNames(
            registers.SelectMany(GetRegisterNames),
            "register");

    [Pure]
    private static IEnumerable<(string Name, string Path)> GetRegisterNames(RegisterYaml register, int index)
    {
        yield return (register.Name, $"registers[{index}].name");

        if (register.Type != DataType.U16)
        {
            yield break;
        }

        yield return (
            register.Low?.Name ?? $"{register.Name}L",
            register.Low != null ? $"registers[{index}].low.name" : $"registers[{index}].name (implicit low register)");
        yield return (
            register.High?.Name ?? $"{register.Name}H",
            register.High != null ? $"registers[{index}].high.name" : $"registers[{index}].name (implicit high register)");
    }
}