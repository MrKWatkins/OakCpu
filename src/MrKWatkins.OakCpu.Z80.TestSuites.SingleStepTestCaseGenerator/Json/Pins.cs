namespace MrKWatkins.OakCpu.Z80.TestSuites.SingleStepTestCaseGenerator.Json;

[Flags]
public enum Pins
{
    None = 0,
    Read = 1,
    Write = 2,
    Memory = 4,
    IO = 8
}