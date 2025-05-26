namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public class CpuYamlTests : TestFixture
{
    [Test]
    public async Task Load()
    {
        var yaml = await LoadZ80DefinitionFileAsync("cpu.yaml");
        yaml.Cpu.Should().NotBeNull();
    }
}