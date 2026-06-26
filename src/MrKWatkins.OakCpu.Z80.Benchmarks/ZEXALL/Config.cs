using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace MrKWatkins.OakCpu.Z80.Benchmarks.ZEXALL;

public class Config : ManualConfig
{
    public Config()
    {
        // T-States taken from the output of the test case in the TestSuites project.
        AddColumn(new ZXSpectrumSpeedColumn(562_613_742));

        // The hot emulation loop runs ~2x faster once dynamic PGO recompiles it to its profile-guided tier-1. That
        // background rejit lands shortly after the loop gets hot, so with too little warmup the measurement can catch the
        // slower pre-PGO tier; on a loaded machine that made results bimodal (~x140 vs ~x335). A generous fixed warmup
        // guarantees the PGO rejit has completed before measurement, matching the steady state a long-running emulator
        // (billions of instructions) always reaches.
        AddJob(Job.Default.WithWarmupCount(20));
    }
}