using MrKWatkins.OakCpu.CodeGenerator.Validation;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Validation;

public sealed class FunctionValidationTests
{
    [Test]
    public void Validate_ReturnsErrorsForDuplicateNames()
    {
        var yaml = ValidationTestHelper.DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
            interrupts: {}
            functions:
              - name: duplicate
                type: u8
                expression: one
              - name: duplicate
                type: u16
                expression: two
            """);

        var errors = FunctionValidation.Validate(yaml.Functions).ToArray();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Equal("The function duplicate is defined multiple times.");
        errors[0].Paths.Should().HaveCount(2);
        errors[0].Paths[0].Should().Equal("functions[0].name");
        errors[0].Paths[1].Should().Equal("functions[1].name");
    }
}