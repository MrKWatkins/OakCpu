using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class FunctionYaml
{
    private IReadOnlyList<FunctionArgumentYaml>? parameters;

    private FunctionYaml()
    {
    }

    public string Name { get; private set; } = null!;

    public string Type { get; private set; } = null!;

    public string Expression { get; private set; } = null!;

    public IReadOnlyList<FunctionArgumentYaml> Parameters
    {
        get => parameters ?? [];
        private set => parameters = value;
    }

    public override string ToString() => $"{Type} {Name}({string.Join(", ", Parameters)})";
}