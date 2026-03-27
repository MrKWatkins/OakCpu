using BenchmarkDotNet.Configs;

namespace MrKWatkins.OakCpu.Z80.Benchmarks.ZEXALL;

public class Config : ManualConfig
{
    public Config()
    {
        // T-States taken from the output of the test case in the TestSuites project.
        AddColumn(new ZXSpectrumSpeedColumn(562_613_742));
    }
}