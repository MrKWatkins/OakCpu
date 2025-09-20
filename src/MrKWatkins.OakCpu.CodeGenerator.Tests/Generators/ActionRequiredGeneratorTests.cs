using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class ActionRequiredGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => ActionRequiredGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public Task GenerateOutput()
    {
        var result = ActionRequiredGenerator.Instance.Generate(Z80GeneratorContext);
        return Verify(result);
    }
}