using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public class RegisterYamlTests : TestFixture
{
    [Test]
    public async Task Load()
    {
        var registersYaml = await LoadZ80DefinitionFileAsync("registers.yaml");
        registersYaml.Registers.Should().HaveCount(15);
    }

    [Test]
    public void Serialize_ValidRegisterWithAllProperties()
    {
        var originalYaml = """
                           name: AF
                           type: u16
                           high:
                             name: A
                             type: u8
                           low:
                             name: F
                             type: u8
                             flags: true
                           """;

        var register = YamlSerializer.Deserialize<RegisterYaml>(System.Text.Encoding.UTF8.GetBytes(originalYaml), YamlOptions.Instance);
        var serializedBytes = YamlSerializer.Serialize(register, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);

        // Verify that serialization works and produces valid YAML output
        (serializedYaml.Length > 0).Should().BeTrue();
        serializedYaml.Contains("name: AF", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("type: u16", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("name: A", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("name: F", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("flags: true", StringComparison.Ordinal).Should().BeTrue();
    }

    [Test]
    public void Serialize_ValidRegisterWithMinimalProperties()
    {
        var originalYaml = """
                           name: B
                           type: u8
                           """;

        var register = YamlSerializer.Deserialize<RegisterYaml>(System.Text.Encoding.UTF8.GetBytes(originalYaml), YamlOptions.Instance);
        var serializedBytes = YamlSerializer.Serialize(register, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);

        // Verify that serialization works and produces valid YAML output
        (serializedYaml.Length > 0).Should().BeTrue();
        serializedYaml.Contains("name: B", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("type: u8", StringComparison.Ordinal).Should().BeTrue();
    }
}