namespace MrKWatkins.OakCpu.DocumentationGenerator.Tests;

public sealed class YamlWriterTests : TestFixture
{
    [TestCase("LD R0, (RP0 + d)")]
    [TestCase("JR C0, d")]
    [TestCase("RST N0")]
    [TestCase("DAA")]
    public Task Write(string instruction) => Verify(YamlWriter.Write("Z80", [GetDocumentation(Z80Documentation, instruction)])).UseParameters(CpuDocumentation.Slug(instruction));

    [Test]
    public void Write_QuotesSpecialCharacters()
    {
        var yaml = YamlWriter.Write("Z80", [GetDocumentation(Z80Documentation, "LD R0, (RP0 + d)")]);

        yaml.Should().StartWith("cpu: Z80\ninstructions:\n  - instruction: LD R0, (RP0 + d)\n");
        yaml.Should().Contain("    documentation: \"");
        yaml.Should().Contain("        url: \"https://");
    }

    [Test]
    public void Write_UsesTheDefinitionKeys()
    {
        var yaml = YamlWriter.Write("Z80", [GetDocumentation(Z80Documentation, "LD R0, (RP0 + d)")]);

        yaml.Should().Contain("      - opcode: DD 46 d\n        instruction: LD B, (IX + d)\n        R0: B\n        RP0: IX\n        t_states: [19]\n        undocumented: false\n");
        yaml.Should().NotContain("mnemonic");
        yaml.Should().NotContain("encoding");
    }
}