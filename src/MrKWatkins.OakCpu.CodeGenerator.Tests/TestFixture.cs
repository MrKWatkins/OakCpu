using MrKWatkins.OakCpu.CodeGenerator.Generators;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public abstract class TestFixture
{
    private static readonly Lazy<YamlFile> LazyZ80Yaml = new(() => LoadZ80YamlAsync().Result);
    private static readonly Lazy<GeneratorContext> LazyZ80GeneratorInput = new(() => GeneratorContext.Create("MrKWatkins.OakCpu.Z80", Z80Yaml));
    private static readonly Lazy<YamlFile> LazyZ80NYaml = new(() => YamlFile.Merge(Z80Yaml, LoadYamlAsync(GetDefinitionsDirectory("z80n")).Result));
    private static readonly Lazy<GeneratorContext> LazyZ80NGeneratorInput = new(() => GeneratorContext.Create("MrKWatkins.OakCpu.Z80N", Z80NYaml));

    protected static YamlFile Z80Yaml => LazyZ80Yaml.Value;

    protected static GeneratorContext Z80GeneratorContext => LazyZ80GeneratorInput.Value;

    /// <summary>
    /// The Z80N definitions merged into the Z80 definitions they are based on, i.e. what the generator builds a Z80N from.
    /// </summary>
    protected static YamlFile Z80NYaml => LazyZ80NYaml.Value;

    protected static GeneratorContext Z80NGeneratorContext => LazyZ80NGeneratorInput.Value;

    [Pure]
    protected static FileGeneratorContext CreateZ80FileGeneratorContext() => Z80GeneratorContext.CreateFileContext();

    [Pure]
    protected static FileGeneratorContext CreateZ80NFileGeneratorContext() => Z80NGeneratorContext.CreateFileContext();

    [Pure]
    protected static Task<YamlFile> LoadZ80DefinitionFileAsync(string name)
    {
        var file = Path.Combine(Z80DefinitionsDirectory, name);
        return DeserializeYamlAsync<YamlFile>(file);
    }

    [Pure]
    protected static Task<YamlFile> LoadZ80YamlAsync() => LoadYamlAsync(Z80DefinitionsDirectory);

    [Pure]
    private static async Task<YamlFile> LoadYamlAsync(string definitionsDirectory)
    {
        var yamls = new List<YamlFile>();
        foreach (var file in Directory.GetFiles(definitionsDirectory, "*.yaml", SearchOption.AllDirectories))
        {
            yamls.Add(await DeserializeYamlAsync<YamlFile>(file));
        }
        return YamlFile.Combine(yamls);
    }

    private static async Task<TYaml> DeserializeYamlAsync<TYaml>([PathReference] string file)
    {
        try
        {
            await using var stream = File.OpenRead(file);
            return await YamlSerializer.DeserializeAsync<TYaml>(stream, YamlOptions.Instance);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Could not load YAML file {file}.", exception);
        }
    }

    [Pure]
    protected static string Z80DefinitionsDirectory => GetDefinitionsDirectory("z80");

    [Pure]
    private static string GetDefinitionsDirectory(string cpu)
    {
        // Assumes the test assembly is in the source directory!
        var testAssembly = new FileInfo(typeof(TestFixture).Assembly.Location);
        var directory = testAssembly.Directory;
        while (directory != null && directory.GetDirectories("definitions").Length == 0)
        {
            directory = directory.Parent;
        }

        if (directory == null)
        {
            throw new InvalidOperationException("Could not find the definitions directory.");
        }

        return Path.Combine(directory.FullName, "definitions", cpu);
    }
}