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
        var instructionsYaml = await LoadZ80DefinitionFileAsync("ld-8-bit-load-from-register.yaml");
        instructionsYaml.Instructions.Should().ContainSingle(i => i.Mnemonic == "LD R0, R1");
    }

    [Test]
    public async Task Load_LD_RP0_nn()
    {
        var instructionsYaml = await LoadZ80DefinitionFileAsync("ld-16-bit-load-immediate.yaml");
        instructionsYaml.Instructions.Should().ContainSingle(i => i.Mnemonic == "LD RP0, nn");
    }
}