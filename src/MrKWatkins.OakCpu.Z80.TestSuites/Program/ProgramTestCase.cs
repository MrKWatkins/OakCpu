namespace MrKWatkins.OakCpu.Z80.TestSuites.Program;

public abstract class ProgramTestCase : TestCase
{
    private readonly ushort testAddress;
    private readonly byte[] memory;

    private protected ProgramTestCase(string name, ushort testAddress, byte[] memory)
        : base(name)
    {
        this.testAddress = testAddress;
        this.memory = memory;
    }

    public override void Execute<TTestHarness>(TextWriter? testOutput = null)
    {
        var z80 = new TTestHarness();
        z80.CopyIntoMemory(memory);
        InitializeZ80(z80);
        SetTestCase(z80);

        var resultWatcher = new ResultWatchingOutput(testOutput, PassedString, ErrorString, SkippedString);
        var printInterceptor = CreatePrintInterceptor(z80, resultWatcher);

        // TODO: TState limit.
        var stopAddress = StopAddress;
        while (true)
        {
            z80.Step();

            var pc = z80.RegisterPC;
            if (pc == PrintInterceptor.PrintRoutineAddress)
            {
                printInterceptor.PrintRoutineCalled(z80);
            }
            else if (pc == stopAddress)
            {
                break;
            }
        }

        switch (resultWatcher.Result)
        {
            case ProgramTestResult.None:
                z80.AssertFail("Test did not return a result.");
                break;

            case ProgramTestResult.Failed:
                z80.AssertFail("Test failed.");
                break;
        }
    }

    protected virtual void SetTestCase(Z80TestHarness z80)
    {
        // Write the address of the test at the start of the test table.
        z80.WriteWordToMemory(TestTableAddress, testAddress);

        // Write the 0x0000 terminator afterwards.
        z80.WriteWordToMemory((ushort)(TestTableAddress + 2), 0x0000);
    }

    private protected abstract ushort StopAddress { get; }

    private protected abstract ushort TestTableAddress { get; }

    private protected abstract string PassedString { get; }

    private protected abstract string ErrorString { get; }

    private protected virtual string? SkippedString => null;

    private protected abstract void InitializeZ80(Z80TestHarness z80);

    [Pure]
    private protected abstract PrintInterceptor CreatePrintInterceptor(Z80TestHarness z80, ResultWatchingOutput output);
}