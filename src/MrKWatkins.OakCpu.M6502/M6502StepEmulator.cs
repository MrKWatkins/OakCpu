namespace MrKWatkins.OakCpu.M6502;

public partial class M6502StepEmulator
{
    private const ushort OpcodeReadStep1 = OpcodeReadStep0 + 1;

    /// <summary>
    /// Executes a single instruction; used for testing.
    /// </summary>
    /// <param name="onStepComplete">Called after every step. Use to perform any action required at the end of a step.</param>
    /// <param name="onBeforeStep">Optional function that is called before each step.</param>
    public void ExecuteInstruction(Action<ActionRequired> onStepComplete, Action? onBeforeStep = null)
    {
        ArgumentNullException.ThrowIfNull(onStepComplete);

        var readingOpcode = true;
        while (true)
        {
            onBeforeStep?.Invoke();
            var actionRequired = Step();
            onStepComplete(actionRequired);

            if (!readingOpcode)
            {
                if (currentStep == OpcodeReadStep0)
                {
                    return;
                }
                if (actionRequired == ActionRequired.OpcodeRead && currentStep == OpcodeReadStep1)
                {
                    PC -= 1;
                    currentStep = OpcodeReadStep0;
                    return;
                }
            }

            if (readingOpcode && currentStep != OpcodeReadStep1)
            {
                readingOpcode = false;
            }
        }
    }
}