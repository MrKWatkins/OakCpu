using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.Definitions;

/// <summary>
/// A link to external documentation for a CPU's instructions.
/// </summary>
/// <param name="Name">The name of the documentation.</param>
/// <param name="Url">The URL of the documentation.</param>
/// <param name="Covers">Which instructions the documentation covers.</param>
public sealed record Reference(string Name, Uri Url, ReferenceCoverage Covers)
{
    [Pure]
    internal static IReadOnlyList<Reference> Create(IReadOnlyList<ReferenceYaml> yamls) =>
        yamls.Select(yaml => new Reference(yaml.Name, new Uri(yaml.Url, UriKind.Absolute), yaml.Covers)).ToList();
}