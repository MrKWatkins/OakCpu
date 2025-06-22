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

    protected static void AdjustForOverlappedRead(Z80TestHarness z80, List<Cycle> cycles)
    {
        // If the last cycle was a MemoryRead, then we've had an overlapped read. The SingleStep tests assume instruction level
        // execution so won't take this into account. We need to adjust the PC and remove the event.
        if (cycles.Last().IsOpcodeRead)
        {
            z80.RegisterPC--;
            cycles.RemoveAt(cycles.Count - 1);
        }
    }

    protected void AssertCycles(Z80TestHarness z80, IReadOnlyList<Cycle> expectedCycles, IReadOnlyList<Cycle> actualCycles)
    {
        if (!AssertionsToRun.HasFlag(Assertions.Cycles))
        {
            return;
        }

        z80.AssertEqual(actualCycles.Count, expectedCycles.Count, $"Expected {expectedCycles.Count} cycles but was {actualCycles.Count}");

        for (var f = 0; f < Math.Min(actualCycles.Count, expectedCycles.Count); f++)
        {
            AssertCycle(z80, actualCycles[f], expectedCycles[f], f);
        }
    }

    protected static void AssertCycle(Z80TestHarness z80, Cycle actual, Cycle expected, int index)
    {
        z80.AssertEqual(actual.Type, expected.Type, $"Expected cycle {index} to have type {expected.Type} but was {actual.Type}");
        z80.AssertEqual(actual.Address, expected.Address, $"Expected cycle {index} to have address {expected.Address} but was {actual.Address}");

        if (expected.Data.HasValue)
        {
            z80.AssertEqual(actual.Data, expected.Data, $"Expected cycle {index} to have data {expected.Data} but was {actual.Data}");
        }
    }
}