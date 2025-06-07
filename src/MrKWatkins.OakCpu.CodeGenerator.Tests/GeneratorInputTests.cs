using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public sealed class GeneratorInputTests : TestFixture
{
    [Test]
    public void Create()
    {
        var generatorInput = GeneratorInput.Create("MrKWatkins.OakCpu.Z80", Z80Yaml);
        generatorInput.Cpu.Name.Should().Be("Z80");
    }

    [Test]
    public async Task Create_Cpu()
    {
        var cpu = await LoadZ80DefinitionFileAsync("cpu.yaml");
        var registers = await LoadZ80DefinitionFileAsync("registers.yaml");

        var combined = YamlFile.Combine(cpu, registers);

        var generatorInput = GeneratorInput.Create("MrKWatkins.OakCpu.Z80", combined);
        generatorInput.Cpu.Name.Should().Be("Z80");
    }
}