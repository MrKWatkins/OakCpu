using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public sealed class OpcodeYamlNoPrefixFirstComparerTests
{
    [TestCase("0x00", "0x00", 0)]
    [TestCase("0x00", "0x01", -1)]
    [TestCase("0x01", "0x00", 1)]
    [TestCase("0x00", "0x00 0x00", -1)]
    [TestCase("0x00 0x00", "0x00", 1)]
    [TestCase("0x00 0x00", "0x00 0x00", 0)]
    [TestCase("0x00 0x00", "0x01 0x00", -1)]
    [TestCase("0x01 0x00", "0x00 0x00", 1)]
    [TestCase("0x00 0x00", "0x00 0x01", -1)]
    [TestCase("0x00 0x01", "0x00 0x00", 1)]
    public void Compare(string opcodeX, string opcodeY, int expected)
    {
        var x = new OpcodeYaml { Opcode = opcodeX };
        var y = new OpcodeYaml { Opcode = opcodeY };

        OpcodeYamlNoPrefixFirstComparer.Instance.Compare(x, y).Should().Equal(expected);
    }

    [Test]
    public void Compare_SameReference()
    {
        var opcode = new OpcodeYaml { Opcode = "0x00" };

        OpcodeYamlNoPrefixFirstComparer.Instance.Compare(opcode, opcode).Should().Equal(0);
    }

    [Test]
    public void Compare_NullLeft()
    {
        var opcode = new OpcodeYaml { Opcode = "0x00" };

        OpcodeYamlNoPrefixFirstComparer.Instance.Compare(null, opcode).Should().Equal(-1);
    }

    [Test]
    public void Compare_NullRight()
    {
        var opcode = new OpcodeYaml { Opcode = "0x00" };

        OpcodeYamlNoPrefixFirstComparer.Instance.Compare(opcode, null).Should().Equal(1);
    }
}