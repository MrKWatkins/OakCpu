namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class ContentionTableTests
{
    // Before the first line.
    [TestCase(0, 0)]
    [TestCase(14334, 0)]

    // The first line.
    [TestCase(14335, 6)]
    [TestCase(14336, 5)]
    [TestCase(14337, 4)]
    [TestCase(14338, 3)]
    [TestCase(14339, 2)]
    [TestCase(14340, 1)]
    [TestCase(14341, 0)]
    [TestCase(14342, 0)]
    [TestCase(14343, 6)]
    [TestCase(14344, 5)]
    [TestCase(14345, 4)]
    [TestCase(14346, 3)]
    [TestCase(14347, 2)]
    [TestCase(14348, 1)]
    [TestCase(14349, 0)]
    [TestCase(14350, 0)]
    [TestCase(14455, 6)]
    [TestCase(14456, 5)]
    [TestCase(14457, 4)]
    [TestCase(14458, 3)]
    [TestCase(14459, 2)]
    [TestCase(14460, 1)]
    [TestCase(14461, 0)]
    [TestCase(14462, 0)]

    // Border after the first line.
    [TestCase(14463, 0)]
    [TestCase(14558, 0)]

    // The second line.
    [TestCase(14559, 6)]
    [TestCase(14684, 1)]

    // Border after the second line.
    [TestCase(14687, 0)]

    // The last line.
    [TestCase(57119, 6)]
    [TestCase(57244, 1)]

    // Border after the last line.
    [TestCase(57247, 0)]

    // After the border after the last line.
    [TestCase(57343, 0)]
    public void Get(int tStatesAfterInterrupt, byte expected)
    {
        ContentionTable.EarlyTimings[tStatesAfterInterrupt].Should().Equal(expected);
        ContentionTable.LateTimings[tStatesAfterInterrupt + 1].Should().Equal(expected);
    }

    [Test]
    public void FuseCheck()
    {
        // Based on https://sourceforge.net/p/fuse-emulator/fuse/ci/master/tree/unittests/unittests.c.
        var checksum = 0L;
        for (var f = 0; f < 80000; f++)
        {
            checksum += ContentionTable.EarlyTimings[f % ContentionTable.TStatesPerFrame] * (f + 1);
        }

        checksum.Should().Equal(2308862976L);
    }
}