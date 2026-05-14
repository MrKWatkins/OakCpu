using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.InstructionMethods;

internal sealed record InstructionMethodPlan(
    string MethodName,
    string Comment,
    StepSequence Sequence,
    IReadOnlyList<InstructionStepPlan> Steps,
    IReadOnlyList<StatementSyntax> OverlapStatements,
    StepSequence? DeferredNextSequence,
    bool CompletesInstructionImplicitly);