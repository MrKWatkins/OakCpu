using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Optimization;

// Peephole optimizations run over the fully assembled statement list of a generated method body.
//
// The instruction emulator concatenates the statements for every step of an instruction into a single method, which
// creates cross-step patterns that do not exist per step, e.g. a byte write to half of a register pair immediately
// followed by a 16-bit read of the pair, or a program counter increment for an overlapped opcode read immediately
// rolled back again. It also allows registers to be cached in locals for the duration of the instruction because
// only the bus handler can observe the fields mid-method, and the emulator contract does not require internal
// register state to be accurate during OnActionRequired.
//
// Step emulator methods execute a single step and can only use the optimizations that do not need state to survive
// beyond the method, as the emulator suspends between steps with all state in fields.
internal abstract class MethodBodyOptimization
{
    private static readonly IReadOnlyList<MethodBodyOptimization> InstructionMethodOptimizations =
    [
        new RemoveCancellingAdjustments(),
        new DeferRegisterStores(),
        new ComposePairReadsAfterHalfWrites()
    ];

    private static readonly IReadOnlyList<MethodBodyOptimization> StepMethodOptimizations =
    [
        new RemoveCancellingAdjustments(),
        new ComposePairReadsAfterHalfWrites()
    ];

    [MustUseReturnValue]
    internal static IReadOnlyList<StatementSyntax> PerformInstructionMethodOptimizations(FileGeneratorContext context, IReadOnlyList<StatementSyntax> statements) =>
        Perform(InstructionMethodOptimizations, context, statements);

    [MustUseReturnValue]
    internal static IReadOnlyList<StatementSyntax> PerformStepMethodOptimizations(FileGeneratorContext context, IReadOnlyList<StatementSyntax> statements) =>
        Perform(StepMethodOptimizations, context, statements);

    [MustUseReturnValue]
    private static IReadOnlyList<StatementSyntax> Perform(IReadOnlyList<MethodBodyOptimization> optimizations, FileGeneratorContext context, IReadOnlyList<StatementSyntax> statements) =>
        optimizations.Aggregate(statements, (current, optimization) => optimization.Optimize(context, current));

    [MustUseReturnValue]
    protected abstract IReadOnlyList<StatementSyntax> Optimize(FileGeneratorContext context, IReadOnlyList<StatementSyntax> statements);
}