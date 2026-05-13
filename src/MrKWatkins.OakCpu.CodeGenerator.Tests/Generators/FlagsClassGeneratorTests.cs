using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class FlagsClassGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => FlagsClassGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public Task GenerateOutput()
    {
        var result = string.Join(
            Environment.NewLine + Environment.NewLine,
            FlagsClassGenerator.Instance.GenerateFiles(Z80GeneratorContext)
                .OrderBy(file => file.FileName)
                .Select(file => $"=== {file.FileName} ==={Environment.NewLine}{file.Source}"));
        return Verify(result);
    }
}