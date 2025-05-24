namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public class RegisterYamlTests : TextFixture
{
    [Test]
    public async Task Load()
    {
        var registersYaml = await LoadZ80DefinitionFileAsync("registers.yaml");
        registersYaml.Registers.Should().ContainSingle(r => r.Flags);
    }
}