using BenchmarkDotNet.Running;
using MrKWatkins.OakCpu.Z80.Benchmarks;

// BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
BenchmarkRunner.Run<BitCastVersusCmovBenchmark>();