using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public class RegisterYamlTests : TextFixture
{
    [Test]
    public async Task Load()
    {
        await using var registersYaml = GetTestDataStream("registers.yaml");
        var yaml = await YamlSerializer.DeserializeAsync<YamlFile>(registersYaml);
    }
}