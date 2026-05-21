using MrKWatkins.OakCpu.CodeGenerator.Validation;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Validation;

public sealed class DefinitionValidationTests
{
    [Test]
    public void Validate()
    {
        var yaml = ValidationTestHelper.DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
            interrupts: {}
            opcode_read:
              - fetch
            """);

        AssertThat.Invoking(() => DefinitionValidation.Validate(yaml)).Should().NotThrow();
    }

    [Test]
    public void Validate_ThrowsGroupedValidationErrors()
    {
        var yaml = ValidationTestHelper.DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
              fields:
                - name: shared
                  type: u8
            interrupts:
              properties:
                - name: shared
                  type: u8
            opcode_read:
              - fetch
            functions:
              - name: duplicate
                type: u8
                expression: one
              - name: duplicate
                type: u8
                expression: two
            """);

        var exception = Assert.Throws<InvalidOperationException>(() => DefinitionValidation.Validate(yaml));

        Assert.That(exception!.Message, Does.StartWith("Definition validation failed:"));
        Assert.That(exception.Message, Does.Contain("- cpu.fields[0].name, interrupts.properties[0].name: The data member shared is defined multiple times."));
        Assert.That(exception.Message, Does.Contain("- functions[0].name, functions[1].name: The function duplicate is defined multiple times."));
    }
}