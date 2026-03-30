using MrKWatkins.EmulatorTestSuites.Z80;

namespace MrKWatkins.OakCpu.Z80.Testing;

/// <summary>
/// Models the 48K Spectrum floating bus by sampling the byte the ULA is fetching for display output.
/// </summary>
/// <remarks>
/// The one-T-state sample offset reflects that the IO read callback observes the bus one T-state after the CPU asks for the port value.
/// </remarks>
internal sealed class FloatingBusIO(Func<int> getTStatesInCurrentFrame, Func<ushort, byte> readByteFromMemory) : IIOReader, IIOWriter
{
    private const byte DefaultBusValue = 0xBF;
    private const byte UndrivenBusValue = 0xFF;
    private const int FloatingBusDisplayStart = 14_338;
    private const int FloatingBusDisplayLines = 192;
    private const int FloatingBusTStatesPerLine = 224;
    private const int FloatingBusActiveFetchesPerLine = 128;
    private const int FloatingBusSampleOffset = 1;
    private const int TStatesPerFrame = 69_888;

    public byte Read(ushort port)
    {
        if ((port & 0x00FF) == 0xFE || (port & 1) == 0)
        {
            return DefaultBusValue;
        }

        var tStatesInCurrentFrame = getTStatesInCurrentFrame() + FloatingBusSampleOffset;
        if (tStatesInCurrentFrame >= TStatesPerFrame)
        {
            tStatesInCurrentFrame -= TStatesPerFrame;
        }

        if (tStatesInCurrentFrame < FloatingBusDisplayStart)
        {
            return UndrivenBusValue;
        }

        var relative = tStatesInCurrentFrame - FloatingBusDisplayStart;
        var line = relative / FloatingBusTStatesPerLine;
        if (line is < 0 or >= FloatingBusDisplayLines)
        {
            return UndrivenBusValue;
        }

        var offset = relative % FloatingBusTStatesPerLine;
        if (offset >= FloatingBusActiveFetchesPerLine)
        {
            return UndrivenBusValue;
        }

        var burstOffset = offset % 8;
        if (burstOffset >= 4)
        {
            return UndrivenBusValue;
        }

        var x = offset / 8 * 2 + (burstOffset >> 1);
        var address = (burstOffset & 1) == 0 ? PixelAddress(line, x) : AttributeAddress(line, x);
        return readByteFromMemory(address);
    }

    public void Write(ushort port, byte value)
    {
    }

    [Pure]
    private static ushort PixelAddress(int line, int x) => (ushort)(0x4000 | ((line & 0xC0) << 5) | ((line & 0x07) << 8) | ((line & 0x38) << 2) | x);

    [Pure]
    private static ushort AttributeAddress(int line, int x) => (ushort)(0x5800 + (line >> 3) * 32 + x);
}