using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class EmulatorInstanceFieldsPropertiesAndConstructorGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => EmulatorInstanceFieldsPropertiesAndConstructorGenerator.Instance.Invoking(g => g.Generate(Z80GeneratorInput)).Should().NotThrow();
}