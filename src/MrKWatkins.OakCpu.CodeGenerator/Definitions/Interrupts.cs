using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Interrupts
{
    private Interrupts(IReadOnlyDictionary<string, UserDefinedDataMember> properties, IReadOnlyList<Statement> handle, StepSequence halted, IReadOnlyList<InterruptMode> modes)
    {
        Properties = properties;
        Handle = handle;
        Halted = halted;
        Modes = modes;
    }

    public IReadOnlyDictionary<string, UserDefinedDataMember> Properties { get; }

    public IReadOnlyList<Statement> Handle { get; }

    public StepSequence Halted { get; }

    public IReadOnlyList<InterruptMode> Modes { get; }

    public IEnumerable<StepSequence> AllSequences => Enumerable.Repeat(Halted, 1).Concat(Modes.Select(m => m.Sequence)).Distinct();

    public IEnumerable<Step> AllSteps => AllSequences.SelectMany(sequence => sequence.Steps);

    [Pure]
    public static Interrupts Create(ParserContext context, InterruptsYaml yaml, IReadOnlyDictionary<string, StepSequence> availableSequences)
    {
        var properties = yaml.Properties.ToDictionary(p => p.Name, p => context.Configuration.UserDefinedDataMembers[p.Name]);

        var handle = Parser.ParseStatements(context, yaml.Handle);

        var halted = yaml.HaltedCycle.Any()
            ? NamedStepSequence.Create(context, "halted", yaml.HaltedCycle, NextOpcodeMode.Loop, true)
            : availableSequences.TryGetValue("halted", out var sequence)
                ? sequence
                : availableSequences.TryGetValue("halted_cycle", out var legacySequence)
                    ? legacySequence
                    : throw new InvalidOperationException("No sequence named halted exists for the halted cycle.");

        var modes = yaml.Modes.Select(mode => InterruptMode.Create(context, mode, availableSequences)).ToList();

        return new Interrupts(properties, handle, halted, modes);
    }
}
