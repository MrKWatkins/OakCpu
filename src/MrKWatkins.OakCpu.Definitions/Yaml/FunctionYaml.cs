using VYaml.Annotations;

namespace MrKWatkins.OakCpu.Definitions.Yaml;

[YamlObject]
public sealed partial class FunctionYaml
{
    private FunctionYaml()
    {
    }

    public string Name { get; private init; } = null!;

    // TODO: VYaml bug - if this is of the type DataType then the Yaml parser fails.
    public string Type { get; private init; } = null!;

    public IReadOnlyList<string> Parameters
    {
        get => field ?? [];
        private init;
    }

    public string Expression { get; private init; } = null!;

    public override string ToString() => $"{Type} {Name}({string.Join(", ", Parameters)})";
}