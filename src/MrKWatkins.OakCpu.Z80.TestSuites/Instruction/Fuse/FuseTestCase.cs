namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction.Fuse;

public sealed class FuseTestCase : InstructionTestCase
{
    internal FuseTestCase(string name, InstructionTestSuiteOptions options, Input input, Expected expected)
        : base(name, options)
    {
        Input = input;
        Expected = expected;
    }

    public Input Input { get; }

    public Expected Expected { get; }


    public override void Execute<TTestHarness>(TextWriter? testOutput = null)
    {
        var z80 = new TTestHarness();

        Input.Setup(z80);

        var cycles = new List<Cycle>((int)Expected.TStates);
        while (z80.TStates <= Expected.TStates)
        {
            cycles.AddRange(z80.Cycle());
        }

        AdjustForOverlappedRead(z80, cycles);

        Assert(z80, cycles);
    }

    private void Assert(Z80TestHarness z80, IReadOnlyList<Cycle> cycles)
    {
        using (z80.CreateAssertionScope())
        {
            Expected.Assert(AssertionsToRun, z80);
            AssertCycles(z80, cycles);
        }
    }

    private void AssertCycles(Z80TestHarness z80, IReadOnlyList<Cycle> cycles)
    {
        if (!AssertionsToRun.HasFlag(Assertions.Cycles))
        {
            return;
        }

        // For Fuse, we just test memory and IO events. We ignore empty cycles because we have no way to get the Address value (IR) for None cycles after an opcode read.
        // We could infer some from the MemoryContends, but not all, plus Fuse puts IR on the address bus after incrementing R; it should be before.
        var actualCycles = cycles.Select((c, i) => (TState: i, Cycle: c)).Where(c => c.Cycle.Type != CycleType.None).ToList();
        var expectedCycles = CycleBuilder.BuildCycles(Expected.Events, MemoryCycleMethod).ToList();

        z80.AssertEqual(actualCycles.Count, expectedCycles.Count, $"Expected {expectedCycles.Count} cycles but was {actualCycles.Count}");

        for (var f = 0; f < Math.Min(actualCycles.Count, expectedCycles.Count); f++)
        {
            var actual = actualCycles[f];
            var expected = expectedCycles[f];

            z80.AssertEqual(actual.TState, expected.TState, $"Expected the T-State of cycle {f} to have be {expected.TState} but was {actual.TState}");

            AssertCycle(z80, actual.Cycle, expected.Cycle, f);
        }
    }
}