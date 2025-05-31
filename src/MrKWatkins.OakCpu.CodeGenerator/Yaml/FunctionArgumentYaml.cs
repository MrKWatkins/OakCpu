using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class FunctionArgumentYaml
{
    private FunctionArgumentYaml()
    {
    }

    public string Name { get; private set; } = null!;

    public override string ToString() => Name;
}