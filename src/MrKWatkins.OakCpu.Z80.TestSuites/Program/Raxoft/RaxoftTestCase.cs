namespace MrKWatkins.OakCpu.Z80.TestSuites.Program.Raxoft;

public sealed class RaxoftTestCase : ProgramTestCase
{
    private const ushort StartAddress = 0x8000;
    private readonly RaxoftTestType type;

    internal RaxoftTestCase(string name, ushort testAddress, byte[] memory, RaxoftTestType type)
        : base(name, testAddress, memory)
    {
        this.type = type;
    }

    private protected override ushort StopAddress => 0x0000;

    private protected override ushort TestTableAddress => type switch
    {
        RaxoftTestType.Ccf => 0x887F,
        _ => 0x887A
    };

    private protected override string PassedString => "OK";

    private protected override string ErrorString => "FAILED";

    private protected override string SkippedString => "Skipped";

    private protected override void InitializeZ80(Z80TestHarness z80)
    {
        z80.RegisterPC = StartAddress;

        // CLS routine; needs interrupts. Return instead.
        z80.WriteByteToMemory(0x0D6B, 0xC9);

        // CHAN_OPEN routine; needs interrupts. Return instead.
        z80.WriteByteToMemory(0x1601, 0xC9);

        // TODO: 0XBF is expected on port 0xFE.
    }

    private protected override PrintInterceptor CreatePrintInterceptor(Z80TestHarness z80, ResultWatchingOutput output) => new ZXSpectrumPrintInterceptor(z80, output);
}