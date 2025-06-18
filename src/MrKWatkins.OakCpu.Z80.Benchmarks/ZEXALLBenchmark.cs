using BenchmarkDotNet.Attributes;
using MrKWatkins.OakCpu.Z80.Tests;
using MrKWatkins.OakCpu.Z80.TestSuites.Program.ZEXALL;

namespace MrKWatkins.OakCpu.Z80.Benchmarks;

// | Method | Test        | Mean      | Error    | StdDev   | Code Size | Allocated |
// |------- |------------ |----------:|---------:|---------:|----------:|----------:|
// | ZEXALL | <inc,dec> a |  194.6 ms |  1.70 ms |  1.51 ms |           |   2.27 MB | OakEmu for reference.
//
// | ZEXALL | <inc,dec> a | 3021.0 ms | 54.00 ms | 50.00 ms |      84 B |  65.71 KB | 2025-06-16 First measurement.
// | ZEXALL | <inc,dec> a | 1762.0 ms | 50.00 ms | 47.00 ms |      84 B |  65.71 KB | 2025-06-16 Move flags into separate functions.
// | ZEXALL | <inc,dec> a | 1448.0 ms | 76.00 ms | 63.00 ms |      84 B |  65.71 KB | 2025-06-16 Reuse instruction steps for duplicates.
// | ZEXALL | <inc,dec> a |  326.5 ms |  7.72 ms | 22.77 ms |      54 B |  65.71 KB | 2025-06-17 Proof of concept for the Step array approach.
// | ZEXALL | <inc,dec> a |  297.8 ms |  5.81 ms | 10.48 ms |      84 B |  65.43 KB | 2025-06-18 After tidying and simplifying Step array approach.
// | ZEXALL | <inc,dec> a |  237.0 ms |  4.73 ms |  9.11 ms |      54 B |  64.87 KB | 2025-06-18 Skip executing step functions for empty steps.
// | ZEXALL | <inc,dec> a |  207.1 ms |  4.13 ms |  7.35 ms |      54 B |  64.87 KB | 2025-06-18 Use function pointers for step handlers.
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