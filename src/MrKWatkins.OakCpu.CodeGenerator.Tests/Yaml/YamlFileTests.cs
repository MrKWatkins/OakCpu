using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public sealed class YamlFileTests : TestFixture
{
    [TestCaseSource(nameof(YamlFileTestCases))]
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public void Deserialize(string path)
    {
        var bytes = File.ReadAllBytes(path);

        AssertThat.Invoking(() => _ = YamlSerializer.Deserialize<YamlFile>(bytes, YamlOptions.Instance)).Should().NotThrow();
    }

    [Pure]
    public static IEnumerable<TestCaseData> YamlFileTestCases() =>
        new DirectoryInfo(Z80DefinitionsDirectory)
            .EnumerateFiles("*.yaml")
            .Select(file => new TestCaseData(file.FullName).SetArgDisplayNames(file.Name));

    [Test]
    public void Deserialize_ValidYamlFileWithMinimalProperties()
    {
        var yaml = """
                   cpu:
                     name: TestCpu
                   interrupts:
                     handle: test_handler
                   """;

        var yamlFile = YamlSerializer.Deserialize<YamlFile>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        // Verify deserialized properties
        yamlFile.Cpu.Should().NotBeNull();
        yamlFile.Cpu.Name.Should().Equal("TestCpu");
        yamlFile.Interrupts.Should().NotBeNull();
        yamlFile.Interrupts.Handle.Should().Equal("test_handler");
        yamlFile.Registers.Should().BeEmpty();
        yamlFile.Flags.Should().BeEmpty();
        yamlFile.Instructions.Should().BeEmpty();

        // Verify round-trip serialization
        var serializedBytes = YamlSerializer.Serialize(yamlFile, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);
        serializedYaml.Contains("name: TestCpu", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("handle: test_handler", StringComparison.Ordinal).Should().BeTrue();
    }
}