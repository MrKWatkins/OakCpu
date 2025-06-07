using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public sealed class YamlFileTests : TestFixture
{
    [TestCaseSource(nameof(YamlFileTestCases))]
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public async Task Deserialize(string path)
    {
        await using var stream = File.OpenRead(path);

        await FluentActions.Invoking(async () => await YamlSerializer.DeserializeAsync<YamlFile>(stream, YamlOptions.Instance)).Should().NotThrowAsync();
    }

    [Pure]
    public static IEnumerable<TestCaseData> YamlFileTestCases() =>
        new DirectoryInfo(Z80DefinitionsDirectory)
            .EnumerateFiles("*.yaml")
            .Select(file => new TestCaseData(file.FullName).SetArgDisplayNames(file.Name));
}