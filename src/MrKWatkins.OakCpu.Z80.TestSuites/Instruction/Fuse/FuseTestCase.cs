namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction.Fuse;

public sealed class FuseTestCase : InstructionTestCase
{
    internal FuseTestCase(string name, InstructionTestSuiteOptions options, FuseZ80InputState input, FuseZ80ExpectedState expected)
        : base(name, options)
    {
        Input = input;
        Expected = expected;

        Input.IOReads = expected.Events.Where(r => r.Type == FuseEventType.PortRead).Select(r => new IOEvent(r.Address, r.Data ?? 0)).ToList();

        // For Fuse, we just test memory and IO events. We ignore empty cycles because we have no way to get the Address value (IR) for None cycles after an opcode read.
        // We could infer some from the MemoryContends, but not all, plus Fuse puts IR on the address bus after incrementing R; it should be before.
        Expected.Cycles = CycleBuilder.BuildCycles(expected.Events, MemoryCycleMethod).ToList();
    }

    public FuseZ80InputState Input { get; }

    public FuseZ80ExpectedState Expected { get; }

    public override void Execute<TTestHarness>(TextWriter? testOutput = null)
    {
        var z80 = CreateZ80<TTestHarness>(Input);

        Input.Setup(z80);

        while (z80.TStates <= Input.MinimumTStatesToRun)
        {
            z80.ExecuteInstruction();
        }

        AdjustForOverlappedRead(z80);

        Assert(z80);
    }

    private void Assert(Z80TestHarness z80)
    {
        using (z80.CreateAssertionScope())
        {
            Expected.Assert(AssertionsToRun, z80);
        }
    }
}