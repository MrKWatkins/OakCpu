using VYaml.Annotations;

namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

[YamlObject]
public sealed partial class CpuYaml
{
    private CpuYaml()
    {
    }

    public string Name { get; private init; } = null!;

    /// <summary>
    /// The name of the definitions directory for the CPU this CPU is a variant of, if any. Definitions from the base CPU are inherited;
    /// this CPU's definitions are additions to, or overrides of, them.
    /// </summary>
    public string? Base { get; private init; }

    public string? Documentation { get; private init; }

    public IReadOnlyList<ActionYaml> Actions
    {
        get => field ?? [];
        private init;
    }

    public IReadOnlyList<PinYaml> Pins
    {
        get => field ?? [];
        private init;
    }

    public IReadOnlyList<FieldYaml> Fields
    {
        get => field ?? [];
        private init;
    }

    public IReadOnlyList<string?> OpcodeRead
    {
        get => field ?? [];
        private init;
    }

    public NextOpcodeMode? DefaultNextOpcode { get; private init; }

    [YamlIgnore]
    public NextOpcodeMode EffectiveDefaultNextOpcode => DefaultNextOpcode ?? NextOpcodeMode.Read;

    /// <summary>
    /// Merges a variant CPU into the CPU it is based on. Pins, actions and fields from the variant are appended to those from the base
    /// so that the values generated for the base CPU's members are unchanged.
    /// </summary>
    [Pure]
    internal static CpuYaml Merge(CpuYaml @base, CpuYaml variant) =>
        new()
        {
            Name = variant.Name,
            Base = @base.Base,
            Documentation = variant.Documentation ?? @base.Documentation,
            Actions = [.. @base.Actions, .. variant.Actions],
            Pins = [.. @base.Pins, .. variant.Pins],
            Fields = [.. @base.Fields, .. variant.Fields],
            OpcodeRead = variant.OpcodeRead.Count > 0 ? variant.OpcodeRead : @base.OpcodeRead,
            DefaultNextOpcode = variant.DefaultNextOpcode ?? @base.DefaultNextOpcode
        };

    public override string ToString() => Name;
}