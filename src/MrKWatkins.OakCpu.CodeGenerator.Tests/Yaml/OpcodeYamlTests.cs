using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public sealed class OpcodeYamlTests : TestFixture
{
    [Test]
    public void Deserialize_ValidOpcodeWithAllProperties()
    {
        var yaml = """
                   opcode: 0x12 0x34
                   """;

        var opcode = YamlSerializer.Deserialize<OpcodeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        opcode.Opcode.Should().Equal("0x12 0x34");
        // Note: R0, R1, RP0, RP1, C0, N0 are not set from YAML but computed elsewhere
        opcode.R0.Should().BeNull();
        opcode.R1.Should().BeNull();
        opcode.RP0.Should().BeNull();
        opcode.RP1.Should().BeNull();
        opcode.C0.Should().BeNull();
        opcode.N0.Should().BeNull();

        // Verify round-trip serialization
        var serializedBytes = YamlSerializer.Serialize(opcode, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);
        serializedYaml.Contains("opcode: 0x12 0x34", StringComparison.Ordinal).Should().BeTrue();
    }

    [Test]
    public void Deserialize_ValidOpcodeWithMinimalProperties()
    {
        var yaml = """
                   opcode: 0x00
                   """;

        var opcode = YamlSerializer.Deserialize<OpcodeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        opcode.Opcode.Should().Equal("0x00");
        opcode.R0.Should().BeNull();
        opcode.R1.Should().BeNull();
        opcode.RP0.Should().BeNull();
        opcode.RP1.Should().BeNull();
        opcode.C0.Should().BeNull();
        opcode.N0.Should().BeNull();
    }

    [Test]
    public void Deserialize_ValidOpcodeWithSingleByte()
    {
        var yaml = """
                   opcode: 0xFF
                   """;

        var opcode = YamlSerializer.Deserialize<OpcodeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        opcode.Opcode.Should().Equal("0xFF");
        opcode.PrefixByte.Should().BeNull();
        opcode.OpcodeByte.Should().Equal(0xFF);
    }

    [Test]
    public void Deserialize_ValidOpcodeWithPrefix()
    {
        var yaml = """
                   opcode: 0xDD 0x46
                   """;

        var opcode = YamlSerializer.Deserialize<OpcodeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        opcode.Opcode.Should().Equal("0xDD 0x46");
        opcode.PrefixByte.Should().Equal(0xDD);
        opcode.OpcodeByte.Should().Equal(0x46);
    }

    [TestCase("0x00", null, 0x00)]
    [TestCase("0x12", null, 0x12)]
    [TestCase("0xFF", null, 0xFF)]
    [TestCase("0xDD 0x00", 0xDD, 0x00)]
    [TestCase("0xFD 0xFF", 0xFD, 0xFF)]
    [TestCase("0xED 0x40", 0xED, 0x40)]
    [TestCase("0xCB 0x06", 0xCB, 0x06)]
    public void PrefixAndOpcodeBytes_ParsedCorrectly(string opcodeString, byte? expectedPrefix, byte expectedOpcode)
    {
        var yaml = $"""
                    opcode: {opcodeString}
                    """;

        var opcode = YamlSerializer.Deserialize<OpcodeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        opcode.PrefixByte.Should().Equal(expectedPrefix);
        opcode.OpcodeByte.Should().Equal(expectedOpcode);
    }

    [Test]
    public void Deserialize_ValidOpcodeWithRegisterOperands()
    {
        var yaml = """
                   opcode: 0x40
                   r0: B
                   r1: C
                   """;

        var opcode = YamlSerializer.Deserialize<OpcodeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        opcode.R0.Should().Equal("B");
        opcode.R1.Should().Equal("C");
        opcode.RP0.Should().BeNull();
        opcode.RP1.Should().BeNull();
        opcode.C0.Should().BeNull();
        opcode.N0.Should().BeNull();
    }

    [Test]
    public void Deserialize_ValidOpcodeWithSingleOpcodeValue()
    {
        var yaml = """
                   opcode: 0x01
                   """;

        var opcode = YamlSerializer.Deserialize<OpcodeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        opcode.Opcode.Should().Equal("0x01");
        opcode.RP0.Should().BeNull();
        opcode.RP1.Should().BeNull();
        opcode.R0.Should().BeNull();
        opcode.R1.Should().BeNull();
    }

    [Test]
    public void Deserialize_ValidOpcodeWithOpcodeOnly()
    {
        var yaml = """
                   opcode: 0x20
                   """;

        var opcode = YamlSerializer.Deserialize<OpcodeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        opcode.Opcode.Should().Equal("0x20");
        opcode.C0.Should().BeNull();
        opcode.N0.Should().BeNull();
    }



    [Test]
    public void ToString_ReturnsOpcodeString()
    {
        var yaml = """
                   opcode: 0xDD 0x21
                   """;

        var opcode = YamlSerializer.Deserialize<OpcodeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        opcode.ToString().Should().Equal("0xDD 0x21");
    }

    [Test]
    public void Deserialize_InvalidOpcodeFormat_ShouldThrow()
    {
        var yaml = """
                   opcode: invalid_hex
                   """;

        AssertThat.Invoking(() =>
        {
            var opcode = YamlSerializer.Deserialize<OpcodeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);
            // Accessing the property to trigger the parsing
            _ = opcode.OpcodeByte;
        }).Should().Throw<Exception>();
    }

    [Test]
    public void Deserialize_InvalidOpcodeFormat_NonHex_ShouldThrow()
    {
        var yaml = """
                   opcode: 0xGG
                   """;

        AssertThat.Invoking(() =>
        {
            var opcode = YamlSerializer.Deserialize<OpcodeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);
            // Accessing the property to trigger the parsing
            _ = opcode.OpcodeByte;
        }).Should().Throw<Exception>();
    }

    [Test]
    public void Deserialize_InvalidNumberValue_TooLarge_ShouldThrow()
    {
        var yaml = """
                   opcode: 0x00
                   n0: 256
                   """;

        AssertThat.Invoking(() => YamlSerializer.Deserialize<OpcodeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance))
            .Should().Throw<Exception>();
    }

    [Test]
    public void Deserialize_InvalidNumberValue_Negative_ShouldThrow()
    {
        var yaml = """
                   opcode: 0x00
                   n0: -1
                   """;

        AssertThat.Invoking(() => YamlSerializer.Deserialize<OpcodeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance))
            .Should().Throw<Exception>();
    }

    [Test]
    public void GetPrefixAndOpcode_CachedCorrectly()
    {
        var yaml = """
                   opcode: 0xED 0x5F
                   """;

        var opcode = YamlSerializer.Deserialize<OpcodeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        // Access the properties multiple times to ensure caching works
        var prefix1 = opcode.PrefixByte;
        var opcodeByte1 = opcode.OpcodeByte;
        var prefix2 = opcode.PrefixByte;
        var opcodeByte2 = opcode.OpcodeByte;

        prefix1.Should().Equal(prefix2);
        opcodeByte1.Should().Equal(opcodeByte2);
        prefix1.Should().Equal(0xED);
        opcodeByte1.Should().Equal(0x5F);
    }
}