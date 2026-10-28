using MrKWatkins.OakAsm.Ast.Resolution.Opcodes;
using MrKWatkins.OakCpu.M6502;

namespace MrKWatkins.OakCpu.DocumentationGenerator.Tests;

public sealed class CycleCalculatorTests : TestFixture
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
        var calculator = new CycleCalculator(Z80Model);

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
        var calculator = new CycleCalculator(Z80Model);

        calculator.Calculate(GetInstruction(Z80Model, mnemonic)).Should().SequenceEqual(expectedEarly, expectedFull);
    }

    [TestCase("NEXTREG $n, $n", 20)]
    [TestCase("NEXTREG $n, A", 17)]
    [TestCase("MUL D, E", 8)]
    [TestCase("LDIX", 16)]
    [TestCase("JP (C)", 13)]
    public void Calculate_Z80N(string mnemonic, int expected)
    {
        var calculator = new CycleCalculator(Z80NModel);

        calculator.Calculate(GetInstruction(Z80NModel, mnemonic)).Should().SequenceEqual(expected);
    }

    [Test]
    public void Calculate_Z80N_EarlyExit()
    {
        var calculator = new CycleCalculator(Z80NModel);

        calculator.Calculate(GetInstruction(Z80NModel, "LDIRX")).Should().SequenceEqual(16, 21);
    }

    [TestCase("NOP", 2)]
    [TestCase("CLC", 2)]
    [TestCase("TAX", 2)]
    [TestCase("ASL A", 2)]
    [TestCase("LDA #n", 2)]
    [TestCase("LDA zp", 3)]
    [TestCase("LDA zp,X", 4)]
    [TestCase("LDA abs", 4)]
    [TestCase("LDA (zp,X)", 6)]
    [TestCase("STA abs,X", 5)]
    [TestCase("STA (zp),Y", 6)]
    [TestCase("ASL zp", 5)]
    [TestCase("INC abs,X", 7)]
    [TestCase("PHA", 3)]
    [TestCase("PLA", 4)]
    [TestCase("JMP abs", 3)]
    [TestCase("JMP (abs)", 5)]
    [TestCase("JSR abs", 6)]
    [TestCase("RTS", 6)]
    [TestCase("RTI", 6)]
    [TestCase("BRK", 7)]
    [TestCase("SLO (zp),Y", 8)]
    public void Calculate_M6502(string mnemonic, int expected)
    {
        var calculator = new CycleCalculator(M6502Model);

        calculator.Calculate(GetInstruction(M6502Model, mnemonic)).Should().SequenceEqual(expected);
    }

    [TestCase("LDA abs,X", 4, 5)]
    [TestCase("LDA (zp),Y", 5, 6)]
    [TestCase("NOP abs,X", 4, 5)]
    public void Calculate_M6502_PageCross(string mnemonic, int expectedSamePage, int expectedPageCross)
    {
        var calculator = new CycleCalculator(M6502Model);

        calculator.Calculate(GetInstruction(M6502Model, mnemonic)).Should().SequenceEqual(expectedSamePage, expectedPageCross);
    }

    [Test]
    public void Calculate_M6502_Branch()
    {
        var calculator = new CycleCalculator(M6502Model);

        calculator.Calculate(GetInstruction(M6502Model, "BPL rel")).Should().SequenceEqual(2, 3, 4);
    }

    /// <summary>
    /// Every opcode should take the cycles the step emulator does, counting the bus accesses it makes. The scenarios cover
    /// indexing that does and does not cross a page, and branches taken and not taken.
    /// </summary>
    [Test]
    public void Calculate_AgreesWithEmulator_M6502()
    {
        var calculator = new CycleCalculator(M6502Model);
        var scenarios = new (byte Operand, byte Index, byte Flags)[] { (0x00, 0x00, 0x00), (0x00, 0x00, 0xC3), (0x7F, 0xFF, 0x00), (0x7F, 0xFF, 0xC3) };

        var mismatches = new List<string>();
        for (var opcode = 0; opcode < 256; opcode++)
        {
            var instruction = GetInstruction(M6502Model, null, (byte)opcode);
            var expected = calculator.Calculate(instruction);
            var actual = scenarios.Select(scenario => CountCycles((byte)opcode, scenario.Operand, scenario.Index, scenario.Flags)).Distinct().Order().ToList();
            if (!expected.SequenceEqual(actual))
            {
                mismatches.Add($"{opcode:X2} {instruction.Mnemonic}: definitions {string.Join("/", expected)}, emulator {string.Join("/", actual)}.");
            }
        }

        string.Join(Environment.NewLine, mismatches).Should().Equal("");
    }

    /// <summary>
    /// Runs an opcode on the step emulator with every other byte of memory set to the operand, counting the bus accesses.
    /// The dummy read of an implied instruction is the fetch of the next opcode, which the emulator runs and then undoes.
    /// </summary>
    [Pure]
    private static int CountCycles(byte opcode, byte operand, byte index, byte flags)
    {
        var memory = new byte[65536];
        Array.Fill(memory, operand);
        memory[0x02F0] = opcode;

        var emulator = new M6502StepEmulator();
        emulator.Reset();
        emulator.Registers.PC = 0x02F0;
        emulator.Registers.X = index;
        emulator.Registers.Y = index;
        emulator.Registers.P = flags;
        emulator.Registers.S = 0xFD;

        var cycles = 0;
        emulator.ExecuteInstruction(action =>
        {
            switch (action)
            {
                case ActionRequired.OpcodeRead:
                case ActionRequired.MemoryRead:
                    emulator.Data = memory[emulator.Address];
                    cycles++;
                    break;

                case ActionRequired.MemoryWrite:
                    memory[emulator.Address] = emulator.Data;
                    cycles++;
                    break;
            }
        });

        return cycles;
    }

    [Test]
    public void IsRedirect()
    {
        CycleCalculator.IsRedirect(GetInstruction(Z80Model, "DDCB Redirect")).Should().BeTrue();
        CycleCalculator.IsRedirect(GetInstruction(Z80Model, "DDED Redirect")).Should().BeTrue();
        CycleCalculator.IsRedirect(GetInstruction(Z80Model, "NOP")).Should().BeFalse();
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
        var calculator = new CycleCalculator(model);
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
        if (template is [0xDD or 0xFD, 0xCB, null, _])
        {
            return ($"{template[0]:X2}CB", null, template[3]!.Value);
        }

        var bytes = template.TakeWhile(b => b != null).Select(b => b!.Value).ToList();
        return bytes is [0xCB or 0xDD or 0xED or 0xFD, _, ..]
            ? (null, bytes[0], bytes[1])
            : (null, null, bytes[0]);
    }
}