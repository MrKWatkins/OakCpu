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
    public void Deserialize_ValidRegisterWithAllProperties()
    {
        var yaml = """
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

        var register = YamlSerializer.Deserialize<RegisterYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        // Verify deserialized properties
        register.Name.Should().Equal("AF");
        register.Type.Should().Equal(DataType.U16);
        register.High.Should().NotBeNull();
        register.High!.Name.Should().Equal("A");
        register.High.Type.Should().Equal(DataType.U8);
        register.Low.Should().NotBeNull();
        register.Low!.Name.Should().Equal("F");
        register.Low.Type.Should().Equal(DataType.U8);
        register.Low.Flags.Should().BeTrue();

        // Verify round-trip serialization
        var serializedBytes = YamlSerializer.Serialize(register, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);
        serializedYaml.Contains("name: AF", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("type: u16", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("name: A", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("name: F", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("flags: true", StringComparison.Ordinal).Should().BeTrue();
    }

    [Test]
    public void Deserialize_ValidRegisterWithMinimalProperties()
    {
        var yaml = """
                   name: B
                   type: u8
                   """;

        var register = YamlSerializer.Deserialize<RegisterYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        // Verify deserialized properties
        register.Name.Should().Equal("B");
        register.Type.Should().Equal(DataType.U8);
        register.High.Should().BeNull();
        register.Low.Should().BeNull();
        register.Flags.Should().BeFalse();

        // Verify round-trip serialization
        var serializedBytes = YamlSerializer.Serialize(register, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);
        serializedYaml.Contains("name: B", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("type: u8", StringComparison.Ordinal).Should().BeTrue();
    }
}