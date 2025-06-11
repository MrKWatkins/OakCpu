namespace MrKWatkins.OakCpu.Z80.TestSuites.Program;

internal abstract class PrintInterceptor
{
    public const ushort PrintRoutineAddress = 0x0010;

    private protected PrintInterceptor(Z80TestHarness z80, ResultWatchingOutput output)
    {
        Z80 = z80;
        Output = output;
    }

    public Z80TestHarness Z80 { get; }

    public ResultWatchingOutput Output { get; }

    internal void PrintRoutineCalled(Z80TestHarness z80)
    {
        HandlePrintRoutine();

        // RET.
        var address = z80.ReadWordFromMemory(z80.RegisterSP);
        z80.RegisterSP += 2;
        z80.RegisterPC = address;
    }

    private protected abstract void HandlePrintRoutine();
}