using VYaml.Annotations;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

internal static class YamlOptions
{
    public static readonly YamlSerializerOptions Instance = CreateOptions();

    [Pure]
    private static YamlSerializerOptions CreateOptions()
    {
        var options = YamlSerializerOptions.Standard;
        options.NamingConvention = NamingConvention.SnakeCase;
        return options;
    }
}
