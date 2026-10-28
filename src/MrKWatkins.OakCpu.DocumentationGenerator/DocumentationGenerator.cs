namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Generates documentation for the instructions of a CPU from its definitions: a Markdown page per instruction for people, two contents pages, and
/// one YAML file with everything on the pages as data for tools.
/// </summary>
public static class DocumentationGenerator
{
    /// <summary>
    /// The name of the YAML file holding the documentation for every instruction.
    /// </summary>
    public const string YamlFileName = "instructions.yaml";

    /// <summary>
    /// The name of the contents page listing the instructions by group.
    /// </summary>
    public const string IndexFileName = "index.md";

    /// <summary>
    /// The name of the contents page laying the opcodes out in tables.
    /// </summary>
    public const string OpcodesFileName = "opcodes.md";

    [Pure]
    public static IReadOnlyList<GeneratedDocument> Generate(CpuModel model)
    {
        var documentation = CpuDocumentation.Create(model);

        return documentation
            .Select(instruction => new GeneratedDocument(instruction.Page, MarkdownWriter.Write(instruction)))
            .Prepend(new GeneratedDocument(OpcodesFileName, ContentsWriter.WriteOpcodeTables(model.Cpu.Name, documentation)))
            .Prepend(new GeneratedDocument(IndexFileName, ContentsWriter.WriteIndex(model.Cpu.Name, documentation)))
            .Prepend(new GeneratedDocument(YamlFileName, YamlWriter.Write(model.Cpu.Name, documentation)))
            .ToList();
    }
}