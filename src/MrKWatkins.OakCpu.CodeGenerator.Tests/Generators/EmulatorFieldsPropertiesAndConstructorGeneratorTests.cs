using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class EmulatorFieldsPropertiesAndConstructorGeneratorTests : TextFixture
{
    [Test]
    public void Generate()
    {
        var classDefinition = EmulatorFieldsPropertiesAndConstructorGenerator.Instance.Generate(Z80GeneratorInput);
    }
}