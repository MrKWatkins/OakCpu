using MrKWatkins.OakCpu.CodeGenerator.Validation;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Validation;

public sealed class ValidationHelpersTests
{
    [Test]
    public void ValidateDuplicateNames()
    {
        var errors = ValidationHelpers.ValidateDuplicateNames(
                [("duplicate", "items[1].name"), ("unique", "items[2].name"), ("duplicate", "items[0].name")],
                "item")
            .ToArray();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Equal("The item duplicate is defined multiple times.");
        errors[0].Paths.Should().HaveCount(2);
        errors[0].Paths[0].Should().Equal("items[0].name");
        errors[0].Paths[1].Should().Equal("items[1].name");
    }

    [Test]
    public void ValidateDuplicateValues()
    {
        var errors = ValidationHelpers.ValidateDuplicateValues(
                [(2, "items[1].value"), (1, "items[0].value"), (2, "items[2].value")],
                value => $"The value {value} is defined multiple times.")
            .ToArray();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Equal("The value 2 is defined multiple times.");
        errors[0].Paths.Should().HaveCount(2);
        errors[0].Paths[0].Should().Equal("items[1].value");
        errors[0].Paths[1].Should().Equal("items[2].value");
    }

    [Test]
    public void GetAvailableSequenceNames()
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

        var names = ValidationHelpers.GetAvailableSequenceNames(yaml);

        names.Contains("main").Should().BeTrue();
        names.Contains("opcode_read").Should().BeTrue();
        names.Contains("halted").Should().BeTrue();
    }

    [Test]
    public void Indexed()
    {
        var indexed = new[] { "zero", "one", "two" }.Indexed().ToArray();

        indexed.Should().HaveCount(3);
        indexed[0].Should().Equal(("zero", 0));
        indexed[1].Should().Equal(("one", 1));
        indexed[2].Should().Equal(("two", 2));
    }
}