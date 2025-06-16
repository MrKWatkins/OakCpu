using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace MrKWatkins.OakCpu.Z80.Benchmarks;

[DisassemblyDiagnoser]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class BitCastVersusCmovBenchmark
{
    private byte A;
    private byte F;

    [Benchmark(Baseline = true)]
    public void BitCast()
    {
        A = (byte)(A + 0x01);
        // Flags.
        var flags = 0b00000000; // Reset N.
        flags |= A & 0b00101000; // Copy X and Y from A.
        flags |= F & 0b00000001; // Copy C from F.
        flags |= Unsafe.BitCast<bool, byte>(A == 0x80) << 2; // Set PV if A == 0x80 is true.
        flags |= Unsafe.BitCast<bool, byte>((A & 0x0F) == 0x00) << 4; // Set H if (A & 0x0F) == 0x00 is true.
        flags |= Unsafe.BitCast<bool, byte>(A == 0) << 6; // Set Z if is_zero(A) is true.
        flags |= A & 0x80; // Set S if is_negative(A) is true.
        F = (byte)flags;
    }

    [Benchmark]
    public void Cmov()
    {
        A = (byte)(A + 0x01);
        // Flags.
        var flags = 0b00000000; // Reset N.
        flags |= A & 0b00101000; // Copy X and Y from A.
        flags |= F & 0b00000001; // Copy C from F.
        flags |= A == 0x80 ? 4 : 0; // Set PV if A == 0x80 is true.
        flags |= (A & 0x0F) == 0x00 ? 16 : 0; // Set H if (A & 0x0F) == 0x00 is true.
        flags |= A == 0 ? 64 : 0; // Set Z if is_zero(A) is true.
        flags |= A & 0x80; // Set S if is_negative(A) is true.
        F = (byte)flags;
    }
}