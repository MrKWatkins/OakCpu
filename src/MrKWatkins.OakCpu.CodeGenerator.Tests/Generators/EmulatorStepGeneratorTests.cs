using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class EmulatorStepGeneratorTests : TestFixture
{
    [Test]
    public void Generate()
    {
        var classDefinition = EmulatorStepGenerator.Instance.Generate(Z80GeneratorInput);
    }
}