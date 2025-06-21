namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction.SingleStep;

public sealed class SingleStepTestCase : InstructionTestCase
{
    internal SingleStepTestCase(string name, InstructionTestSuiteOptions options)
        : base(name, options)
    {
    }

    public override void Execute<TTestHarness>(TextWriter? testOutput = null)
    {
        testOutput?.WriteLine("Executing ");
        foreach (var step in Step.Load(this))
        {
            Execute<TTestHarness>(step, testOutput);
        }
    }

    private void Execute<TTestHarness>(Step step, TextWriter? testOutput)
        where TTestHarness : Z80TestHarness, new()
    {
        testOutput?.Write('.');

        // TODO: Avoid creating each time.
        var z80 = new TTestHarness();

        step.Input.Setup(z80);

        // TODO: Avoid creating each time.
        var cycles = new List<Cycle>(step.TStates);
        while (z80.TStates <= (ulong)step.TStates)
        {
            cycles.AddRange(z80.Cycle());
        }

        AdjustForOverlappedRead(z80, cycles);

        Assert(step, z80, cycles);
    }

    private void Assert(Step step, Z80TestHarness z80, IReadOnlyList<Cycle> cycles)
    {
        using (z80.CreateAssertionScope($"Step {step.Index}"))
        {
            step.Expected.Assert(AssertionsToRun, z80);
            AssertCycles(z80, step.Cycles, cycles);
        }
    }
}