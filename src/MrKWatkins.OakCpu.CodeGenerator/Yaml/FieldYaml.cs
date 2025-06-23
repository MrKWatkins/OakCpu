using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class FieldYaml
{
    private FieldYaml()
    {
    }

    public string Name { get; private set; } = null!;

    public DataType Type { get; private set; }

    public bool Getter { get; private set; }

    public bool Setter { get; private set; }

    public override string ToString() => $"{Name}: {Type}";
}