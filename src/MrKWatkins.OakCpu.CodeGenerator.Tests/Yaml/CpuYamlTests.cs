using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public class CpuYamlTests : TestFixture
{
    [Test]
    public async Task Load()
    {
        var yaml = await LoadZ80DefinitionFileAsync("cpu.yaml");
        yaml.Cpu.Should().NotBeNull();
        yaml.OpcodeRead.Should().HaveCount(4);
    }

    [Test]
    public void Serialize_ValidCpuWithMinimalProperties()
    {
        var originalYaml = """
                           name: Z80
                           """;

        var cpu = YamlSerializer.Deserialize<CpuYaml>(System.Text.Encoding.UTF8.GetBytes(originalYaml), YamlOptions.Instance);
        var serializedBytes = YamlSerializer.Serialize(cpu, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);

        // Verify that serialization works and produces valid YAML output
        (serializedYaml.Length > 0).Should().BeTrue();
        serializedYaml.Contains("name: Z80", StringComparison.Ordinal).Should().BeTrue();
    }
}