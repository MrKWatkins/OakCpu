using System.Text;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Validation;

internal static class ValidationTestHelper
{
    [Pure]
    public static TYaml Deserialize<TYaml>(string yaml) => YamlSerializer.Deserialize<TYaml>(Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

    [Pure]
    public static YamlFile DeserializeYamlFile(string yaml) => Deserialize<YamlFile>(yaml);
}