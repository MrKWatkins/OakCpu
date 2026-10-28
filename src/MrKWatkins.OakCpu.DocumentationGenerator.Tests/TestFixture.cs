using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.DocumentationGenerator.Tests;

public abstract class TestFixture
{
    private static readonly Lazy<CpuModel> LazyZ80Model = new(() => CpuModel.LoadAsync(new DirectoryInfo(GetDefinitionsDirectory("z80"))).Result);
    private static readonly Lazy<CpuModel> LazyZ80NModel = new(() => CpuModel.LoadAsync(new DirectoryInfo(GetDefinitionsDirectory("z80n"))).Result);
    private static readonly Lazy<CpuModel> LazyM6502Model = new(() => CpuModel.LoadAsync(new DirectoryInfo(GetDefinitionsDirectory("6502"))).Result);
    private static readonly Lazy<IReadOnlyList<InstructionDocumentation>> LazyZ80Documentation = new(() => CpuDocumentation.Create(Z80Model));
    private static readonly Lazy<IReadOnlyList<InstructionDocumentation>> LazyZ80NDocumentation = new(() => CpuDocumentation.Create(Z80NModel));
    private static readonly Lazy<IReadOnlyList<InstructionDocumentation>> LazyM6502Documentation = new(() => CpuDocumentation.Create(M6502Model));

    protected static CpuModel Z80Model => LazyZ80Model.Value;

    protected static CpuModel Z80NModel => LazyZ80NModel.Value;

    /// <summary>
    /// The 6502 model; named after the identifier its definitions use for the CPU.
    /// </summary>
    protected static CpuModel M6502Model => LazyM6502Model.Value;

    protected static IReadOnlyList<InstructionDocumentation> Z80Documentation => LazyZ80Documentation.Value;

    protected static IReadOnlyList<InstructionDocumentation> Z80NDocumentation => LazyZ80NDocumentation.Value;

    protected static IReadOnlyList<InstructionDocumentation> M6502Documentation => LazyM6502Documentation.Value;

    /// <summary>
    /// Gets the instruction with the given mnemonic, preferring an unprefixed opcode when the mnemonic has prefixed duplicates.
    /// </summary>
    [Pure]
    protected static Instruction GetInstruction(CpuModel model, string mnemonic) =>
        model.Instructions
            .Where(instruction => instruction.Mnemonic == mnemonic)
            .OrderBy(instruction => instruction.Prefix.HasValue)
            .ThenBy(instruction => instruction.Prefix)
            .First();

    [Pure]
    protected static Instruction GetInstruction(CpuModel model, byte? prefix, byte opcode, string? table = null) =>
        model.Instructions.Single(instruction =>
            instruction.OpcodeTable == table &&
            (instruction.Prefix == prefix && instruction.Opcode == opcode || instruction.Duplicates.Any(duplicate => duplicate.Prefix == prefix && duplicate.Opcode == opcode)));

    [Pure]
    protected static InstructionYaml GetInstructionYaml(CpuModel model, string form) =>
        model.Yaml.Instructions.First(instruction => instruction.Instruction == form);

    [Pure]
    protected static InstructionDocumentation GetDocumentation(IReadOnlyList<InstructionDocumentation> documentation, string form) =>
        documentation.Single(d => d.Instruction == form);

    [Pure]
    protected static string GetDefinitionsDirectory(string cpu)
    {
        // Assumes the test assembly is in the source directory!
        var testAssembly = new FileInfo(typeof(TestFixture).Assembly.Location);
        var directory = testAssembly.Directory;
        while (directory != null && directory.GetDirectories("definitions").Length == 0)
        {
            directory = directory.Parent;
        }

        if (directory == null)
        {
            throw new InvalidOperationException("Could not find the definitions directory.");
        }

        return Path.Combine(directory.FullName, "definitions", cpu);
    }
}