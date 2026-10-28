namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// A generated document.
/// </summary>
/// <param name="Path">The relative path of the document, e.g. <c>8-bit-load/ld-r0-rp0-d.md</c>.</param>
/// <param name="Content">The content of the document.</param>
public sealed record GeneratedDocument(string Path, string Content);