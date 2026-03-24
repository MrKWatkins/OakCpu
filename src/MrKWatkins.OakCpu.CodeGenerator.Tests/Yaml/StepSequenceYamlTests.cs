using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public sealed class StepSequenceYamlTests : TestFixture
{
    [Test]
    public void Deserialize_ValidStepSequenceWithAllProperties()
    {
        var yaml = """
                    name: opcode_read
                    group:
                      name: interrupt_mode
                      number: 1
                    execute_overlap_on_start: true
                    next_opcode: custom
                    steps:
                      - step_one
                      - step_two
                   """;

        var sequence = YamlSerializer.Deserialize<StepSequenceYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        sequence.Name.Should().Equal("opcode_read");
        var group = sequence.Group;
        group.Should().NotBeNull();
        group!.Name.Should().Equal("interrupt_mode");
        group.Number.Should().Equal(1);
        sequence.ExecuteOverlapOnStart.Should().BeTrue();
        sequence.NextOpcode.Should().Equal(NextOpcodeMode.Custom);
        sequence.Steps.Should().HaveCount(2);
        sequence.Steps[0].Should().Equal("step_one");
        sequence.Steps[1].Should().Equal("step_two");
    }
}