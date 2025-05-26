namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public sealed class GeneratorInputTests : TestFixture
{
    [Test]
    public void Create()
    {
        var generatorInput = GeneratorInput.Create("MrKWatkins.OakCpu.Z80", Z80Yaml);
        generatorInput.Cpu.Name.Should().Be("Z80");
    }
}