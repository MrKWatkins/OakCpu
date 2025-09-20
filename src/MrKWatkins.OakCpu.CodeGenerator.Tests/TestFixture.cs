using MrKWatkins.OakCpu.CodeGenerator.Generators;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public abstract class TestFixture
{
    private static readonly Lazy<YamlFile> LazyZ80Yaml = new(() => LoadZ80YamlAsync().Result);
    private static readonly Lazy<GeneratorContext> LazyZ80GeneratorInput = new(() => GeneratorContext.Create("MrKWatkins.OakCpu.Z80", Z80Yaml));

    protected static YamlFile Z80Yaml => LazyZ80Yaml.Value;

    protected static GeneratorContext Z80GeneratorContext => LazyZ80GeneratorInput.Value;

    [Pure]
    protected static Task<YamlFile> LoadZ80DefinitionFileAsync(string name)
    {
        var file = Path.Combine(Z80DefinitionsDirectory, name);
        return DeserializeYamlAsync<YamlFile>(file);
    }

    [Pure]
    protected static async Task<YamlFile> LoadZ80YamlAsync()
    {
        var yamls = new List<YamlFile>();
        foreach (var file in Directory.GetFiles(Z80DefinitionsDirectory, "*.yaml", SearchOption.AllDirectories))
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
    protected static string Z80DefinitionsDirectory
    {
        get
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

            return Path.Combine(directory.FullName, "definitions", "z80");
        }
    }
}