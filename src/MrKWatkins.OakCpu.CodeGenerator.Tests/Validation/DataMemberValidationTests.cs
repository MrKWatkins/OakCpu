using MrKWatkins.OakCpu.CodeGenerator.Validation;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Validation;

public sealed class DataMemberValidationTests
{
    [Test]
    public void Validate_ReturnsErrorsForDuplicateNames()
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
            """);

        var errors = DataMemberValidation.Validate(yaml.Cpu.Fields, yaml.Interrupts.Properties).ToArray();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Equal("The data member shared is defined multiple times.");
        errors[0].Paths.Should().HaveCount(2);
        errors[0].Paths[0].Should().Equal("cpu.fields[0].name");
        errors[0].Paths[1].Should().Equal("interrupts.properties[0].name");
    }
}