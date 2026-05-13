namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

/// <summary>
/// Carries the immutable generator model together with the per-file state accumulated while emitting syntax.
/// </summary>
public sealed class FileGeneratorContext
{
    /// <summary>
    /// Initializes a new file generation context for the supplied immutable model context.
    /// </summary>
    public FileGeneratorContext(GeneratorContext generatorContext)
    {
        GeneratorContext = generatorContext;
    }

    /// <summary>
    /// Gets the immutable generator model context shared across generated files.
    /// </summary>
    public GeneratorContext GeneratorContext { get; }

    /// <summary>
    /// Gets the namespaces required by the current generated file.
    /// </summary>
    internal RequiredUsings RequiredUsings { get; } = new();

    [Pure]
    public static implicit operator GeneratorContext(FileGeneratorContext context) => context.GeneratorContext;
}