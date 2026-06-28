using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.InstructionMethods;

internal static class InstructionMethodPlanner
{
    private const string NextInstructionVariableNamePrefix = "nextInstruction";

    [MustUseReturnValue]
    internal static InstructionMethodPlan CreatePlan(FileGeneratorContext context, StepSequence sequence, IReadOnlyList<Step> steps, string methodName, string comment)
    {
        var overlapStep = sequence.Steps.FirstOrDefault(step => context.GeneratorContext.GetStepLayout(step).ExecutesAsOverlapOnly);
        var overlapTrailingStatementsToSkip = overlapStep == null ? 0 : context.GeneratorContext.GetImplicitInstructionStepsCompleteStatementCount(overlapStep);
        var overlapStatements = overlapStep == null ? [] : StatementGenerator.GenerateOverlapStatements(context, overlapStep, overlapTrailingStatementsToSkip).ToArray();
        var completesInstructionImplicitly = overlapStep != null
            ? overlapTrailingStatementsToSkip != 0
            : steps.Count != 0 && context.GeneratorContext.GetImplicitInstructionStepsCompleteStatementCount(steps[^1]) != 0;
        var deferredNextSequence = GetDeferredNextSequence(context, sequence);

        var stepPlans = steps
            .Select((step, index) => CreateStepPlan(context, step, overlapStep, index))
            .ToList();
        var localDeclarationCounts = GetLocalDeclarationCounts(stepPlans);
        stepPlans = stepPlans
            .Select(stepPlan => stepPlan with { RequiresBlock = RequiresStepBlock(stepPlan.StepStatements, localDeclarationCounts) })
            .ToList();

        return new InstructionMethodPlan(methodName, comment, sequence, stepPlans, overlapStatements, deferredNextSequence, completesInstructionImplicitly);
    }

    [Pure]
    private static StepSequence? GetDeferredNextSequence(GeneratorContext context, StepSequence sequence) =>
        sequence.NextOpcode switch
        {
            NextOpcodeMode.Loop => sequence,
            NextOpcodeMode.Overlapped when sequence.OverlappedSequenceName is { } overlappedSequenceName => context.GetSequence(overlappedSequenceName),
            _ => null
        };

    [Pure]
    private static bool ContainsRedirectCall(Step step) =>
        ContainsCall(step.Statements, PreDefinedFunction.MoveToInterruptMode) ||
        ContainsCall(step.Statements, PreDefinedFunction.MoveToOpcode) ||
        ContainsCall(step.Statements, PreDefinedFunction.MoveToSequence) ||
        ContainsCall(step.Statements, PreDefinedFunction.MoveToSequenceGroup);

    [Pure]
    private static bool ContainsCall(IEnumerable<Statement> statements, PreDefinedFunction function) => statements.Any(statement => ContainsCall(statement, function));

    [Pure]
    private static bool ContainsCall(AstNode node, PreDefinedFunction function)
    {
        if (node is Call call && call.Function == function)
        {
            return true;
        }

        return node.Children.Any(child => ContainsCall(child, function));
    }

    [Pure]
    private static bool ShouldRollbackOpcodeRead(Step step, Action action) =>
        action.Name == "opcode_read" && ContainsCurrentStepAssignment(step.Statements, 1);

    [Pure]
    private static bool ContainsCurrentStepAssignment(IEnumerable<Statement> statements, int value) =>
        statements.Any(statement => ContainsCurrentStepAssignment(statement, value));

    [Pure]
    private static bool ContainsCurrentStepAssignment(AstNode node, int value) =>
        node is Assignment
        {
            Target: DataMemberAccess { DataMember: var dataMember },
            Value: Number { Value: var assignmentValue }
        } && dataMember == PreDefinedDataMember.CurrentStep && assignmentValue == value ||
        node.Children.Any(child => ContainsCurrentStepAssignment(child, value));

    [MustUseReturnValue]
    private static InstructionStepPlan CreateStepPlan(FileGeneratorContext context, Step step, Step? instructionExitOverlapStep, int instructionTStatesBeforeStep)
    {
        var stepLayout = context.GeneratorContext.GetStepLayout(step);
        var action = StepMetadata.GetAction(context, step);
        var containsRedirect = ContainsRedirectCall(step);
        var isMoveToOpcode = ContainsCall(step.Statements, PreDefinedFunction.MoveToOpcode);
        var rollsBackOpcodeRead = ShouldRollbackOpcodeRead(step, action);
        var nextInstructionVariableName = containsRedirect ? $"{NextInstructionVariableNamePrefix}{stepLayout.Index}" : null;
        var trailingStatementsToSkip = context.GeneratorContext.GetImplicitInstructionStepsCompleteStatementCount(step);
        var requiresBody = !stepLayout.DoesNothing || stepLayout.QueuesOverlapStep || containsRedirect || ContainsCall(step.Statements, PreDefinedFunction.HandleInterrupts) || ContainsCall(step.Statements, PreDefinedFunction.InstructionComplete);
        var stepStatements = requiresBody
            ? StatementGenerator.GenerateInstructionStatements(context, step, nextInstructionVariableName, instructionExitOverlapStep, instructionTStatesBeforeStep, trailingStatementsToSkip).ToList()
            : [];

        return new InstructionStepPlan(action, rollsBackOpcodeRead, isMoveToOpcode, nextInstructionVariableName, stepStatements, false);
    }

    [Pure]
    private static IReadOnlyDictionary<string, int> GetLocalDeclarationCounts(IEnumerable<InstructionStepPlan> stepPlans)
    {
        var declarationNames = stepPlans
            .SelectMany(stepPlan => stepPlan.StepStatements)
            .SelectMany(statement => statement.DescendantNodesAndSelf().OfType<LocalDeclarationStatementSyntax>())
            .SelectMany(statement => statement.Declaration.Variables.Select(variable => variable.Identifier.ValueText));

        return declarationNames
            .GroupBy(name => name)
            .ToDictionary(group => group.Key, group => group.Count());
    }

    [Pure]
    private static bool RequiresStepBlock(IEnumerable<StatementSyntax> stepStatements, IReadOnlyDictionary<string, int> localDeclarationCounts) =>
        stepStatements
            .SelectMany(statement => statement.DescendantNodesAndSelf().OfType<LocalDeclarationStatementSyntax>())
            .SelectMany(statement => statement.Declaration.Variables.Select(variable => variable.Identifier.ValueText))
            .Any(name => localDeclarationCounts.TryGetValue(name, out var count) && count > 1);
}