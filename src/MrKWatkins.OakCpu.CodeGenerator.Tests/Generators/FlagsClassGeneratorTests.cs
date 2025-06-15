using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class FlagsClassGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => FlagsClassGenerator.Instance.Invoking(g => g.Generate(Z80GeneratorContext)).Should().NotThrow();
}