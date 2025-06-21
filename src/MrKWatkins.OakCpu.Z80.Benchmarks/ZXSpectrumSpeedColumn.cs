using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace MrKWatkins.OakCpu.Z80.Benchmarks;

public sealed class ZXSpectrumSpeedColumn(long tStates) : IColumn
{
    public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
    {
        var statistics = summary[benchmarkCase]?.ResultStatistics;
        if (statistics == null)
        {
            return "???";
        }

        var elapsedNanoseconds = statistics.Mean;
        var elapsedSeconds = elapsedNanoseconds / 1000.0 / 1000.0 / 1000.0;
        var tStatesPerSecond = tStates / elapsedSeconds;
        // https://retrocomputing.stackexchange.com/a/12833
        const double spectrumTStatesPerSecond = 70908 * 50.01;
        var speedFactor = tStatesPerSecond / spectrumTStatesPerSecond;

        return $"x{speedFactor:N2}";
    }

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) => GetValue(summary, benchmarkCase);

    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => true;

    public bool IsAvailable(Summary summary) => true;

    public string Id => nameof(ZXSpectrumSpeedColumn);

    public string ColumnName => "ZX Speed";

    public bool AlwaysShow => true;

    public ColumnCategory Category => ColumnCategory.Custom;

    public int PriorityInCategory => 0;

    public bool IsNumeric => true;

    public UnitType UnitType => UnitType.Size;

    public string Legend => "Times faster than a ZX Spectrum";
}