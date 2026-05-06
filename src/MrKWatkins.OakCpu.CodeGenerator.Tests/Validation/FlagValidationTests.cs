using MrKWatkins.OakCpu.CodeGenerator.Validation;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Validation;

public sealed class FlagValidationTests
{
    [Test]
    public void Validate()
    {
        var yaml = ValidationTestHelper.DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
            interrupts: {}
            flags:
              - name: carry
                index: 0
                condition: c
                not_condition: nc
              - name: zero
                index: 1
                condition: z
                not_condition: nz
            """);

        var errors = FlagValidation.Validate(yaml.Flags).ToArray();

        errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_ReturnsErrorsForDuplicateNamesIndexesAndConditions()
    {
        var yaml = ValidationTestHelper.DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
            interrupts: {}
            flags:
              - name: carry
                index: 0
                condition: c
              - name: carry
                index: 0
                not_condition: c
            """);

        var errors = FlagValidation.Validate(yaml.Flags).ToArray();

        errors.Should().HaveCount(3);

        errors[0].Message.Should().Equal("The flag carry is defined multiple times.");
        errors[0].Paths[0].Should().Equal("flags[0].name");
        errors[0].Paths[1].Should().Equal("flags[1].name");

        errors[1].Message.Should().Equal("The flag index 0 is defined multiple times by flags carry.");
        errors[1].Paths[0].Should().Equal("flags[0].index");
        errors[1].Paths[1].Should().Equal("flags[1].index");

        errors[2].Message.Should().Equal("The condition c is defined multiple times by flags carry.");
        errors[2].Paths[0].Should().Equal("flags[0].condition");
        errors[2].Paths[1].Should().Equal("flags[1].not_condition");
    }
}