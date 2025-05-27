namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public sealed class InstructionYamlTests : TestFixture
{
    [Test]
    public async Task Load_NOP()
    {
        var instructionsYaml = await LoadZ80DefinitionFileAsync("general.yaml");
        instructionsYaml.Instructions.Should().ContainSingle(i => i.Mnemonic == "NOP");
    }

    [Test]
    public async Task Load_LD_R0_R1()
    {
        var instructionsYaml = await LoadZ80DefinitionFileAsync("load-from-register-8-bit.yaml");
        instructionsYaml.Instructions.Should().ContainSingle(i => i.Mnemonic == "LD R0, R1");
    }
}