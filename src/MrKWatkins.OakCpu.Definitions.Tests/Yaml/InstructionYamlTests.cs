using MrKWatkins.OakCpu.Definitions.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.Definitions.Tests.Yaml;

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
        var yaml = await LoadZ80YamlAsync();
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
    public void Deserialize_DocumentationAndUndocumented()
    {
        var yaml = """
                   group: Test
                   mnemonic: SLL R0
                   documentation: Shifts {R0} left.
                   undocumented: true
                   opcodes:
                     - opcode: 0xCB 0x30
                       undocumented: true
                     - opcode: 0xCB 0x31
                   steps:
                     - R0 = R0 << 1;
                   """;

        var instruction = YamlSerializer.Deserialize<InstructionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        instruction.Documentation.Should().Equal("Shifts {R0} left.");
        instruction.Undocumented.Should().BeTrue();
        instruction.Opcodes[0].Undocumented.Should().BeTrue();
        instruction.Opcodes[1].Undocumented.Should().BeFalse();
    }

    [Test]
    public void Deserialize_DocumentationAndUndocumentedDefaults()
    {
        var yaml = """
                   group: Test
                   mnemonic: NOP
                   opcodes:
                     - opcode: 0x00
                   steps:
                     -
                   """;

        var instruction = YamlSerializer.Deserialize<InstructionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        instruction.Documentation.Should().BeNull();
        instruction.Undocumented.Should().BeFalse();
        instruction.Opcodes[0].Undocumented.Should().BeFalse();
    }

    [Test]
    public void WithOpcodes_KeepsDocumentationAndUndocumented()
    {
        var yaml = """
                   group: Test
                   mnemonic: SLL R0
                   documentation: Shifts {R0} left.
                   undocumented: true
                   opcodes:
                     - opcode: 0xCB 0x30
                     - opcode: 0xCB 0x31
                   steps:
                     - R0 = R0 << 1;
                   """;

        var instruction = YamlSerializer.Deserialize<InstructionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        var reduced = instruction.WithOpcodes([instruction.Opcodes[0]]);

        reduced.Opcodes.Should().HaveCount(1);
        reduced.Documentation.Should().Equal("Shifts {R0} left.");
        reduced.Undocumented.Should().BeTrue();
    }
}