using Microsoft.CodeAnalysis.CSharp.Syntax;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.InstructionMethods;

internal sealed record InstructionStepPlan(
    Action Action,
    bool RollsBackOpcodeRead,
    bool IsMoveToOpcode,
    string? NextInstructionVariableName,
    IReadOnlyList<StatementSyntax> StepStatements,
    bool RequiresBlock);