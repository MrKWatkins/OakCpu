using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class StepStructGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => StepStructGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public Task GenerateOutput()
    {
        var result = StepStructGenerator.Instance.Generate(Z80GeneratorContext);
        return Verify(result);
    }
}