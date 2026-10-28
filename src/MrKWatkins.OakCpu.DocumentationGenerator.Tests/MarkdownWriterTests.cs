namespace MrKWatkins.OakCpu.DocumentationGenerator.Tests;

public sealed class MarkdownWriterTests : TestFixture
{
    [TestCase("LD R0, (RP0 + d)")]
    [TestCase("ADD A, R0")]
    [TestCase("JR C0, d")]
    [TestCase("BIT N0, (IX + d)")]
    [TestCase("CCF")]
    [TestCase("SLL R0")]
    [TestCase("LDIR")]
    [TestCase("DAA")]
    public Task Write(string instruction) => Verify(MarkdownWriter.Write(GetDocumentation(Z80Documentation, instruction))).UseParameters(CpuDocumentation.Slug(instruction));

    [TestCase("LDIRX")]
    [TestCase("NEXTREG $n, $n")]
    public Task Write_Z80N(string instruction) => Verify(MarkdownWriter.Write(GetDocumentation(Z80NDocumentation, instruction))).UseParameters(CpuDocumentation.Slug(instruction));

    [Test]
    public void Write_SubstitutesSymbolicOperands()
    {
        var markdown = MarkdownWriter.Write(GetDocumentation(Z80Documentation, "SLL R0"));

        markdown.Should().StartWith("# SLL r\n");
        markdown.Should().NotContain("{R0}");
        markdown.Should().Contain("This instruction is undocumented.");
        markdown.Should().Contain("| Instruction | Opcode | T-States | Notes |");
    }
}