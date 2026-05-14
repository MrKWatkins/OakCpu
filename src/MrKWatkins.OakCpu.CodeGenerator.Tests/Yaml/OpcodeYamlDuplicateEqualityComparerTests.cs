using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public sealed class OpcodeYamlDuplicateEqualityComparerTests
{
    [TestCaseSource(nameof(TestCases))]
    public void Equals(OpcodeYaml x, OpcodeYaml y, bool equal) => OpcodeYamlDuplicateEqualityComparer.Instance.Equals(x, y).Should().Equal(equal);

    [Test]
    public void Equals_SameReference()
    {
        var opcode = new OpcodeYaml { Opcode = "0x00" };

        OpcodeYamlDuplicateEqualityComparer.Instance.Equals(opcode, opcode).Should().BeTrue();
    }

    [Test]
    public void Equals_NullLeft()
    {
        OpcodeYamlDuplicateEqualityComparer.Instance.Equals(null, new OpcodeYaml { Opcode = "0x00" }).Should().BeFalse();
    }

    [Test]
    public void Equals_NullRight()
    {
        OpcodeYamlDuplicateEqualityComparer.Instance.Equals(new OpcodeYaml { Opcode = "0x00" }, null).Should().BeFalse();
    }

    [Test]
    public void GetHashCode_EquivalentOpcodesMatch()
    {
        var x = new OpcodeYaml { Opcode = "0xED 0x00", R0 = "X", N0 = 1 };
        var y = new OpcodeYaml { Opcode = "0xED 0x01", R0 = "X", N0 = 1 };

        OpcodeYamlDuplicateEqualityComparer.Instance.GetHashCode(x).Should().Equal(OpcodeYamlDuplicateEqualityComparer.Instance.GetHashCode(y));
    }

    [Pure]
    public static IEnumerable<TestCaseData> TestCases()
    {
        yield return CreateTestCase("Prefixed and non-prefixed", false, "0x00", "0xED 0x00");
        yield return CreateTestCase("Both non-prefixed", true, "0x00", "0x01");
        yield return CreateTestCase("Different prefix", true, "0xFD 0x00", "0xED 0x00");
        yield return CreateTestCase("Different R0", false, "0xED 0x00", "0xED 0x01", xR0: "X", yR0: "Y");
        yield return CreateTestCase("Different R1", false, "0xED 0x00", "0xED 0x01", xR1: "X", yR1: "Y");
        yield return CreateTestCase("Different RP0", false, "0xED 0x00", "0xED 0x01", xRP0: "X", yRP0: "Y");
        yield return CreateTestCase("Different RP1", false, "0xED 0x00", "0xED 0x01", xRP1: "X", yRP1: "Y");
        yield return CreateTestCase("Different C0", false, "0xED 0x00", "0xED 0x01", xC0: "X", yC0: "Y");
        yield return CreateTestCase("Different N0", false, "0xED 0x00", "0xED 0x01", xN0: 123, yN0: 234);
    }

    [Pure]
    private static TestCaseData CreateTestCase(
        string name, bool expected,
        string xOpcode, string yOpcode,
        string? xR0 = null, string? yR0 = null,
        string? xR1 = null, string? yR1 = null,
        string? xRP0 = null, string? yRP0 = null,
        string? xRP1 = null, string? yRP1 = null,
        string? xC0 = null, string? yC0 = null,
        byte? xN0 = null, byte? yN0 = null)
    {
        var x = new OpcodeYaml { Opcode = xOpcode, R0 = xR0, R1 = xR1, RP0 = xRP0, RP1 = xRP1, C0 = xC0, N0 = xN0 };
        var y = new OpcodeYaml { Opcode = yOpcode, R0 = yR0, R1 = yR1, RP0 = yRP0, RP1 = yRP1, C0 = yC0, N0 = yN0 };

        return new TestCaseData(x, y, expected).SetArgDisplayNames(name);
    }
}