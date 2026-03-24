using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class NamedStepSequence : StepSequence
{
    private NamedStepSequence(string name, IReadOnlyList<Step> steps, NextOpcodeMode nextOpcode, bool executeOverlapOnStart)
        : base(name, steps, nextOpcode, executeOverlapOnStart)
    {
    }

    [Pure]
    public static NamedStepSequence Create(ParserContext context, StepSequenceYaml yaml) =>
        new(yaml.Name, Step.Parse(GetDisplayName(yaml.Name), context, yaml.Steps), yaml.NextOpcode, yaml.ExecuteOverlapOnStart);

    [Pure]
    public static NamedStepSequence Create(
        ParserContext context,
        string name,
        IReadOnlyList<string?> steps,
        NextOpcodeMode nextOpcode,
        bool executeOverlapOnStart = false) =>
        new(name, Step.Parse(GetDisplayName(name), context, steps), nextOpcode, executeOverlapOnStart);

    [Pure]
    private static string GetDisplayName(string name) =>
        name switch
        {
            "opcode_read" => "Opcode read",
            "halted" => "Halt cycle",
            "halted_cycle" => "Halt cycle",
            _ when name.StartsWith("interrupt_mode_", StringComparison.Ordinal) &&
                   byte.TryParse(name["interrupt_mode_".Length..], out var mode)
                => $"Interrupt Mode {mode}",
            _ => name.Replace('_', ' ')
        };
}