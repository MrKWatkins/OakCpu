using BenchmarkDotNet.Running;
using MrKWatkins.OakCpu.Z80.Benchmarks.ZEXALL;

// BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
BenchmarkRunner.Run<ZEXALLBenchmark>();