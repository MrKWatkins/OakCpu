using MrKWatkins.OakCpu.Definitions.Validation;

namespace MrKWatkins.OakCpu.Definitions.Tests.Validation;

public sealed class CpuValidationTests
{
    [Test]
    public void Validate()
    {
        var yaml = ValidationTestHelper.DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
              cycles:
                name: Cycles
                count: actions
                minimum: 2
              operands:
                - name: n
                - name: nn
                  bytes: 2
              references:
                - name: Manual
                  url: https://example.com/manual.pdf
            interrupts: {}
            """);

        var errors = CpuValidation.Validate(yaml.Cpu).ToArray();

        errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_ReturnsErrorsForInvalidCyclesOperandsAndReferences()
    {
        var yaml = ValidationTestHelper.DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
              cycles:
                name: " "
                minimum: 0
              operands:
                - name: n
                - name: n
                  bytes: 0
                - name: "1"
              references:
                - name: " "
                  url: not a url
            interrupts: {}
            """);

        var errors = CpuValidation.Validate(yaml.Cpu).ToArray();

        errors.Select(error => error.ToString()).Should().SequenceEqual(
            "cpu.cycles.name: The cycles have no name.",
            "cpu.cycles.minimum: The minimum cycles 0 is less than one.",
            "cpu.operands[0].name, cpu.operands[1].name: The operand n is defined multiple times.",
            "cpu.operands[1].bytes: The operand n has 0 bytes, which is less than one.",
            "cpu.operands[2].name: The operand name 1 is not one or more letters.",
            "cpu.references[0].name: The reference has no name.",
            "cpu.references[0].url: The reference   has the invalid URL not a url.");
    }
}