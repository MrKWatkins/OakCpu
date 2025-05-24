using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class FlagsClassGeneratorTests : TextFixture
{
    [Test]
    public void Generate()
    {
        var flagsClass = FlagsClassGenerator.Instance.Generate(Z80GeneratorInput);
    }
}