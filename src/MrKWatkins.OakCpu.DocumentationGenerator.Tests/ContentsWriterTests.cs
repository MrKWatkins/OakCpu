namespace MrKWatkins.OakCpu.DocumentationGenerator.Tests;

public sealed class ContentsWriterTests : TestFixture
{
    [Test]
    public Task WriteIndex() => Verify(ContentsWriter.WriteIndex("Z80N", Z80NDocumentation));

    [Test]
    public void WriteIndex_LinksEveryInstruction()
    {
        var markdown = ContentsWriter.WriteIndex("Z80", Z80Documentation);

        markdown.Should().StartWith("# Z80 Instructions\n");
        foreach (var instruction in Z80Documentation)
        {
            markdown.Should().Contain($"]({instruction.Page})");
        }

        markdown.Should().Contain("## 8-Bit Load\n");
        markdown.Should().Contain("- [LD r, (rr + d)](8-bit-load/ld-r0-at-rp0-d.md): Loads the byte at (rr + d) into r.\n");
        markdown.Should().Contain("- *[SLL r](rotate-and-shift/sll-r0.md)*: ");
    }

    [Test]
    public Task WriteOpcodeTables() => Verify(ContentsWriter.WriteOpcodeTables("Z80N", Z80NDocumentation));

    [Test]
    public void WriteOpcodeTables_HasATablePerPrefix()
    {
        var markdown = ContentsWriter.WriteOpcodeTables("Z80", Z80Documentation);

        markdown.Should().StartWith("# Z80 Opcodes\n");
        markdown.Split('\n').Where(line => line.StartsWith("## ", StringComparison.Ordinal)).Should().SequenceEqual(
            "## No prefix",
            "## CB prefix",
            "## DD prefix",
            "## DD CB prefix",
            "## ED prefix",
            "## FD prefix",
            "## FD CB prefix");
    }

    [Test]
    public void WriteOpcodeTables_LinksEveryOpcode()
    {
        var markdown = ContentsWriter.WriteOpcodeTables("Z80", Z80Documentation);

        // Every opcode table is full on the Z80: the unused opcodes are all duplicates of other instructions, and the prefix
        // bytes link to their tables.
        markdown.Split('\n').Count(line => line.StartsWith("| **", StringComparison.Ordinal)).Should().Equal(7 * 16);
        markdown.Should().NotContain("|  |");
        markdown.Should().Contain("| [CB prefix](#cb-prefix) |");
        markdown.Should().Contain("| [DD CB prefix](#dd-cb-prefix) |");
        markdown.Should().Contain("| [ED prefix](#ed-prefix) |");

        markdown.Should().Contain("[LD B, (IX + d)](8-bit-load/ld-r0-at-rp0-d.md)");
        markdown.Should().Contain("*[NOP](general-purpose-arithmetic-and-cpu-control/nop.md)*");
        markdown.Should().Contain("[RLC (IX + d)](rotate-and-shift/rlc-at-ix-d.md)");
    }
}