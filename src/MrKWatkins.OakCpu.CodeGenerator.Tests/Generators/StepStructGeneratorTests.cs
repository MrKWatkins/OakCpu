using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class StepStructGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => StepStructGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public void GenerateContainsStructDeclaration()
    {
        var result = StepStructGenerator.Instance.Generate(Z80GeneratorContext);

        result.Contains("internal unsafe readonly struct Step", StringComparison.Ordinal).Should().BeTrue();
    }

    [Test]
    public void GenerateContainsExpectedFields()
    {
        var result = StepStructGenerator.Instance.Generate(Z80GeneratorContext);

        result.Contains("internal readonly", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("Handler", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("NextStep", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("ActionRequired", StringComparison.Ordinal).Should().BeTrue();
    }
}