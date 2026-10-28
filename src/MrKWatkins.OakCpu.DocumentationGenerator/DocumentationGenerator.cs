namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Generates documentation for the instructions of a CPU from its definitions: a Markdown page per instruction for people, and
/// one YAML file with everything on the pages as data for tools.
/// </summary>
public static class DocumentationGenerator
{
    /// <summary>
    /// The name of the YAML file holding the documentation for every instruction.
    /// </summary>
    public const string YamlFileName = "instructions.yaml";

    [Pure]
    public static IReadOnlyList<GeneratedDocument> Generate(CpuModel model)
    {
        var documentation = CpuDocumentation.Create(model);

        return documentation
            .Select(instruction => new GeneratedDocument(instruction.Page, MarkdownWriter.Write(instruction)))
            .Prepend(new GeneratedDocument(YamlFileName, YamlWriter.Write(model.Cpu.Name, documentation)))
            .ToList();
    }
}