using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class FunctionYaml
{
    private IReadOnlyList<string>? parameters;

    private FunctionYaml()
    {
    }

    public string Name { get; private set; } = null!;

    // TODO: VYaml bug - if this is of the type DataType then the Yaml parser fails.
    public string Type { get; private set; } = null!;

    public IReadOnlyList<string> Parameters
    {
        get => parameters ?? [];
        private set => parameters = value;
    }

    public string Expression { get; private set; } = null!;

    public override string ToString() => $"{Type} {Name}({string.Join(", ", Parameters)})";
}