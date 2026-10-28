using MrKWatkins.OakCpu.Definitions.Validation;

namespace MrKWatkins.OakCpu.Definitions.Tests.Validation;

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

    [Test]
    public void Validate_ReturnsErrorsForUnknownDocumentationPlaceholders()
    {
        var yaml = ValidationTestHelper.DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
            interrupts: {}
            functions:
              - name: is_negative
                type: bool
                parameters: [value]
                expression: (value & 0x80) != 0
                documentation: "bit 7 of {value} is set"
                not_documentation: "bit 7 of {val} is reset"
                value_documentation: "whether {value} is {negative}"
            """);

        var errors = FunctionValidation.Validate(yaml.Functions).ToArray();

        errors.Select(error => error.ToString()).Should().SequenceEqual(
            "functions[0].not_documentation: The not_documentation of the function is_negative uses placeholders that are not its parameters: val.",
            "functions[0].value_documentation: The value_documentation of the function is_negative uses placeholders that are not its parameters: negative.");
    }
}