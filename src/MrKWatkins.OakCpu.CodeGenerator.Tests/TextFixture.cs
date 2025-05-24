using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public abstract class TextFixture
{
    [Pure]
    internal static async Task<GeneratorInput> LoadTestDataAsync()
    {
        await using var registersYaml = GetTestDataStream("registers.yaml");
        var yaml = await YamlSerializer.DeserializeAsync<YamlFile>(registersYaml);

        return GeneratorInput.Create("MrKWatkins.TestNamespace", yaml);
    }

    [Pure]
    [MustDisposeResource]
    protected static Stream GetTestDataStream(string name) =>
        typeof(TextFixture).Assembly.GetManifestResourceStream(typeof(TextFixture), $"TestData.{name}")
        ?? throw new InvalidOperationException($"Could not find embedded resource {name}.");
}