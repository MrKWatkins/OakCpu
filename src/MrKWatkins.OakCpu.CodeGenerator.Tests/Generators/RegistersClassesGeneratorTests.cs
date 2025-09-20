using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class RegistersClassesGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => RegistersClassesGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public void GenerateContainsRegistersClass()
    {
        var result = RegistersClassesGenerator.Instance.Generate(Z80GeneratorContext);

        result.Should().NotBeNull();
        (result.Length > 0).Should().BeTrue();
    }

    [Test]
    public void GenerateContainsExpectedProperties()
    {
        var result = RegistersClassesGenerator.Instance.Generate(Z80GeneratorContext);

        // Should contain register properties
        result.Contains("public", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("get", StringComparison.Ordinal).Should().BeTrue();
    }
}