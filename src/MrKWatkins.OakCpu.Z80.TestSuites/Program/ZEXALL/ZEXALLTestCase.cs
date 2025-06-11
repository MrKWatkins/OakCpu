namespace MrKWatkins.OakCpu.Z80.TestSuites.Program.ZEXALL;

public sealed class ZEXALLTestCase : ProgramTestCase
{
    internal ZEXALLTestCase(string name, ushort testAddress, byte[] memory)
        : base(name, testAddress, memory)
    {
    }

    private protected override ushort StopAddress => 0x0000;

    private protected override ushort TestTableAddress => 0x013A;

    private protected override string PassedString => "OK";

    private protected override string ErrorString => "ERROR";

    private protected override void InitializeZ80(Z80TestHarness z80)
    {
        z80.RegisterPC = ZEXALLTestSuite.StartAddress;

        // SP - loaded by first instructions.
        z80.WriteByteToMemory(0x0006, 0xFF);
        z80.WriteByteToMemory(0x0007, 0xFF);

        // Do nothing for RST $38.
        z80.WriteByteToMemory(0x0038, 0xC9);
    }

    private protected override PrintInterceptor CreatePrintInterceptor(Z80TestHarness z80, ResultWatchingOutput output) => new CPMPrintInterceptor(z80, output);
}