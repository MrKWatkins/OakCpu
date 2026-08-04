using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class NamedStepSequence : StepSequence
{
    private NamedStepSequence(string name, IReadOnlyList<Step> steps, NextOpcodeMode nextOpcode, bool executeOverlapOnStart, IReadOnlyList<byte>? stepPins = null)
        : base(name, steps, nextOpcode, executeOverlapOnStart, overrideStepPins: stepPins)
    {
    }

    [Pure]
    public static NamedStepSequence Create(ParserContext context, StepSequenceYaml yaml) =>
        new(yaml.Name, Step.Parse(GetDisplayName(yaml.Name), context, yaml.Steps), yaml.NextOpcode, yaml.ExecuteOverlapOnStart, CreateOverrideStepPins(context, yaml));

    [Pure]
    public static NamedStepSequence Create(
        ParserContext context,
        string name,
        IReadOnlyList<string?> steps,
        NextOpcodeMode nextOpcode,
        bool executeOverlapOnStart = false) =>
        new(name, Step.Parse(GetDisplayName(name), context, steps), nextOpcode, executeOverlapOnStart);

    [Pure]
    private static IReadOnlyList<byte>? CreateOverrideStepPins(ParserContext context, StepSequenceYaml yaml)
    {
        if (yaml.OverrideStepPins == null)
        {
            return null;
        }

        if (yaml.OverrideStepPins.Count > yaml.Steps.Count)
        {
            throw new InvalidOperationException($"The sequence {yaml.Name} has more override_step_pins entries than steps.");
        }

        return Pin.ResolveMasks(context.Configuration.Pins, yaml.OverrideStepPins, $"sequence {yaml.Name}");
    }

    [Pure]
    private static string GetDisplayName(string name) =>
        name switch
        {
            "opcode_read" => "Opcode read",
            "halted" => "Halt cycle",
            _ when name.StartsWith("interrupt_mode_", StringComparison.Ordinal) &&
                   byte.TryParse(name["interrupt_mode_".Length..], out var mode)
                => $"Interrupt Mode {mode}",
            _ => name.Replace('_', ' ')
        };
}