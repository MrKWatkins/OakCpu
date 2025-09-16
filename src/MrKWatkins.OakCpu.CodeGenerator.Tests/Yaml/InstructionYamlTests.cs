using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public sealed class InstructionYamlTests : TestFixture
{
    [Test]
    public async Task Load_NOP()
    {
        var instructionsYaml = await LoadZ80DefinitionFileAsync("general-purpose-arithmetic-and-cpu-control/nop.yaml");
        instructionsYaml.Instructions.Should().ContainSingle(i => i.Mnemonic == "NOP");
    }

    [Test]
    public async Task Load_LD_R0_R1()
    {
        var instructionsYaml = await LoadZ80DefinitionFileAsync("8-bit-load/ld-8-bit-copy_register_register.yaml");
        instructionsYaml.Instructions.Should().ContainSingle(i => i.Mnemonic == "LD R0, R1");
    }

    [Test]
    public async Task Load_LD_RP0_nn()
    {
        var instructionsYaml = await LoadZ80DefinitionFileAsync("16-bit-load/ld-16-bit-load_register-pair_immediate.yaml");
        instructionsYaml.Instructions.Should().ContainSingle(i => i.Mnemonic == "LD RP0, nn");
    }

    [Test]
    public async Task Groups()
    {
        var yaml = await LoadZ80Yaml();
        var groups = yaml.Instructions.Select(i => i.Group).Distinct().Order().ToList();
        groups.Should().SequenceEqual(
            "16-Bit Arithmetic",
            "16-Bit Load",
            "8-Bit Arithmetic",
            "8-Bit Load",
            "Bit Set, Reset, and Test",
            "Call and Return",
            "Exchange, Block Transfer, and Search",
            "General-Purpose Arithmetic and CPU Control",
            "Input and Output",
            "Jump",
            "Redirects",
            "Rotate and Shift");
    }

    [Test]
    public void Serialize_ValidInstructionWithMinimalProperties()
    {
        var originalYaml = """
                           group: Test Group
                           mnemonic: NOP
                           next_opcode: read
                           """;

        var instruction = YamlSerializer.Deserialize<InstructionYaml>(System.Text.Encoding.UTF8.GetBytes(originalYaml), YamlOptions.Instance);
        var serializedBytes = YamlSerializer.Serialize(instruction, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);

        // Verify that serialization works and produces valid YAML output
        (serializedYaml.Length > 0).Should().BeTrue();
        serializedYaml.Contains("group: Test Group", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("mnemonic: NOP", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("next_opcode: read", StringComparison.Ordinal).Should().BeTrue();
    }
}