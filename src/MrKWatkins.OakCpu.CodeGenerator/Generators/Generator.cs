using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class Generator
{
    protected const string ActionRequiredEnumName = "ActionRequired";
    protected const string StepStructName = "Step";
    protected const string StepHandlerFieldName = "Handler";
    protected const string StepNextStepFieldName = "NextStep";
    protected const string StepActionRequiredFieldName = "ActionRequired";
    protected const string EmulatorParameterName = "emulator";
    protected const string ActionRequiredParameterName = "actionRequired";
    protected const string ErrorMethodName = "Error";
    protected const string HandleInterruptsMethodName = "HandleInterrupts";
    protected const string InterruptModeStepTableFieldName = "InterruptModeStepTable";
    private const string StepMethodPrefix = "Step";

    private protected Generator()
    {
    }

    [Pure]
    protected static string GetStepMethodName(Step step) =>
        step.MethodIndex != null
            ? $"{StepMethodPrefix}{step.MethodIndex}"
            : throw new InvalidOperationException($"Step {step.Name} does not have a {nameof(step.MethodIndex)}.");
}