namespace MrKWatkins.OakCpu.CodeGenerator.Yaml;

public enum NextOpcodeMode
{
    /// <summary>
    /// Sets the next step to 0 to start a new opcode read next step.
    /// </summary>
    Read,

    /// <summary>
    /// Executes step 0, then sets the next step to 1 to perform an overlapped opcode read.
    /// </summary>
    Overlapped,

    /// <summary>
    /// Custom handling; the user needs to handle setting the next step.
    /// </summary>
    Custom
}