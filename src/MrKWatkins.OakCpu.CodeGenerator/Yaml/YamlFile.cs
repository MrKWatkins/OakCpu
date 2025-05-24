using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class YamlFile
{
    private IReadOnlyList<RegisterYaml>? registers;
    private IReadOnlyList<FlagYaml>? flags;

    private YamlFile()
    {
    }

    public IReadOnlyList<RegisterYaml> Registers
    {
        get => registers ?? [];
        private set => registers = value;
    }

    public IReadOnlyList<FlagYaml> Flags
    {
        get => flags ?? [];
        private set => flags = value;
    }

    [Pure]
    public static YamlFile Combine(params IEnumerable<YamlFile> files)
    {
        var registers = new List<RegisterYaml>();
        var flags = new List<FlagYaml>();
        foreach (var file in files)
        {
            registers.AddRange(file.Registers);
            flags.AddRange(file.Flags);
        }
        return new YamlFile { Registers = registers, Flags = flags };
    }
}