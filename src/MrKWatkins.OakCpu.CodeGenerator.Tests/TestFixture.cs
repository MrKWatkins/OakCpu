using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public abstract class TestFixture
{
    private static readonly Lazy<YamlFile> LazyZ80Yaml = new(() => LoadZ80Yaml().Result);
    private static readonly Lazy<GeneratorInput> LazyZ80GeneratorInput = new(() => GeneratorInput.Create("MrKWatkins.OakCpu.Z80", Z80Yaml));

    protected static YamlFile Z80Yaml => LazyZ80Yaml.Value;

    protected static GeneratorInput Z80GeneratorInput => LazyZ80GeneratorInput.Value;

    [Pure]
    protected static async Task<YamlFile> LoadZ80DefinitionFileAsync(string name)
    {
        var file = Path.Combine(Z80DefinitionsDirectory, name);
        await using var stream = File.OpenRead(file);
        return await YamlSerializer.DeserializeAsync<YamlFile>(stream, YamlOptions.Instance);
    }

    [Pure]
    private static async Task<YamlFile> LoadZ80Yaml()
    {
        var yamls = new List<YamlFile>();
        foreach (var file in Directory.GetFiles(Z80DefinitionsDirectory, "*.yaml"))
        {
            await using var stream = File.OpenRead(file);
            yamls.Add(await YamlSerializer.DeserializeAsync<YamlFile>(stream, YamlOptions.Instance));
        }
        return YamlFile.Combine(yamls);
    }

    [Pure]
    private static string Z80DefinitionsDirectory
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