using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using MrKWatkins.EmulatorTestSuites.Z80.Program.ZEXALL;
using MrKWatkins.OakCpu.Z80.Tests;

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
// | ZEXALL | <inc,dec> a |  204.1 ms |  4.03 ms |  4.80 ms |      54 B |  64.87 KB | 2025-06-18 Remove some array bounds checks.
// | ZEXALL | <inc,dec> a |  191.5 ms |  3.61 ms |  3.37 ms |      84 B |  64.87 KB | 2025-06-18 Remove some more empty steps.
//
// Switched to aluop a,nn to match OakEmu, now that the test passes.
//
// | Method | Test       | Mean    | Error    | StdDev   | ZX Speed |  Allocated |
// |------- |----------- |--------:|---------:|---------:|---------:|-----------:|
// | ZEXALL | aluop a,nn | 1.714 s | 0.0087 s | 0.0081 s |   x92.58 |    2.38 MB | OakEmu for reference.
// | ZEXALL | aluop a,nn | 1.842 s | 0.0362 s | 0.0483 s |   x86.14 |   65.71 KB | 2025-06-21 After fixing various flags.
// | ZEXALL | aluop a,nn | 2.167 s | 0.0425 s | 0.0489 s |   x73.23 |   65.69 KB | 2025-06-24 After Q and interrupts.
// | ZEXALL | aluop a,nn | 2.132 s | 0.0420 s | 0.0789 s |   x74.43 |   65.69 KB | 2025-06-25 After adding a separate halt cycle.
// | ZEXALL | aluop a,nn | 2.068 s | 0.0257 s | 0.0240 s |   x76.74 |   65.69 KB | 2025-06-25 After fixing LDIR. (Does that mean it's now doing fewer cycles? Error has dropped a lot too. Or just variation in the benchmark? Who knows...)
// | ZEXALL | aluop a,nn | 2.424 s | 0.0286 s | 0.0253 s |   x65.45 |   65.70 KB | 2025-06-07 After completing emulator and separating out test suites. No idea why the speed has dropped so much...
[MemoryDiagnoser]
[Config(typeof(Config))]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ZEXALLBenchmark
{
    private ZEXALLTestCase testCase = null!;

    [Params("aluop a,nn")]
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

    private class Config : ManualConfig
    {
        public Config()
        {
            // T-States taken from the output of the test case in the TestSuites project.
            AddColumn(new ZXSpectrumSpeedColumn(562_613_742));
        }
    }
}