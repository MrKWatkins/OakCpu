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

    /// <summary>
    /// Describes the function for documentation, with its parameters in braces, e.g. <c>bit 7 of {value} is set</c>. For a
    /// boolean function it is the condition the function tests; otherwise it is the value the function returns.
    /// </summary>
    public string? Documentation { get; private init; }

    /// <summary>
    /// Describes the condition a boolean function tests when it does not hold, e.g. <c>bit 7 of {value} is reset</c>.
    /// </summary>
    public string? NotDocumentation { get; private init; }

    /// <summary>
    /// Describes the value of a boolean function when it is used as a number, e.g. <c>the half carry</c>.
    /// </summary>
    public string? ValueDocumentation { get; private init; }

    public override string ToString() => $"{Type} {Name}({string.Join(", ", Parameters)})";
}