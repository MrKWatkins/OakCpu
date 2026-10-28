using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.InstructionMethods;

internal sealed record InstructionMethodPlan(
    string MethodName,
    string Comment,
    StepSequence Sequence,
    IReadOnlyList<InstructionStepPlan> Steps,
    IReadOnlyList<StatementSyntax> OverlapStatements,
    StepSequence? DeferredNextSequence,
    bool CompletesInstructionImplicitly);