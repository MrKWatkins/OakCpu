namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction;

public abstract class InstructionTestCase : TestCase
{
    private protected InstructionTestCase(string name, InstructionTestSuiteOptions options)
        : base(name)
    {
        AssertionsToRun = options.GetAssertionsToRunFor(name);
        MemoryCycleMethod = options.MemoryCycleMethod;
    }

    public Assertions AssertionsToRun { get; }

    public MemoryCycleMethod MemoryCycleMethod { get; }

    [Pure]
    protected static TTestHarness CreateZ80<TTestHarness>(Z80InputState inputState)
        where TTestHarness : Z80TestHarness, new()
    {
        var z80 = new TTestHarness { RecordCycles = true };
        z80.SetIO(new InstructionIO(z80, inputState));
        return z80;
    }

    protected static void AdjustForOverlappedRead(Z80TestHarness z80)
    {
        // If the last cycle was a MemoryRead, then we've had an overlapped read. The SingleStep tests assume instruction level
        // execution so won't take this into account. We need to adjust the PC and remove the event.
        if (z80.MutableCycles?.Last().IsOpcodeRead == true)
        {
            z80.RegisterPC--;
            z80.MutableCycles.RemoveAt(z80.MutableCycles.Count - 1);
        }
    }
}