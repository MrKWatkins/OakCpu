using MrKWatkins.OakCpu.CodeGenerator.Validation;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Validation;

public sealed class SequenceValidationTests
{
    [Test]
    public void Validate()
    {
        var yaml = ValidationTestHelper.DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
            interrupts:
              halted_cycle:
                - halt
            opcode_read:
              - fetch
            sequences:
              - name: main
                next_opcode: read
            """);

        var errors = SequenceValidation.Validate(yaml).ToArray();

        errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_ReturnsErrorsForDuplicateNamesGroupsAndInterruptModes()
    {
        var yaml = ValidationTestHelper.DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
            interrupts:
              halted_cycle:
                - halt
              modes:
                - number: 2
                  next_opcode: read
                - number: 2
                  next_opcode: custom
                - number: 3
                  next_opcode: custom
                  sequence: missing
            opcode_read:
              - fetch
            sequences:
              - name: duplicate
                next_opcode: read
              - name: duplicate
                next_opcode: custom
              - name: grouped_one
                next_opcode: read
                group:
                  name: grouped
                  number: 1
              - name: grouped_two
                next_opcode: custom
                group:
                  name: grouped
                  number: 1
            """);

        var errors = SequenceValidation.Validate(yaml).ToArray();

        errors.Should().HaveCount(4);

        errors[0].Message.Should().Equal("The sequence duplicate is defined multiple times.");
        errors[0].Paths[0].Should().Equal("sequences[0].name");
        errors[0].Paths[1].Should().Equal("sequences[1].name");

        errors[1].Message.Should().Equal("The sequence group grouped contains multiple sequences for number 1: grouped_one, grouped_two.");
        errors[1].Paths[0].Should().Equal("sequences[2].group.number");
        errors[1].Paths[1].Should().Equal("sequences[3].group.number");

        errors[2].Message.Should().Equal("Interrupt mode 2 is defined multiple times.");
        errors[2].Paths[0].Should().Equal("interrupts.modes[0].number");
        errors[2].Paths[1].Should().Equal("interrupts.modes[1].number");

        errors[3].Message.Should().Equal("No sequence named missing exists for interrupt mode 3.");
        errors[3].Paths[0].Should().Equal("interrupts.modes[2].sequence");
    }
}