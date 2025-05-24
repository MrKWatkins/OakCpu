using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class RegisterYaml
{
    private IReadOnlyList<string>? combines;

    private RegisterYaml()
    {
    }

    public string Name { get; private set; } = null!;

    public DataType Type { get; private set; }

    public bool Flags { get; private set; }

    public bool ProgramCounter { get; private set; }

    public string? Category { get; private set; }

    public IReadOnlyList<string> Combines
    {
        get => combines ?? [];
        private set => combines = value;
    }

    public override string ToString() => $"{Name}: {Type}";
}