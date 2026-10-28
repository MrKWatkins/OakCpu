using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.DocumentationGenerator.Tests;

public sealed class OperandsTests : TestFixture
{
    [TestCase("NOP", "")]
    [TestCase("LD R0, R1", "R0 R1")]
    [TestCase("LD R0, (RP0 + d)", "R0 RP0")]
    [TestCase("ADD RP0, RP1", "RP0 RP1")]
    [TestCase("BIT N0, R0", "N0 R0")]
    [TestCase("JP C0, nn", "C0")]
    public void PlaceholdersIn(string mnemonic, string expected) => string.Join(' ', Operands.PlaceholdersIn(mnemonic)).Should().Equal(expected);

    [TestCase("Copies {R1} into {R0}.", "R1 R0")]
    [TestCase("Does nothing.", "")]
    [TestCase("Tests bit {N0} of {R0}, {R0} again.", "N0 R0")]
    public void PlaceholdersInDocumentation(string documentation, string expected) => string.Join(' ', Operands.PlaceholdersInDocumentation(documentation)).Should().Equal(expected);

    [Test]
    public void For()
    {
        var yaml = GetInstructionYaml(Z80Model, "LD R0, (RP0 + d)");
        var opcode = yaml.Opcodes.Single(o => o.PrefixByte == 0xFD && o.OpcodeByte == 0x4E);

        var operands = Operands.For(yaml.Mnemonic, opcode);

        operands.Select(o => $"{o.Key}={o.Value}").Should().SequenceEqual("R0=C", "RP0=IY");
    }

    [Test]
    public void For_Bit()
    {
        var yaml = GetInstructionYaml(Z80Model, "BIT N0, R0");
        var opcode = yaml.Opcodes.Single(o => o.OpcodeByte == 0x5F);

        Operands.For(yaml.Mnemonic, opcode).Select(o => $"{o.Key}={o.Value}").Should().SequenceEqual("N0=3", "R0=A");
    }

    [Test]
    public void Substitute()
    {
        var operands = new[] { new KeyValuePair<string, string>("R0", "B"), new KeyValuePair<string, string>("RP0", "IX") };

        Operands.Substitute("Copies the byte at ({RP0} + d) into {R0}.", operands).Should().Equal("Copies the byte at (IX + d) into B.");
    }

    [Test]
    public void Substitute_ThrowsForUnknownPlaceholder() =>
        AssertThat.Invoking(() => Operands.Substitute("Copies {R1}.", [new KeyValuePair<string, string>("R0", "B")])).Should().Throw<InvalidOperationException>();

    [Test]
    public void SubstituteMnemonic() =>
        Operands.SubstituteMnemonic("LD R0, (RP0 + d)", [new KeyValuePair<string, string>("R0", "B"), new KeyValuePair<string, string>("RP0", "IX")]).Should().Equal("LD B, (IX + d)");

    [Test]
    public void Symbolic()
    {
        var yaml = GetInstructionYaml(Z80Model, "LD R0, (RP0 + d)");

        Operands.SubstituteMnemonic(yaml.Mnemonic, Operands.Symbolic(yaml.Mnemonic, yaml.Opcodes)).Should().Equal("LD r, (rr + d)");
    }

    [Test]
    public void Symbolic_BitNumber()
    {
        var yaml = GetInstructionYaml(Z80Model, "BIT N0, R0");

        Operands.SubstituteMnemonic(yaml.Mnemonic, Operands.Symbolic(yaml.Mnemonic, yaml.Opcodes)).Should().Equal("BIT b, r");
    }

    [Test]
    public void Symbolic_PageZeroAddress()
    {
        var yaml = GetInstructionYaml(Z80Model, "RST N0");

        Operands.SubstituteMnemonic(yaml.Mnemonic, Operands.Symbolic(yaml.Mnemonic, yaml.Opcodes)).Should().Equal("RST p");
    }

    [Test]
    public void Symbolic_Conditions()
    {
        var yaml = GetInstructionYaml(Z80Model, "CALL C0, nn");

        Operands.SubstituteMnemonic(yaml.Mnemonic, Operands.Symbolic(yaml.Mnemonic, yaml.Opcodes)).Should().Equal("CALL cc, nn");
    }

    [Test]
    public void Symbolic_TwoRegisters()
    {
        var yaml = GetInstructionYaml(Z80Model, "LD R0, R1");

        Operands.SubstituteMnemonic(yaml.Mnemonic, Operands.Symbolic(yaml.Mnemonic, yaml.Opcodes)).Should().Equal("LD r, r'");
    }

    [Test]
    public void Get_ThrowsForUnknownPlaceholder()
    {
        var opcode = GetInstructionYaml(Z80Model, "NOP").Opcodes[0];

        AssertThat.Invoking(() => Operands.Get(opcode, "R9")).Should().Throw<ArgumentException>();
    }
}