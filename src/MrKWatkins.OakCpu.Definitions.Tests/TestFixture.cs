using MrKWatkins.OakCpu.Definitions.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.Definitions.Tests;

public abstract class TestFixture
{
    private static readonly Lazy<YamlFile> LazyZ80Yaml = new(() => LoadZ80YamlAsync().Result);
    private static readonly Lazy<CpuModel> LazyZ80Model = new(() => CpuModel.Create(Z80Yaml));
    private static readonly Lazy<YamlFile> LazyZ80NYaml = new(() => YamlFile.Merge(Z80Yaml, CpuModel.LoadDirectoryAsync(new DirectoryInfo(GetDefinitionsDirectory("z80n"))).Result));
    private static readonly Lazy<CpuModel> LazyZ80NModel = new(() => CpuModel.Create(Z80NYaml));

    protected static YamlFile Z80Yaml => LazyZ80Yaml.Value;

    protected static CpuModel Z80Model => LazyZ80Model.Value;

    /// <summary>
    /// The Z80N definitions merged into the Z80 definitions they are based on, i.e. what the generator builds a Z80N from.
    /// </summary>
    protected static YamlFile Z80NYaml => LazyZ80NYaml.Value;

    protected static CpuModel Z80NModel => LazyZ80NModel.Value;

    [Pure]
    protected static Task<YamlFile> LoadZ80DefinitionFileAsync(string name)
    {
        var file = Path.Combine(Z80DefinitionsDirectory, name);
        return DeserializeYamlAsync<YamlFile>(file);
    }

    [Pure]
    protected static Task<YamlFile> LoadZ80YamlAsync() => CpuModel.LoadDirectoryAsync(new DirectoryInfo(Z80DefinitionsDirectory));

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
    protected static string GetDefinitionsDirectory(string cpu)
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