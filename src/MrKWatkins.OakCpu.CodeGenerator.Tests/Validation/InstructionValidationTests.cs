using MrKWatkins.OakCpu.CodeGenerator.Validation;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Validation;

public sealed class InstructionValidationTests
{
    [Test]
    public void Validate_ReturnsErrorsForDuplicateOpcodes()
    {
        var yaml = ValidationTestHelper.DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
            interrupts: {}
            instructions:
              - group: test
                mnemonic: FIRST
                opcode_table: prefixed
                opcodes:
                  - opcode: 0xCB 0x10
                next_opcode: custom
                steps:
                  - first
              - group: test
                mnemonic: SECOND
                opcode_table: prefixed
                opcodes:
                  - opcode: 0xCB 0x10
                next_opcode: custom
                steps:
                  - second
            """);

        var errors = InstructionValidation.Validate(yaml.Instructions, yaml).ToArray();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Equal("The opcodes in opcode table prefixed are defined multiple times by instructions FIRST, SECOND: 0xCB 0x10");
        errors[0].Paths[0].Should().Equal("instructions[0].opcodes[0].opcode");
        errors[0].Paths[1].Should().Equal("instructions[1].opcodes[0].opcode");
    }

    [Test]
    public void Validate_ReturnsErrorsForInvalidOverlappedSequences()
    {
        var yaml = ValidationTestHelper.DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
            interrupts: {}
            instructions:
              - group: test
                mnemonic: OVERLAP
                opcodes:
                  - opcode: 0x10
                next_opcode: custom
                overlapped_sequence: missing
                steps:
                  - overlap
            """);

        var errors = InstructionValidation.Validate(yaml.Instructions, yaml).ToArray();

        errors.Should().HaveCount(2);
        errors[0].Message.Should().Equal("Instruction OVERLAP specifies an overlapped sequence but does not use next_opcode: overlapped.");
        errors[0].Paths[0].Should().Equal("instructions[0].next_opcode");
        errors[1].Message.Should().Equal("Instruction OVERLAP references unknown overlapped sequence missing.");
        errors[1].Paths[0].Should().Equal("instructions[0].overlapped_sequence");
    }
}