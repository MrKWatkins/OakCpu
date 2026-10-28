using VYaml.Annotations;

namespace MrKWatkins.OakCpu.Definitions.Yaml;

[YamlObject]
public sealed partial class RegisterYaml
{
    private RegisterYaml()
    {
    }

    public string Name { get; private init; } = null!;

    public DataType Type { get; private init; }

    public string? Documentation { get; private init; }

    public bool Flags { get; private init; }

    public bool ProgramCounter { get; private init; }

    public bool Transient { get; private init; }

    public string? Category { get; private init; }

    public RegisterYaml? High { get; private init; }

    public RegisterYaml? Low { get; private init; }

    public override string ToString() => $"{Name}: {Type}";
}