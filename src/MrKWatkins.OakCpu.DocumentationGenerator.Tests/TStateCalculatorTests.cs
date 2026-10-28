using MrKWatkins.OakAsm.Ast.Resolution.Opcodes;

namespace MrKWatkins.OakCpu.DocumentationGenerator.Tests;

public sealed class TStateCalculatorTests : TestFixture
{
    [TestCase("NOP", 4)]
    [TestCase("LD B, n", 7)]
    [TestCase("LD B, (IX + d)", 19)]
    [TestCase("LD (nn), A", 13)]
    [TestCase("ADD A, B", 4)]
    [TestCase("ADD HL, BC", 11)]
    [TestCase("ADD IX, BC", 15)]
    [TestCase("INC BC", 6)]
    [TestCase("EX AF, AF'", 4)]
    [TestCase("HALT", 4)]
    [TestCase("JP HL", 4)]
    [TestCase("JP IX", 8)]
    [TestCase("EI", 4)]
    [TestCase("DI", 4)]
    [TestCase("LD A, I", 9)]
    [TestCase("IN (C)", 12)]
    [TestCase("OUT (C), 0", 12)]
    [TestCase("SLL B", 8)]
    [TestCase("RLC (IX + d)", 23)]
    [TestCase("RLC (IX + d), B", 23)]
    [TestCase("BIT 0x00, (IX + d)", 20)]
    [TestCase("CPI", 16)]
    [TestCase("RLD", 18)]
    public void Calculate(string mnemonic, int expected)
    {
        var calculator = new TStateCalculator(Z80Model);

        calculator.Calculate(GetInstruction(Z80Model, mnemonic)).Should().SequenceEqual(expected);
    }

    [TestCase("JR NZ, d", 7, 12)]
    [TestCase("DJNZ d", 8, 13)]
    [TestCase("CALL NZ, nn", 10, 17)]
    [TestCase("RET NZ", 5, 11)]
    [TestCase("LDIR", 16, 21)]
    [TestCase("CPIR", 16, 21)]
    [TestCase("INIR", 16, 21)]
    public void Calculate_EarlyExit(string mnemonic, int expectedEarly, int expectedFull)
    {
        var calculator = new TStateCalculator(Z80Model);

        calculator.Calculate(GetInstruction(Z80Model, mnemonic)).Should().SequenceEqual(expectedEarly, expectedFull);
    }

    [TestCase("NEXTREG $n, $n", 20)]
    [TestCase("NEXTREG $n, A", 17)]
    [TestCase("MUL D, E", 8)]
    [TestCase("LDIX", 16)]
    [TestCase("JP (C)", 13)]
    public void Calculate_Z80N(string mnemonic, int expected)
    {
        var calculator = new TStateCalculator(Z80NModel);

        calculator.Calculate(GetInstruction(Z80NModel, mnemonic)).Should().SequenceEqual(expected);
    }

    [Test]
    public void Calculate_Z80N_EarlyExit()
    {
        var calculator = new TStateCalculator(Z80NModel);

        calculator.Calculate(GetInstruction(Z80NModel, "LDIRX")).Should().SequenceEqual(16, 21);
    }

    [Test]
    public void IsRedirect()
    {
        TStateCalculator.IsRedirect(GetInstruction(Z80Model, "DDCB Redirect")).Should().BeTrue();
        TStateCalculator.IsRedirect(GetInstruction(Z80Model, "DDED Redirect")).Should().BeTrue();
        TStateCalculator.IsRedirect(GetInstruction(Z80Model, "NOP")).Should().BeFalse();
    }

    /// <summary>
    /// OakAsm's opcode tables carry T-states that were entered by hand from the Zilog manual; every opcode it knows should
    /// agree with what the definitions say.
    /// </summary>
    [Test]
    public void Calculate_AgreesWithOakAsm_Z80() => AssertAgreesWithOakAsm(Z80Model, OakAsm.Z80.Z80Assembly.OpcodeDefinitions);

    [Test]
    public void Calculate_AgreesWithOakAsm_Z80N() => AssertAgreesWithOakAsm(Z80NModel, OakAsm.Z80N.Z80NAssembly.OpcodeDefinitions);

    private static void AssertAgreesWithOakAsm(CpuModel model, IEnumerable<OpcodeDefinition> oakAsmOpcodes)
    {
        var calculator = new TStateCalculator(model);
        var instructions = model.Instructions
            .SelectMany(instruction => instruction.Duplicates
                .Select(duplicate => (Key: (instruction.OpcodeTable, duplicate.Prefix, duplicate.Opcode), Instruction: instruction))
                .Prepend((Key: (instruction.OpcodeTable, instruction.Prefix, instruction.Opcode), Instruction: instruction)))
            .ToDictionary(x => x.Key, x => x.Instruction);

        var mismatches = new List<string>();
        foreach (var definition in oakAsmOpcodes.SelectMany(o => o.Alternatives.Prepend(o)))
        {
            var key = ToKey(definition.InstructionTemplate);
            var template = string.Join(" ", definition.InstructionTemplate.Select(b => b.HasValue ? $"{b:X2}" : "??"));
            if (!instructions.TryGetValue(key, out var instruction))
            {
                mismatches.Add($"{template} {definition.Name} is not defined.");
                continue;
            }

            if (definition.Name == "NOP" && instruction.Mnemonic != "NOP")
            {
                // OakAsm treats unused prefixed opcodes as NOPs; the definitions can give them instructions OakAsm does not know.
                continue;
            }

            var expected = new[] { (int)definition.TStates1, definition.TStates2 ?? definition.TStates1 }.Distinct().Order().ToList();
            var actual = calculator.Calculate(instruction);
            if (!expected.SequenceEqual(actual))
            {
                mismatches.Add($"{template} {definition.Name}: OakAsm {string.Join("/", expected)}, definitions {instruction.Mnemonic} {string.Join("/", actual)}.");
            }
        }

        string.Join(Environment.NewLine, mismatches).Should().Equal("");
    }

    [Pure]
    private static (string? Table, byte? Prefix, byte Opcode) ToKey(IReadOnlyList<byte?> template)
    {
        // Instruction templates have null for operand bytes. The DD CB and FD CB tables have the displacement before the opcode.
        if (template.Count == 4 && template[0] is 0xDD or 0xFD && template[1] == 0xCB && template[2] == null)
        {
            return ($"{template[0]:X2}CB", null, template[3]!.Value);
        }

        var bytes = template.TakeWhile(b => b != null).Select(b => b!.Value).ToList();
        return bytes.Count >= 2 && bytes[0] is 0xCB or 0xDD or 0xED or 0xFD
            ? (null, bytes[0], bytes[1])
            : (null, null, bytes[0]);
    }
}