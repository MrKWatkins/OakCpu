using System.Collections.Frozen;

namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// The external documentation linked from the generated documentation, by CPU name.
/// </summary>
public static class References
{
    private static readonly FrozenDictionary<string, IReadOnlyList<Reference>> ByCpu = new Dictionary<string, IReadOnlyList<Reference>>(StringComparer.OrdinalIgnoreCase)
    {
        ["Z80"] =
        [
            new Reference("Zilog Z80 CPU User Manual", new Uri("https://www.zilog.com/docs/z80/um0080.pdf"), ReferenceCoverage.Documented),
            new Reference("Z80 instruction set, MSX Assembly Page", new Uri("https://map.grauw.nl/resources/z80instr.php"), ReferenceCoverage.All),
            new Reference("The Undocumented Z80 Documented", new Uri("http://www.z80.info/zip/z80-documented.pdf"), ReferenceCoverage.Undocumented)
        ],
        ["Z80N"] =
        [
            new Reference("Extended Z80 instruction set, ZX Spectrum Next wiki", new Uri("https://wiki.specnext.dev/Extended_Z80_instruction_set"), ReferenceCoverage.All)
        ]
    }.ToFrozenDictionary();

    /// <summary>
    /// Gets the references for the given CPU that cover an instruction.
    /// </summary>
    /// <param name="cpu">The name of the CPU that defines the instruction.</param>
    /// <param name="undocumented">Whether the instruction is undocumented.</param>
    [Pure]
    public static IReadOnlyList<Reference> For(string cpu, bool undocumented) =>
        ByCpu.TryGetValue(cpu, out var references)
            ? references.Where(r => r.Covers == ReferenceCoverage.All || r.Covers == (undocumented ? ReferenceCoverage.Undocumented : ReferenceCoverage.Documented)).ToList()
            : [];
}