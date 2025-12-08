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
    protected const string ErrorFunctionName = "Error";
    protected const string HandleInterruptsMethodName = "HandleInterrupts";
    protected const string InterruptModeStepTableFieldName = "InterruptModeStepTable";
    private const string StepImplementationPrefix = "StepImplementation_";

    private protected Generator()
    {
    }

    [Pure]
    protected static string GetStepImplementationName(Step step) =>
        step.FunctionIndex != null
            ? $"{StepImplementationPrefix}{step.FunctionIndex}"
            : throw new InvalidOperationException($"Step {step.Name} does not have a function index.");
}