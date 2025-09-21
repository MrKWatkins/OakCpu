using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class RegistersClassesGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => RegistersClassesGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public Task GenerateOutput()
    {
        var result = RegistersClassesGenerator.Instance.Generate(Z80GeneratorContext);
        return Verify(result);
    }
}