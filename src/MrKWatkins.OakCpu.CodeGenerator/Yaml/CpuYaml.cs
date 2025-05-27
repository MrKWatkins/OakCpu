using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class CpuYaml
{
    private IReadOnlyList<string>? actions;
    private IReadOnlyList<IReadOnlyList<string>>? opcodeRead;

    private CpuYaml()
    {
    }

    public string Name { get; private set; } = null!;

    public IReadOnlyList<string> Actions
    {
        get => actions ?? [];
        private set => actions = value;
    }

    public IReadOnlyList<IReadOnlyList<string>> OpcodeRead
    {
        get => opcodeRead ?? [];
        private set => opcodeRead = value;
    }

    public override string ToString() => Name;
}