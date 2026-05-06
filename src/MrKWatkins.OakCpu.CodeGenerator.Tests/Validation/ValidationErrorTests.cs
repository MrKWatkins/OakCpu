using MrKWatkins.OakCpu.CodeGenerator.Validation;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Validation;

public sealed class ValidationErrorTests
{
    [Test]
    public void ToString_NoPaths()
    {
        var error = new ValidationError("Broken.");

        error.ToString().Should().Equal("Broken.");
    }

    [Test]
    public void ToString_WithSinglePath()
    {
        var error = new ValidationError("Broken.", "interrupts.modes[0].sequence");

        error.ToString().Should().Equal("interrupts.modes[0].sequence: Broken.");
    }

    [Test]
    public void ToString_WithMultiplePaths()
    {
        var error = new ValidationError("Broken.", ["path[0]", "path[1]"]);

        error.ToString().Should().Equal("path[0], path[1]: Broken.");
    }
}