using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class EmulatorFieldsPropertiesAndConstructorGeneratorTests : TestFixture
{
    [Test]
    public void Generate()
    {
        var classDefinition = EmulatorFieldsPropertiesAndConstructorGenerator.Instance.Generate(Z80GeneratorInput);
    }
}