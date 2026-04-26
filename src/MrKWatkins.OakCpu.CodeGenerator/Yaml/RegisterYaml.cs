using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class RegisterYaml
{
    private RegisterYaml()
    {
    }

    public string Name { get; private set; } = null!;

    public DataType Type { get; private set; }

    public string? Documentation { get; private set; }

    public bool Flags { get; private set; }

    public bool ProgramCounter { get; private set; }

    public string? Category { get; private set; }

    public RegisterYaml? High { get; private set; }

    public RegisterYaml? Low { get; private set; }

    public override string ToString() => $"{Name}: {Type}";
}