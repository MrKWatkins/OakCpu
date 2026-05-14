using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public sealed class ActionYamlTests
{
    [Test]
    public void Deserialize_ValidAction()
    {
        var yaml = """
                   name: memory_read
                   documentation: Reads a byte from memory.
                   """;

        var action = YamlSerializer.Deserialize<ActionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        action.Name.Should().Equal("memory_read");
        action.Documentation.Should().Equal("Reads a byte from memory.");
        action.ToString().Should().Equal("memory_read");
    }
}