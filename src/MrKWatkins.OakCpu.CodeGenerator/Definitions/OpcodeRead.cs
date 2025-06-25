using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class OpcodeRead : StepSequence
{
    private OpcodeRead(IReadOnlyList<Step> steps)
        : base(steps, NextOpcodeMode.Custom)
    {
    }

    [Pure]
    public static OpcodeRead Create(ParserContext context, YamlFile yaml) => new(Step.Parse("Opcode read", context, yaml.OpcodeRead));
}