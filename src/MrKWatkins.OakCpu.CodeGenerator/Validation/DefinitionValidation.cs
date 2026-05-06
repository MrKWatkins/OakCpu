using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Validation;

internal static class DefinitionValidation
{
    public static void Validate(YamlFile yaml)
    {
        var errors = ActionValidation.Validate(yaml.Cpu.Actions)
            .Concat(FunctionValidation.Validate(yaml.Functions))
            .Concat(DataMemberValidation.Validate(yaml.Cpu.Fields, yaml.Interrupts.Properties))
            .Concat(RegisterValidation.Validate(yaml.Registers))
            .Concat(FlagValidation.Validate(yaml.Flags))
            .Concat(SequenceValidation.Validate(yaml))
            .Concat(InstructionValidation.Validate(yaml.Instructions, yaml))
            .ToArray();

        if (errors.Length == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Definition validation failed:{Environment.NewLine}{string.Join(Environment.NewLine, errors.Select(error => $"- {error}"))}");
    }
}