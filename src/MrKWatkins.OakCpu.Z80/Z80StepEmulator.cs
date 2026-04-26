namespace MrKWatkins.OakCpu.Z80;

public partial class Z80StepEmulator
{
    private const ushort OpcodeReadStep0 = 0;
    private const ushort OpcodeReadStep1 = 1;
    private const ushort OpcodeReadStep2 = 2;
    private const ushort HaltedStep0 = 8;
    private const ushort HaltedStep1 = 9;
    private const ushort HaltedStep2 = 10;
    private const ushort IM0Start = 12;
    private const ushort IM1Start = 17;
    private const ushort IM2Start = 30;

    /// <summary>
    /// Gets a value indicating whether the emulator is at the start of an instruction or interrupt sequence.
    /// </summary>
    public bool IsAtInstructionBoundary => IsInstructionBoundaryStart(currentStep);

    /// <summary>
    /// Executes a single instruction; used for testing. Assumes the processor is at the start of an instruction.
    /// </summary>
    /// <remarks>
    /// Will not perform overlapped reads; it applies queued overlap work internally and stops before the next sequence start performs its external bus action.
    /// Will be slower than executing steps normally due to resetting the state at the end and delegate overhead from calling <paramref name="onStepComplete"/>.
    /// </remarks>
    /// <param name="onStepComplete">Called after every step. Use to perform any action required at the end of a step.</param>
    /// <param name="onBeforeStep">Optional function that is called before each step.</param>
    public void ExecuteInstruction(Action<ActionRequired> onStepComplete, Action? onBeforeStep = null)
    {
        // Are we reading the opcode, i.e. are we in steps 0 or 1, the first 2 steps of the opcode read sequence?
        var readingOpcode = true;
        while (true)
        {
            onBeforeStep?.Invoke();
            var actionRequired = Step();

            if (!readingOpcode && IsInstructionBoundaryStart(currentStep))
            {
                ExecuteOverlap();
                if (actionRequired != ActionRequired.OpcodeRead)
                {
                    onStepComplete(actionRequired);
                }
                return;
            }

            switch (currentStep)
            {
                case OpcodeReadStep0:
                case HaltedStep0:
                    // If we're not reading the opcode, then we must have completed the instruction or completed a HALT cycle.
                    // Perform any final action and finish. No need to check the opcode table as reading the second byte of an
                    // opcode is done with an overlapped read.
                    if (!readingOpcode)
                    {
                        onStepComplete(actionRequired);
                        return;
                    }

                    break;

                case OpcodeReadStep1:
                    // If we're not on the no-prefix table, then we're reading the second byte of the opcode.
                    if (opcodeStepTable != OpcodeStepTableNoPrefix)
                    {
                        readingOpcode = true;
                        break;
                    }

                    // If we're not reading the opcode, then we've completed the instruction and this is an overlapped read. Do not perform the action
                    // and reset the step and PC.
                    if (!readingOpcode)
                    {
                        PC -= 1;
                        currentStep = 0;
                        return;
                    }

                    break;

                case HaltedStep1:
                    // If we're not reading the opcode, then we've just completed a HALT instruction and overlapped the first step
                    // of the HALT cycle. No need to reset PC as the first step of HALT does not increase PC, just move to the start
                    // of the HALT cycle.
                    if (!readingOpcode)
                    {
                        currentStep = HaltedStep0;
                        return;
                    }

                    break;

                // If we've reached step 2, then we're done with the steps of opcode read we care about.
                case OpcodeReadStep2:
                case HaltedStep2:
                    readingOpcode = false;
                    break;
            }

            onStepComplete(actionRequired);
        }
    }

    [Pure]
    private static bool IsInstructionBoundaryStart(ushort step) => step is OpcodeReadStep0 or HaltedStep0 or IM0Start or IM1Start or IM2Start;
}