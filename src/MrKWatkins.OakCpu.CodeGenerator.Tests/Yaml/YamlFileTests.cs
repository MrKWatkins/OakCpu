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
}