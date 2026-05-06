using MrKWatkins.OakCpu.CodeGenerator.Validation;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Validation;

public sealed class ActionValidationTests
{
    [Test]
    public void Validate()
    {
        var cpu = ValidationTestHelper.Deserialize<CpuYaml>(
            """
            name: TestCpu
            actions:
              - name: read
              - name: write
            """);

        var errors = ActionValidation.Validate(cpu.Actions).ToArray();

        errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_ReturnsErrorsForBuiltInActionDuplicates()
    {
        var cpu = ValidationTestHelper.Deserialize<CpuYaml>(
            """
            name: TestCpu
            actions:
              - name: none
            """);

        var errors = ActionValidation.Validate(cpu.Actions).ToArray();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Equal("The action none is defined multiple times.");
        errors[0].Paths.Should().HaveCount(2);
        errors[0].Paths[0].Should().Equal("builtin.actions.none");
        errors[0].Paths[1].Should().Equal("cpu.actions[0].name");
    }
}