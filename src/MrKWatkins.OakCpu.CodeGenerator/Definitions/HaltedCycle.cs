using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class HaltedCycle : StepSequence
{
    private HaltedCycle(IReadOnlyList<Step> steps)
        : base(steps, NextOpcodeMode.Loop)
    {
    }

    [Pure]
    public static HaltedCycle Create(ParserContext context, InterruptsYaml yaml) => new(Step.Parse("Halt cycle", context, yaml.HaltedCycle));
}