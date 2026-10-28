namespace MrKWatkins.OakCpu.DocumentationGenerator.Tests;

public sealed class YamlWriterTests : TestFixture
{
    [TestCase("LD R0, (RP0 + d)")]
    [TestCase("JR C0, d")]
    [TestCase("RST N0")]
    [TestCase("DAA")]
    public Task Write(string instruction) => Verify(YamlWriter.Write(Z80Model.Cpu, [GetDocumentation(Z80Documentation, instruction)])).UseParameters(CpuDocumentation.Slug(instruction));

    [TestCase("LDA abs,X")]
    [TestCase("NOP")]
    public Task Write_M6502(string instruction) => Verify(YamlWriter.Write(M6502Model.Cpu, [GetDocumentation(M6502Documentation, instruction)])).UseParameters(CpuDocumentation.Slug(instruction));

    [Test]
    public void Write_NamesTheCyclesAfterTheCpu()
    {
        var yaml = YamlWriter.Write(M6502Model.Cpu, [GetDocumentation(M6502Documentation, "LDA abs,X")]);

        yaml.Should().StartWith("cpu: 6502\ninstructions:\n  - instruction: LDA abs,X\n    defined_by: 6502\n");
        yaml.Should().Contain("        cycles: [4, 5]\n");
        yaml.Should().NotContain("t_states");
    }

    [Test]
    public void Write_QuotesSpecialCharacters()
    {
        var yaml = YamlWriter.Write(Z80Model.Cpu, [GetDocumentation(Z80Documentation, "LD R0, (RP0 + d)")]);

        yaml.Should().StartWith("cpu: Z80\ninstructions:\n  - instruction: LD R0, (RP0 + d)\n");
        yaml.Should().Contain("    documentation: \"");
        yaml.Should().Contain("        url: \"https://");
    }

    [Test]
    public void Write_UsesTheDefinitionKeys()
    {
        var yaml = YamlWriter.Write(Z80Model.Cpu, [GetDocumentation(Z80Documentation, "LD R0, (RP0 + d)")]);

        yaml.Should().Contain("      - opcode: DD 46 d\n        instruction: LD B, (IX + d)\n        R0: B\n        RP0: IX\n        t_states: [19]\n        undocumented: false\n");
        yaml.Should().NotContain("mnemonic");
        yaml.Should().NotContain("encoding");
    }
}