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
    public Task Write(string form) => Verify(MarkdownWriter.Write(GetDocumentation(Z80Documentation, form))).UseParameters(CpuDocumentation.Slug(form));

    [TestCase("LDIRX")]
    [TestCase("NEXTREG $n, $n")]
    public Task Write_Z80N(string form) => Verify(MarkdownWriter.Write(GetDocumentation(Z80NDocumentation, form))).UseParameters(CpuDocumentation.Slug(form));

    [Test]
    public void Write_FrontMatterQuotesSpecialCharacters()
    {
        var markdown = MarkdownWriter.Write(GetDocumentation(Z80Documentation, "LD R0, (RP0 + d)"));

        markdown.Should().StartWith("---\ncpu: Z80\n");
        markdown.Should().Contain("form: LD R0, (RP0 + d)\n");
        markdown.Should().Contain("description: \"");
        markdown.Should().Contain("url: \"https://");
    }

    [Test]
    public void Write_SubstitutesSymbolicOperandsInTheBody()
    {
        var markdown = MarkdownWriter.Write(GetDocumentation(Z80Documentation, "SLL R0"));
        var body = markdown[(markdown.IndexOf("\n---\n", StringComparison.Ordinal) + 5)..];

        body.Should().StartWith("\n# SLL r\n");
        body.Should().NotContain("{R0}");
        body.Should().Contain("This instruction is undocumented.");
    }
}