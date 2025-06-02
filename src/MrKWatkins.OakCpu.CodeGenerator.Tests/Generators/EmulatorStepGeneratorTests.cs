using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class EmulatorStepGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => EmulatorFieldsPropertiesAndConstructorGenerator.Instance.Invoking(g => g.Generate(Z80GeneratorInput)).Should().NotThrow();
}