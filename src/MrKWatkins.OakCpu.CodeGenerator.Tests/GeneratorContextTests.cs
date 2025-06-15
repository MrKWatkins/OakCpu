using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public sealed class GeneratorContextTests : TestFixture
{
    [Test]
    public void Create()
    {
        var generatorInput = GeneratorContext.Create("MrKWatkins.OakCpu.Z80", Z80Yaml);
        generatorInput.Cpu.Name.Should().Be("Z80");
        generatorInput.OpcodeRead.Should().HaveCount(4);
    }
}