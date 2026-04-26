using MrKWatkins.EmulatorTestSuites.Z80;
using MrKWatkins.EmulatorTestSuites.ZXSpectrum;
using MrKWatkins.EmulatorTestSuites.ZXSpectrum.Timing;
using MrKWatkins.OakCpu.Z80.Testing;

namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class TimingTests
{
    [TestCaseSource(nameof(TestCases))]
    public void Timing(TimingTestCase testCase) => testCase.Execute<ContendedZXSpectrumTestHarness>(TestContext.Progress);

    [Pure]
    public static IEnumerable<TestCaseData> TestCases() => TimingTestSuite.Instance.TestCases.ToTestCaseData();

    [UsedImplicitly]
    public sealed class ContendedZXSpectrumTestHarness : ZXSpectrumTestHarness
    {
        private readonly ContendedZ80StepEmulatorTestHarness z80 = new();

        public override Z80TestHarness Z80 => z80;

        public override Z80SteppableTestHarness SteppableZ80 => z80;

        public override void StartFrame() => z80.StartFrame();
    }
}
