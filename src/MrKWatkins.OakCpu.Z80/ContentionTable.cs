using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MrKWatkins.OakCpu.Z80;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed class ContentionTable
{
    public const int TStatesPerFrame = 69888;

    public static readonly ContentionTable EarlyTimings = new(true);
    public static readonly ContentionTable LateTimings = new(false);

    private const int PatternSize = 8;
    private const int PatternsPerRow = 16;
    private const int TStatesInBorderAndHorizontalRefresh = 96;
    private const int Rows = 192;
    private readonly byte[] table;

    /// <summary>
    /// Creates a contention table with a constant delay for all T-states. For testing only.
    /// </summary>
    internal ContentionTable(byte delay)
    {
        table = new byte[TStatesPerFrame];
        Array.Fill(table, delay);
    }

    private ContentionTable(bool isEarlyTimings)
    {
        Span<byte> pattern = [6, 5, 4, 3, 2, 1, 0, 0];
        table = new byte[TStatesPerFrame];
        var index = isEarlyTimings ? 14335 : 14336;
        for (var row = 0; row < Rows; row++)
        {
            for (var patternIndex = 0; patternIndex < PatternsPerRow; patternIndex++)
            {
                pattern.CopyTo(table.AsSpan(index, PatternSize));
                index += PatternSize;
            }

            index += TStatesInBorderAndHorizontalRefresh;
        }
    }

    public byte this[int tStatesAfterInterrupt]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(table), tStatesAfterInterrupt);
    }
}