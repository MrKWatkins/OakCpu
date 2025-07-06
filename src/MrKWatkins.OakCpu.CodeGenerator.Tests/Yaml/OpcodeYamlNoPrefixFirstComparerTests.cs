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
}