using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class ActionRequiredGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => ActionRequiredGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public void GenerateContainsEnumDeclaration()
    {
        var result = ActionRequiredGenerator.Instance.Generate(Z80GeneratorContext);

        result.Contains("public enum ActionRequired", StringComparison.Ordinal).Should().BeTrue();
    }
}