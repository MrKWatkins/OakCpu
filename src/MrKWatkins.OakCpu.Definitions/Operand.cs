using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.Definitions;

/// <summary>
/// An operand that follows an opcode in memory, as it is named in instructions, e.g. <c>n</c> for an immediate byte.
/// </summary>
/// <param name="Name">The name of the operand in instructions.</param>
/// <param name="Bytes">The number of bytes the operand takes.</param>
/// <param name="Documentation">A description of the operand.</param>
public sealed record Operand(string Name, int Bytes, Documentation Documentation)
{
    [Pure]
    internal static IReadOnlyList<Operand> Create(IReadOnlyList<OperandYaml> yamls) =>
        yamls.Select(yaml => new Operand(yaml.Name, yaml.EffectiveBytes, Documentation.CreateOptional(yaml.Documentation, $"operand {yaml.Name}"))).ToList();
}