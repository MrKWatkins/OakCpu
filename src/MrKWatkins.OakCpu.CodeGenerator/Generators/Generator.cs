using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class Generator
{
    protected const string ActionRequiredEnumName = "ActionRequired";
    protected const string InstructionActionCallbackParameterName = "onActionRequired";
    protected const string InstructionHandlersFieldName = "Instructions";
    protected const string StepStructName = "Step";
    protected const string OverlapsFieldName = "Overlaps";
    protected const string StepHandlerFieldName = "Handler";
    protected const string StepNextStepFieldName = "NextStep";
    protected const string StepActionRequiredFieldName = "ActionRequired";
    protected const string StepOverlapFieldName = "Overlap";
    protected const string EmulatorParameterName = "emulator";
    protected const string ActionRequiredParameterName = "actionRequired";
    protected const string ErrorMethodName = "Error";
    protected const string CompleteInstructionMethodName = "CompleteInstruction";
    protected const string HandleInterruptsMethodName = "HandleInterrupts";
    protected const string ExecuteOverlapMethodName = "ExecuteOverlap";
    protected const string NextSequenceStepFieldName = "nextSequenceStep";
    protected const string NoNextSequenceStepFieldName = "NoNextSequenceStep";
    private const string OverlapMethodPrefix = "Overlap";
    private const string StepMethodPrefix = "Step";

    private protected Generator()
    {
    }

    [Pure]
    protected static string GetStepMethodName(Step step) =>
        step.MethodIndex != null
            ? $"{StepMethodPrefix}{step.MethodIndex}"
            : throw new InvalidOperationException($"Step {step.Name} does not have a {nameof(step.MethodIndex)}.");

    [Pure]
    protected static string GetOverlapMethodName(GeneratorContext context, Step step) => $"{OverlapMethodPrefix}{context.GetOverlapMethodIndex(step)}";

    [Pure]
    protected static string GetSequenceGroupStepTableFieldName(SequenceGroup group) => $"{group.Name.ToUpperCamelCaseFromSnakeCase()}StepTable";
}