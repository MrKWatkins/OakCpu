using MrKWatkins.OakCpu.SourceGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.SourceGenerator.Tests.Yaml;

public class RegisterYamlTests : YamlTextFixture
{
    [Test]
    public async Task Load()
    {
        await using var registersYaml = LoadTestData("registers.yaml");
        var yaml = await YamlSerializer.DeserializeAsync<YamlFile>(registersYaml);
    }
}