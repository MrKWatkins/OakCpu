using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Interrupts
{
    private Interrupts(IReadOnlyDictionary<string, UserDefinedDataMember> properties, UserDefinedDataMember mode, UserDefinedDataMember trigger)
    {
        Properties = properties;
        Mode = mode;
        Trigger = trigger;
    }

    public IReadOnlyDictionary<string, UserDefinedDataMember> Properties { get; }

    public UserDefinedDataMember Mode { get; }

    public UserDefinedDataMember Trigger { get; }

    [Pure]
    public static Interrupts Create(Configuration configuration, InterruptsYaml yaml)
    {
        var properties = yaml.Properties.ToDictionary(p => p.Name, p => configuration.UserDefinedDataMembers[p.Name]);

        var mode = yaml.Properties.SingleOrDefault(p => p.Mode) ?? throw new InvalidOperationException("No interrupt mode property.");
        var trigger = yaml.Properties.SingleOrDefault(p => p.Trigger) ?? throw new InvalidOperationException("No interrupt trigger property.");

        return new Interrupts(properties, configuration.UserDefinedDataMembers[mode.Name], configuration.UserDefinedDataMembers[trigger.Name]);
    }
}