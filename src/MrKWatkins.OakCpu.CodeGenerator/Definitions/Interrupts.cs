using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Interrupts
{
    private Interrupts(IReadOnlyDictionary<string, UserDefinedDataMember> properties, IReadOnlyList<Statement> handle, HaltedCycle haltedCycle, IReadOnlyList<InterruptMode> modes)
    {
        Properties = properties;
        Handle = handle;
        HaltedCycle = haltedCycle;
        Modes = modes;
    }

    public IReadOnlyDictionary<string, UserDefinedDataMember> Properties { get; }

    public IReadOnlyList<Statement> Handle { get; }

    public HaltedCycle HaltedCycle { get; }

    public IReadOnlyList<InterruptMode> Modes { get; }

    public IEnumerable<Step> AllSteps => HaltedCycle.Concat(Modes.SelectMany(m => m.Steps));

    [Pure]
    public static Interrupts Create(ParserContext context, InterruptsYaml yaml)
    {
        var properties = yaml.Properties.ToDictionary(p => p.Name, p => context.Configuration.UserDefinedDataMembers[p.Name]);

        var handle = Parser.ParseStatements(context, yaml.Handle);

        var haltedCycle = HaltedCycle.Create(context, yaml);

        var modes = yaml.Modes.Select(mode => InterruptMode.Create(context, mode)).ToList();

        return new Interrupts(properties, handle, haltedCycle, modes);
    }
}