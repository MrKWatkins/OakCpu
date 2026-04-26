using System.Reflection;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace MrKWatkins.OakCpu.Z80.Benchmarks.ZEXALL;

[MemoryDiagnoser]
[Config(typeof(Config))]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ZEXALLBenchmark
{
    private const int MemorySize = 65536;
    private const ushort StartAddress = 0x0100;
    private const ushort TestTableStartAddress = 0x013A;
    private const ushort PrintRoutineAddress = 0x0010;
    private const string ResourceName = "MrKWatkins.OakCpu.Z80.Benchmarks.zexall.bin";

    private byte[] initialMemory = null!;

    [Params("aluop a,nn")]
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public string Test { get; set; } = null!;

    [GlobalSetup]
    public void GlobalSetup() => initialMemory = CreateInitialMemory(Test);

    [Benchmark(Baseline = true)]
    public void OakEmu() => new OakEmuRunner(initialMemory).Run();

    [Benchmark]
    public void Z80InstructionEmulator() => new Z80InstructionEmulatorRunner(initialMemory).Run();

    [Benchmark]
    public void Z80StepEmulator() => new Z80StepEmulatorRunner(initialMemory).Run();

    [Pure]
    private static byte[] CreateInitialMemory(string testName)
    {
        var memory = new byte[MemorySize];

        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName) ??
                           throw new InvalidOperationException($"Embedded resource {ResourceName} was not found.");

        var offset = (int)StartAddress;
        while (true)
        {
            var bytesRead = stream.Read(memory.AsSpan(offset));
            if (bytesRead == 0)
            {
                break;
            }

            offset += bytesRead;
        }

        var testAddress = GetTestAddress(memory, testName);
        System.Buffers.Binary.BinaryPrimitives.WriteUInt16LittleEndian(memory.AsSpan(TestTableStartAddress), testAddress);
        System.Buffers.Binary.BinaryPrimitives.WriteUInt16LittleEndian(memory.AsSpan(TestTableStartAddress + 2), 0x0000);

        memory[0x0006] = 0xFF;
        memory[0x0007] = 0xFF;
        memory[PrintRoutineAddress] = 0xC9;
        memory[0x0038] = 0xC9;

        return memory;
    }

    [Pure]
    private static ushort GetTestAddress(byte[] memory, string testName)
    {
        for (var testTableAddress = TestTableStartAddress; ; testTableAddress += 2)
        {
            var testAddress = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(memory.AsSpan(testTableAddress));
            if (testAddress == 0)
            {
                break;
            }

            if (GetTestCaseName(memory, testAddress) == testName)
            {
                return testAddress;
            }
        }

        throw new InvalidOperationException($"Could not find the ZEXALL test case {testName}.");
    }

    [Pure]
    private static string GetTestCaseName(byte[] memory, ushort testCaseAddress)
    {
        var address = testCaseAddress + 65;
        var name = new StringBuilder();

        while (true)
        {
            var character = memory[address];
            if (character == 0x2E)
            {
                return name.ToString();
            }

            name.Append((char)character);
            address++;
        }
    }
}