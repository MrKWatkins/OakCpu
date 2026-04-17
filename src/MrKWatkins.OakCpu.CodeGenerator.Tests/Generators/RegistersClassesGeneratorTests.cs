using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class RegistersClassesGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => RegistersClassesGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public Task GenerateOutput()
    {
        var result = string.Join(
            Environment.NewLine + Environment.NewLine,
            RegistersClassesGenerator.Instance.GenerateFiles(Z80GeneratorContext)
                .OrderBy(file => file.FileName)
                .Select(file => $"=== {file.FileName} ==={Environment.NewLine}{file.Source}"));
        return Verify(result);
    }
}