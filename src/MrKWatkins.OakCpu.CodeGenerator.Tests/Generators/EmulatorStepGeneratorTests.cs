using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class EmulatorStepGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => EmulatorStepGenerator.Instance.Invoking(g => g.Generate(Z80GeneratorContext)).Should().NotThrow();
}