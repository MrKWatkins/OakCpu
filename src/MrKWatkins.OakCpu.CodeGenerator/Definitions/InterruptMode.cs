using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class InterruptMode : StepSequence
{
    private InterruptMode(byte number, IReadOnlyList<Step> steps, NextOpcodeMode nextOpcode)
        : base(steps, nextOpcode)
    {
        Number = number;
    }

    public byte Number { get; }

    [Pure]
    public static InterruptMode Create(ParserContext context, InterruptModeYaml yaml) => new(yaml.Number, Step.Parse($"Interrupt Mode {yaml.Number}", context, yaml.Steps), yaml.NextOpcode);
}