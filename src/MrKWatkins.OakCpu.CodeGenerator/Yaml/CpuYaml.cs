using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class CpuYaml
{
    private IReadOnlyList<ActionYaml>? actions;
    private IReadOnlyList<FieldYaml>? fields;
    private IReadOnlyList<string?>? opcodeRead;

    private CpuYaml()
    {
    }

    public string Name { get; private set; } = null!;

    public string? Documentation { get; private set; }

    public IReadOnlyList<ActionYaml> Actions
    {
        get => actions ?? [];
        private set => actions = value;
    }

    public IReadOnlyList<FieldYaml> Fields
    {
        get => fields ?? [];
        private set => fields = value;
    }

    public IReadOnlyList<string?> OpcodeRead
    {
        get => opcodeRead ?? [];
        private set => opcodeRead = value;
    }

    public NextOpcodeMode? DefaultNextOpcode { get; private set; }

    [YamlIgnore]
    public NextOpcodeMode EffectiveDefaultNextOpcode => DefaultNextOpcode ?? NextOpcodeMode.Read;

    public override string ToString() => Name;
}