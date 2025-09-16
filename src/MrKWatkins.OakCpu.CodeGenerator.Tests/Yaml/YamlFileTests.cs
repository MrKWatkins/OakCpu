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
    public void Serialize_ValidYamlFileWithMinimalProperties()
    {
        var originalYaml = """
                           cpu:
                             name: TestCpu
                           interrupts:
                             handle: test_handler
                           """;

        var yamlFile = YamlSerializer.Deserialize<YamlFile>(System.Text.Encoding.UTF8.GetBytes(originalYaml), YamlOptions.Instance);
        var serializedBytes = YamlSerializer.Serialize(yamlFile, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);

        // Verify that serialization works and produces valid YAML output
        (serializedYaml.Length > 0).Should().BeTrue();
        serializedYaml.Contains("name: TestCpu", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("handle: test_handler", StringComparison.Ordinal).Should().BeTrue();
    }
}