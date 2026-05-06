using MrKWatkins.OakCpu.CodeGenerator.Validation;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Validation;

public sealed class RegisterValidationTests
{
    [Test]
    public void Validate_ReturnsErrorsForInvalidTypes()
    {
        var yaml = ValidationTestHelper.DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
            interrupts: {}
            registers:
              - name: invalid
                type: bool
            """);

        var errors = RegisterValidation.Validate(yaml.Registers).ToArray();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Equal("Register invalid must have type u8 or u16.");
        errors[0].Paths.Should().HaveCount(1);
        errors[0].Paths[0].Should().Equal("registers[0].type");
    }

    [Test]
    public void Validate_ReturnsErrorsForInvalidSubRegisterTypes()
    {
        var yaml = ValidationTestHelper.DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
            interrupts: {}
            registers:
              - name: pair
                type: u16
                high:
                  name: high
                  type: u16
            """);

        var errors = RegisterValidation.Validate(yaml.Registers).ToArray();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Equal("High register high of register pair must have type u8.");
        errors[0].Paths.Should().HaveCount(1);
        errors[0].Paths[0].Should().Equal("registers[0].high.type");
    }

    [Test]
    public void Validate_ReturnsErrorsForImplicitRegisterNameDuplicates()
    {
        var yaml = ValidationTestHelper.DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
            interrupts: {}
            registers:
              - name: pair
                type: u16
              - name: pairH
                type: u8
            """);

        var errors = RegisterValidation.Validate(yaml.Registers).ToArray();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Equal("The register pairH is defined multiple times.");
        errors[0].Paths.Should().HaveCount(2);
        errors[0].Paths[0].Should().Equal("registers[0].name (implicit high register)");
        errors[0].Paths[1].Should().Equal("registers[1].name");
    }
}