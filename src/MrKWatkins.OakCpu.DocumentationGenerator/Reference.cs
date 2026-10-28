namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// A link to external documentation for a CPU's instructions.
/// </summary>
/// <param name="Name">The name of the documentation.</param>
/// <param name="Url">The URL of the documentation.</param>
/// <param name="Covers">Which instructions the documentation covers.</param>
public sealed record Reference(string Name, Uri Url, ReferenceCoverage Covers);