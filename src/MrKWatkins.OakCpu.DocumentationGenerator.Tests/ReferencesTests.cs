namespace MrKWatkins.OakCpu.DocumentationGenerator.Tests;

public sealed class ReferencesTests
{
    [Test]
    public void For_Z80Documented() =>
        References.For("Z80", false).Select(r => r.Covers).Should().SequenceEqual(ReferenceCoverage.Documented, ReferenceCoverage.All);

    [Test]
    public void For_Z80Undocumented() =>
        References.For("Z80", true).Select(r => r.Covers).Should().SequenceEqual(ReferenceCoverage.All, ReferenceCoverage.Undocumented);

    [Test]
    public void For_Z80N() => References.For("Z80N", false).Should().HaveCount(1);

    [Test]
    public void For_UnknownCpu() => References.For("6502", false).Should().BeEmpty();
}