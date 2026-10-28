using VYaml.Annotations;

namespace MrKWatkins.OakCpu.Definitions.Yaml;

[YamlObject]
public sealed partial class SequenceGroupYaml
{
    private SequenceGroupYaml()
    {
    }

    public string Name { get; private init; } = null!;

    public byte Number { get; private init; }
}