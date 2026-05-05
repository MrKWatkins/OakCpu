using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

internal sealed record InstructionMethodPlan(
    string MethodName,
    string Comment,
    StepSequence Sequence,
    IReadOnlyList<InstructionStepPlan> Steps,
    IReadOnlyList<StatementSyntax> OverlapStatements,
    StepSequence? DeferredNextSequence,
    bool CompletesInstructionImplicitly);

internal sealed record InstructionStepPlan(
    Action Action,
    bool RollsBackOpcodeRead,
    string? NextInstructionVariableName,
    IReadOnlyList<StatementSyntax> StepStatements,
    bool RequiresBlock);