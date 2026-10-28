namespace MrKWatkins.OakCpu.DocumentationGenerator.Tests;

public sealed class OpcodeEncodingTests : TestFixture
{
    [TestCase("NOP", "NOP")]
    [TestCase("LD R0, n", "n")]
    [TestCase("LD RP0, nn", "nn")]
    [TestCase("LD (RP0 + d), n", "d n")]
    [TestCase("JR C0, d", "d")]
    [TestCase("NEXTREG $n, $n", "n n")]
    [TestCase("PUSH $nn", "nn")]
    [TestCase("DJNZ d", "d")]
    public void OperandTokens(string mnemonic, string expected) =>
        string.Join(' ', OpcodeEncoding.OperandTokens(mnemonic)).Should().Equal(mnemonic == "NOP" ? "" : expected);

    [TestCase("NOP", null, 0x00, "00")]
    [TestCase("LD R0, n", null, 0x06, "06 n")]
    [TestCase("LD RP0, nn", null, 0x01, "01 nn")]
    [TestCase("LD (nn), A", null, 0x32, "32 nn")]
    [TestCase("LD (RP0 + d), n", 0xDD, 0x36, "DD 36 d n")]
    [TestCase("LD R0, (RP0 + d)", 0xFD, 0x46, "FD 46 d")]
    [TestCase("ADD RP0, RP1", 0xDD, 0x09, "DD 09")]
    [TestCase("JR C0, d", null, 0x20, "20 d")]
    [TestCase("IN A, (n)", null, 0xDB, "DB n")]
    [TestCase("IN R0, (C)", 0xED, 0x40, "ED 40")]
    [TestCase("NOP", 0xDD, 0x00, "DD 00")]
    public void Format(string form, int? prefix, int opcode, string expected)
    {
        var encoding = new OpcodeEncoding(Z80Model);
        var instruction = GetInstruction(Z80Model, (byte?)prefix, (byte)opcode);

        encoding.Format(form, instruction, (byte?)prefix, (byte)opcode).Should().Equal(expected);
    }

    [TestCase("RLC (IX + d)", "DDCB", 0x06, "DD CB d 06")]
    [TestCase("RLC (IX + d), R0", "DDCB", 0x00, "DD CB d 00")]
    [TestCase("BIT N0, (IY + d)", "FDCB", 0x46, "FD CB d 46")]
    public void Format_OpcodeTable(string form, string table, int opcode, string expected)
    {
        var encoding = new OpcodeEncoding(Z80Model);
        var instruction = GetInstruction(Z80Model, null, (byte)opcode, table);

        encoding.Format(form, instruction, null, (byte)opcode).Should().Equal(expected);
    }

    [TestCase("NEXTREG $n, $n", 0x91, "ED 91 n n")]
    [TestCase("NEXTREG $n, A", 0x92, "ED 92 n")]
    [TestCase("PUSH $nn", 0x8A, "ED 8A nn")]
    [TestCase("TEST $n", 0x27, "ED 27 n")]
    public void Format_Z80N(string form, int opcode, string expected)
    {
        var encoding = new OpcodeEncoding(Z80NModel);
        var instruction = GetInstruction(Z80NModel, 0xED, (byte)opcode);

        encoding.Format(form, instruction, 0xED, (byte)opcode).Should().Equal(expected);
    }

    [Test]
    public void Format_ThrowsIfTheMnemonicDoesNotNameTheOperandBytes()
    {
        var encoding = new OpcodeEncoding(Z80Model);
        var instruction = GetInstruction(Z80Model, null, 0x06);

        encoding.Invoking(e => e.Format("LD R0", instruction, null, 0x06)).Should().Throw<InvalidOperationException>();
    }
}