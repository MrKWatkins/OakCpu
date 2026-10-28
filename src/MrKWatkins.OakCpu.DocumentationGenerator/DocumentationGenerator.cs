namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Generates Markdown documentation for the instructions of a CPU from its definitions.
/// </summary>
public static class DocumentationGenerator
{
    /// <summary>
    /// Generates a document for every instruction of the CPU.
    /// </summary>
    [Pure]
    public static IReadOnlyList<GeneratedDocument> Generate(CpuModel model) =>
        CpuDocumentation.Create(model)
            .Select(documentation => new GeneratedDocument(documentation.Path, MarkdownWriter.Write(documentation)))
            .ToList();
}