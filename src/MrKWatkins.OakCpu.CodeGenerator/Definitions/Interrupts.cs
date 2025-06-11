using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Interrupts
{
    private Interrupts(IReadOnlyList<string> properties)
    {
        Properties = properties;
    }

    public IReadOnlyList<string> Properties { get; }

    [Pure]
    public static Interrupts Create(InterruptsYaml yaml) => new(yaml.Properties.Select(p => p.Name).ToList());
}