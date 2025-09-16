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
    public void Deserialize_ValidCpuWithMinimalProperties()
    {
        var yaml = """
                   name: Z80
                   """;

        var cpu = YamlSerializer.Deserialize<CpuYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        // Verify deserialized properties
        cpu.Name.Should().Equal("Z80");
        cpu.Actions.Should().BeEmpty();
        cpu.Fields.Should().BeEmpty();
        cpu.OpcodeRead.Should().BeEmpty();

        // Verify round-trip serialization
        var serializedBytes = YamlSerializer.Serialize(cpu, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);
        serializedYaml.Contains("name: Z80", StringComparison.Ordinal).Should().BeTrue();
    }
}