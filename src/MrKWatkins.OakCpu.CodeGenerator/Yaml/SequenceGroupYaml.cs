using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class SequenceGroupYaml
{
    private SequenceGroupYaml()
    {
    }

    public string Name { get; private set; } = null!;

    public byte Number { get; private set; }
}
