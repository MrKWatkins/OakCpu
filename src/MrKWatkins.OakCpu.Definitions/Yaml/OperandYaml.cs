using VYaml.Annotations;

namespace MrKWatkins.OakCpu.Definitions.Yaml;

/// <summary>
/// An operand that follows an opcode in memory, as it is named in instructions, e.g. <c>n</c> for an immediate byte.
/// </summary>
[YamlObject]
public sealed partial class OperandYaml
{
    private OperandYaml()
    {
    }

    public string Name { get; private init; } = null!;

    /// <summary>
    /// The number of bytes the operand takes.
    /// </summary>
    public int? Bytes { get; private init; }

    [YamlIgnore]
    public int EffectiveBytes => Bytes ?? 1;

    public string? Documentation { get; private init; }

    public override string ToString() => Name;
}