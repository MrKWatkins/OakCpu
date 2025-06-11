namespace MrKWatkins.OakCpu.Z80.TestSuites.InstructionLevel;

public abstract class InstructionLevelTestCase : TestCase
{
    private protected InstructionLevelTestCase(string name, Assertions assertionsToRun = Assertions.All)
        : base(name)
    {
        AssertionsToRun = assertionsToRun;
    }

    public Assertions AssertionsToRun { get; }
}