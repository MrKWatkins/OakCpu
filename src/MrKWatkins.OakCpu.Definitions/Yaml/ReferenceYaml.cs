using VYaml.Annotations;

namespace MrKWatkins.OakCpu.Definitions.Yaml;

/// <summary>
/// A link to external documentation for a CPU's instructions.
/// </summary>
[YamlObject]
public sealed partial class ReferenceYaml
{
    private ReferenceYaml()
    {
    }

    public string Name { get; private init; } = null!;

    /// <summary>
    /// The URL of the documentation. A string rather than a <see cref="Uri" /> so that the YAML can be deserialized; the model
    /// validates it.
    /// </summary>
    [SuppressMessage("Design", "CA1056: URI-like properties should not be strings", Justification = "Deserialized from YAML.")]
    public string Url { get; private init; } = null!;

    /// <summary>
    /// Which instructions the documentation covers.
    /// </summary>
    public ReferenceCoverage Covers { get; private init; }

    public override string ToString() => Name;
}