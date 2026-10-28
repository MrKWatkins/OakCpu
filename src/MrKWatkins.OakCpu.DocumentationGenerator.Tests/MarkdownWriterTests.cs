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

    [TestCase("ADC #n")]
    [TestCase("BPL rel")]
    [TestCase("PLP")]
    [TestCase("NOP")]
    [TestCase("LAX #n")]
    public Task Write_M6502(string instruction) => Verify(MarkdownWriter.Write(GetDocumentation(M6502Documentation, instruction))).UseParameters(CpuDocumentation.Slug(instruction));

    [Test]
    public void Write_NamesTheCyclesAfterTheCpu()
    {
        var markdown = MarkdownWriter.Write(GetDocumentation(M6502Documentation, "LDA abs,X"));

        markdown.Should().Contain("| Instruction | Opcode | Cycles | Notes |\n| ----------- | ------ | ------ | ----- |\n| `LDA abs,X` | `BD abs` | 4 or 5 |  |\n");
    }

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