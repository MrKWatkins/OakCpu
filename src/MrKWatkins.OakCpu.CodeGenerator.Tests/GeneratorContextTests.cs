using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public sealed class GeneratorContextTests : TestFixture
{
    [Test]
    public void Create()
    {
        var generatorInput = GeneratorContext.Create("MrKWatkins.OakCpu.Z80", Z80Yaml);
        generatorInput.Cpu.Name.Should().Equal("Z80");
        generatorInput.OpcodeRead.Should().HaveCount(4);
        generatorInput.OverlapSteps.Should().NotBeEmpty();
    }

    [Test]
    public void Create_DeduplicatesIdenticalOverlapHandlers()
    {
        var generatorInput = GeneratorContext.Create("MrKWatkins.OakCpu.Z80", Z80Yaml);

        var ldcc = generatorInput.Instructions.Single(i => i.Prefix == null && i.Opcode == 0x49);
        var lddd = generatorInput.Instructions.Single(i => i.Prefix == null && i.Opcode == 0x52);

        generatorInput.GetOverlapIndex(ldcc.Steps.Single()).Should().Equal(generatorInput.GetOverlapIndex(lddd.Steps.Single()));
    }
}