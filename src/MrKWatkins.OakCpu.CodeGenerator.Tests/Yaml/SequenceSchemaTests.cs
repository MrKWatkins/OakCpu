using MrKWatkins.OakCpu.CodeGenerator.Generators;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public sealed class SequenceSchemaTests : TestFixture
{
    [Test]
    public void Deserialize_TopLevelSequences()
    {
        var yaml = """
                   cpu:
                     name: TestCpu
                   interrupts: {}
                   sequences:
                      - name: opcode_read
                        execute_overlap_on_start: true
                        next_opcode: custom
                        steps:
                          - opcode_step
                      - name: halted
                        next_opcode: loop
                        steps:
                          - halt_step
                      - name: interrupt_mode_0
                        group:
                          name: interrupt_mode
                          number: 0
                        next_opcode: custom
                        steps:
                          - interrupt_step
                   """;

        var yamlFile = YamlSerializer.Deserialize<YamlFile>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        yamlFile.Sequences.Should().HaveCount(3);
        yamlFile.OpcodeRead.Should().HaveCount(1);
        yamlFile.OpcodeRead[0].Should().Equal("opcode_step");
        var group = yamlFile.Sequences[2].Group;
        group.Should().NotBeNull();
        group!.Name.Should().Equal("interrupt_mode");
        group.Number.Should().Equal(0);
    }

    [Test]
    public void CreateGeneratorContext_WithTopLevelSequences()
    {
        var yaml = """
                   cpu:
                     name: TestCpu
                     actions:
                       - name: opcode_read
                         documentation: The host must read an opcode byte from memory.
                       - name: io_read
                         documentation: The host must read a byte from an I/O port.
                   interrupts:
                     properties:
                       - name: IM
                         type: u8
                         documentation: Gets or sets the interrupt mode.
                       - name: IFF1
                         type: bool
                         documentation: Gets or sets interrupt flip-flop 1.
                       - name: IFF2
                         type: bool
                         documentation: Gets or sets interrupt flip-flop 2.
                       - name: halted
                         type: bool
                         documentation: Gets or sets a value indicating whether the CPU is halted.
                       - name: interrupt
                         type: bool
                         documentation: Gets or sets a value indicating whether the interrupt line is asserted.
                     handle: |
                       interrupt = false;
                   sequences:
                      - name: opcode_read
                        execute_overlap_on_start: true
                        next_opcode: custom
                        steps:
                          - request(action.opcode_read);
                      - name: halted
                        execute_overlap_on_start: true
                        next_opcode: loop
                        steps:
                          - request(action.opcode_read);
                      - name: interrupt_mode_0
                        group:
                          name: interrupt_mode
                          number: 0
                        execute_overlap_on_start: true
                        next_opcode: custom
                        steps:
                          - request(action.io_read);
                   """;

        var yamlFile = YamlSerializer.Deserialize<YamlFile>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        var context = GeneratorContext.Create("MrKWatkins.OakCpu.TestCpu", yamlFile);

        context.Sequences.Should().ContainKey("opcode_read");
        context.Sequences.Should().ContainKey("halted");
        context.Sequences.Should().ContainKey("interrupt_mode_0");
        context.OpcodeRead.Should().HaveCount(1);
        context.Interrupts.Halted.Name.Should().Equal("halted");
        context.SequenceGroups.Should().ContainKey("interrupt_mode");
        context.SequenceGroups["interrupt_mode"].Members[0].Name.Should().Equal("interrupt_mode_0");
        Assert.That(context.GetStepLayout(context.SequenceGroups["interrupt_mode"].Members[0].FirstStep).Index, Is.GreaterThan(context.GetStepLayout(context.OpcodeRead.FirstStep).Index));
    }

    [Test]
    public void CreateGeneratorContext_WithLegacyInterruptModesCreatesSequenceGroup()
    {
        var yaml = """
                   cpu:
                     name: TestCpu
                     actions:
                       - name: io_read
                         documentation: The host must read a byte from an I/O port.
                   interrupts:
                     modes:
                       - number: 0
                         sequence: interrupt_mode_0
                   sequences:
                     - name: opcode_read
                       execute_overlap_on_start: true
                       next_opcode: custom
                       steps:
                         - request(action.io_read);
                     - name: halted
                       execute_overlap_on_start: true
                       next_opcode: loop
                       steps:
                         - request(action.io_read);
                     - name: interrupt_mode_0
                       execute_overlap_on_start: true
                       next_opcode: custom
                       steps:
                         - request(action.io_read);
                   """;

        var yamlFile = YamlSerializer.Deserialize<YamlFile>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        var context = GeneratorContext.Create("MrKWatkins.OakCpu.TestCpu", yamlFile);

        context.SequenceGroups.Should().ContainKey("interrupt_mode");
        context.SequenceGroups["interrupt_mode"].Members[0].Name.Should().Equal("interrupt_mode_0");
    }
}