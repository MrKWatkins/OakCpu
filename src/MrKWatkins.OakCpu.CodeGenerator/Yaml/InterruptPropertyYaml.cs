using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class InterruptPropertyYaml
{
    private InterruptPropertyYaml()
    {
    }

    public string Name { get; private set; } = null!;

    public DataType Type { get; private set; }

    public bool Mode { get; private set; }

    public bool Trigger { get; private set; }

    public override string ToString() => $"{Name}: {Type}{(Mode ? " mode": "")}{(Trigger ? " trigger": "")}";
}