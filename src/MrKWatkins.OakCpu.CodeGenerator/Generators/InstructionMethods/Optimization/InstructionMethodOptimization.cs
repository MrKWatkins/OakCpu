using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.InstructionMethods.Optimization;

// Peephole optimizations run over the fully assembled statement list of an instruction emulator method. The
// instruction emulator concatenates the statements for every step of an instruction into a single method, which
// creates cross-step patterns that do not exist in the step emulator, e.g. a byte write to half of a register pair
// immediately followed by a 16-bit read of the pair, or a program counter increment for an overlapped opcode read
// immediately rolled back again. The step emulator suspends between steps so its generated code is left untouched.
internal abstract class InstructionMethodOptimization
{
    private static readonly IReadOnlyList<InstructionMethodOptimization> All =
    [
        new RemoveCancellingAdjustments(),
        new ComposePairReadsAfterHalfWrites()
    ];

    [MustUseReturnValue]
    internal static IReadOnlyList<StatementSyntax> PerformAllOptimizations(FileGeneratorContext context, IReadOnlyList<StatementSyntax> statements) =>
        All.Aggregate(statements, (current, optimization) => optimization.Optimize(context, current));

    [MustUseReturnValue]
    protected abstract IReadOnlyList<StatementSyntax> Optimize(FileGeneratorContext context, IReadOnlyList<StatementSyntax> statements);
}