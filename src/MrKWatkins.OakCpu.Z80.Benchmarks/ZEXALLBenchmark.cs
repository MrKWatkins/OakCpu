using BenchmarkDotNet.Attributes;
using MrKWatkins.OakCpu.Z80.Tests;
using MrKWatkins.OakCpu.Z80.TestSuites.Program.ZEXALL;

namespace MrKWatkins.OakCpu.Z80.Benchmarks;

// | Method | Test        | Mean    | Error    | StdDev   | Code Size | Allocated |
// |------- |------------ |--------:|---------:|---------:|----------:|----------:|
// | ZEXALL | <inc,dec> a | 3.021 s | 0.0054 s | 0.0050 s |      84 B |  65.71 KB | 2025-06-16 First measurement.
[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 20)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ZEXALLBenchmark
{
    private ZEXALLTestCase testCase = null!;

    [Params("<inc,dec> a")]
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public string Test { get; set; } = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        testCase = ZEXALLTestSuite.ZEXALL.TestCases.First(t => t.Name == Test);
    }

    [Benchmark]
    public void ZEXALL()
    {
        testCase.Execute<Z80EmulatorTestHarness>();
    }
}