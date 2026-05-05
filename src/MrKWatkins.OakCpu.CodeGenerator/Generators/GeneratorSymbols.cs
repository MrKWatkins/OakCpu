using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class GeneratorSymbols
{
    internal const string ActionRequiredEnumName = "ActionRequired";
    internal const string InstructionActionCallbackParameterName = "onActionRequired";
    internal const string InstructionHandlersFieldName = "Instructions";
    internal const string StepStructName = "Step";
    internal const string OverlapsFieldName = "Overlaps";
    internal const string StepHandlerFieldName = "Handler";
    internal const string StepNextStepFieldName = "NextStep";
    internal const string StepActionRequiredFieldName = "ActionRequired";
    internal const string StepOverlapFieldName = "Overlap";
    internal const string EmulatorParameterName = "emulator";
    internal const string ActionRequiredParameterName = "actionRequired";
    internal const string ErrorMethodName = "Error";
    internal const string CompleteInstructionMethodName = "CompleteInstruction";
    internal const string HandleInterruptsMethodName = "HandleInterrupts";
    internal const string ExecuteOverlapMethodName = "ExecuteOverlap";
    internal const string NextSequenceStepFieldName = "nextSequenceStep";
    internal const string NoNextSequenceStepFieldName = "NoNextSequenceStep";

    private const string OverlapMethodPrefix = "Overlap";
    private const string StepMethodPrefix = "Step";

    [Pure]
    internal static string GetStepMethodName(Step step) =>
        step.MethodIndex != null
            ? $"{StepMethodPrefix}{step.MethodIndex}"
            : throw new InvalidOperationException($"Step {step.Name} does not have a {nameof(step.MethodIndex)}.");

    [Pure]
    internal static string GetOverlapMethodName(GeneratorContext context, Step step) => $"{OverlapMethodPrefix}{context.GetOverlapMethodIndex(step)}";

    [Pure]
    internal static string GetSequenceGroupStepTableFieldName(SequenceGroup group) => $"{group.Name.ToUpperCamelCaseFromSnakeCase()}StepTable";
}