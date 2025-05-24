namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public sealed class InstructionYamlTests : TestFixture
{
    [Test]
    public async Task Load()
    {
        var instructionsYaml = await LoadZ80DefinitionFileAsync("general.yaml");
        instructionsYaml.Instructions.Should().ContainSingle(i => i.Name == "NOP");
    }
}