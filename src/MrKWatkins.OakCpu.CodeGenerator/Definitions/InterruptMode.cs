using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class InterruptMode
{
    public const string SequenceGroupName = "interrupt_mode";

    private InterruptMode(byte number, StepSequence sequence)
    {
        Number = number;
        Sequence = sequence;
    }

    public byte Number { get; }

    public StepSequence Sequence { get; }

    [Pure]
    public static InterruptMode Create(
        ParserContext context,
        InterruptModeYaml yaml,
        IReadOnlyDictionary<string, StepSequence> availableSequences)
    {
        var sequence = yaml.Sequence switch
        {
            { } name when availableSequences.TryGetValue(name, out var existingSequence) => existingSequence,
            { } name => throw new InvalidOperationException($"No sequence named {name} exists for interrupt mode {yaml.Number}."),
            _ => NamedStepSequence.Create(context, $"interrupt_mode_{yaml.Number}", yaml.Steps, yaml.NextOpcode, true)
        };

        return new InterruptMode(yaml.Number, sequence);
    }
}