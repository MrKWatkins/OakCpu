using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public sealed class InterruptsYamlTests : TestFixture
{
    [Test]
    public void Deserialize_ValidInterruptsWithAllProperties()
    {
        var yaml = """
                   handle: handle_interrupt
                   properties:
                     - name: enabled
                       type: bool
                       getter: true
                       setter: true
                     - name: counter
                       type: u8
                       getter: true
                   modes:
                     - number: 0
                       next_opcode: read
                     - number: 1
                       next_opcode: overlapped
                   halted_cycle:
                     - step_one
                     - step_two
                     - null
                   """;

        var interrupts = YamlSerializer.Deserialize<InterruptsYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        interrupts.Handle.Should().Equal("handle_interrupt");
        interrupts.Properties.Should().HaveCount(2);
        interrupts.Properties[0].Name.Should().Equal("enabled");
        interrupts.Properties[0].Type.Should().Equal(DataType.Bool);
        interrupts.Properties[1].Name.Should().Equal("counter");
        interrupts.Properties[1].Type.Should().Equal(DataType.U8);
        interrupts.Modes.Should().HaveCount(2);
        interrupts.Modes[0].Number.Should().Equal((byte)0);
        interrupts.Modes[1].Number.Should().Equal((byte)1);
        interrupts.HaltedCycle.Should().HaveCount(3);
        interrupts.HaltedCycle[0].Should().Equal("step_one");
        interrupts.HaltedCycle[1].Should().Equal("step_two");
        interrupts.HaltedCycle[2].Should().BeNull();
    }

    [Test]
    public void Deserialize_ValidInterruptsWithMinimalProperties()
    {
        var yaml = """
                   # No properties specified, should have defaults
                   """;

        var interrupts = YamlSerializer.Deserialize<InterruptsYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        interrupts.Handle.Should().BeNull();
        interrupts.Properties.Should().BeEmpty();
        interrupts.Modes.Should().BeEmpty();
        interrupts.HaltedCycle.Should().BeEmpty();
    }

    [Test]
    public void Deserialize_ValidInterruptsWithEmptyCollections()
    {
        var yaml = """
                   handle: empty_handler
                   properties: []
                   modes: []
                   halted_cycle: []
                   """;

        var interrupts = YamlSerializer.Deserialize<InterruptsYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        interrupts.Handle.Should().Equal("empty_handler");
        interrupts.Properties.Should().BeEmpty();
        interrupts.Modes.Should().BeEmpty();
        interrupts.HaltedCycle.Should().BeEmpty();
    }

    [Test]
    public void Deserialize_ValidInterruptsWithSingleItems()
    {
        var yaml = """
                   handle: single_handler
                   properties:
                     - name: single_prop
                       type: u16
                   modes:
                     - number: 42
                       next_opcode: custom
                   halted_cycle:
                     - single_step
                   """;

        var interrupts = YamlSerializer.Deserialize<InterruptsYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        interrupts.Handle.Should().Equal("single_handler");
        interrupts.Properties.Should().HaveCount(1);
        interrupts.Properties[0].Name.Should().Equal("single_prop");
        interrupts.Properties[0].Type.Should().Equal(DataType.U16);
        interrupts.Modes.Should().HaveCount(1);
        interrupts.Modes[0].Number.Should().Equal((byte)42);
        interrupts.Modes[0].NextOpcode.Should().Equal(NextOpcodeMode.Custom);
        interrupts.HaltedCycle.Should().HaveCount(1);
        interrupts.HaltedCycle[0].Should().Equal("single_step");
    }

    [Test]
    public void Deserialize_ValidInterruptsWithNullHandle()
    {
        var yaml = """
                   handle: null
                   properties: []
                   modes: []
                   halted_cycle: []
                   """;

        var interrupts = YamlSerializer.Deserialize<InterruptsYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        interrupts.Handle.Should().BeNull();
        interrupts.Properties.Should().BeEmpty();
        interrupts.Modes.Should().BeEmpty();
        interrupts.HaltedCycle.Should().BeEmpty();
    }

    [Test]
    public void Deserialize_ValidInterruptsWithMultipleProperties()
    {
        var yaml = """
                   properties:
                     - name: prop1
                       type: bool
                       getter: true
                     - name: prop2
                       type: u8
                       setter: true
                     - name: prop3
                       type: i32
                       getter: true
                       setter: true
                   """;

        var interrupts = YamlSerializer.Deserialize<InterruptsYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        interrupts.Properties.Should().HaveCount(3);
        interrupts.Properties[0].Name.Should().Equal("prop1");
        interrupts.Properties[0].Getter.Should().BeTrue();
        interrupts.Properties[0].Setter.Should().BeFalse();
        interrupts.Properties[1].Name.Should().Equal("prop2");
        interrupts.Properties[1].Getter.Should().BeFalse();
        interrupts.Properties[1].Setter.Should().BeTrue();
        interrupts.Properties[2].Name.Should().Equal("prop3");
        interrupts.Properties[2].Getter.Should().BeTrue();
        interrupts.Properties[2].Setter.Should().BeTrue();
    }

    [Test]
    public void Deserialize_ValidInterruptsWithMultipleModes()
    {
        var yaml = """
                   modes:
                     - number: 0
                       next_opcode: read
                     - number: 1
                       next_opcode: overlapped
                       steps:
                         - mode1_step1
                         - mode1_step2
                     - number: 2
                       next_opcode: custom
                       steps: []
                   """;

        var interrupts = YamlSerializer.Deserialize<InterruptsYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        interrupts.Modes.Should().HaveCount(3);
        interrupts.Modes[0].Number.Should().Equal((byte)0);
        interrupts.Modes[0].NextOpcode.Should().Equal(NextOpcodeMode.Read);
        interrupts.Modes[0].Steps.Should().BeEmpty();
        interrupts.Modes[1].Number.Should().Equal((byte)1);
        interrupts.Modes[1].NextOpcode.Should().Equal(NextOpcodeMode.Overlapped);
        interrupts.Modes[1].Steps.Should().HaveCount(2);
        interrupts.Modes[2].Number.Should().Equal((byte)2);
        interrupts.Modes[2].NextOpcode.Should().Equal(NextOpcodeMode.Custom);
    }

    [Test]
    public void Deserialize_InvalidProperties_NotArray_ShouldThrow()
    {
        var yaml = """
                   properties: not_an_array
                   """;

        AssertThat.Invoking(() => YamlSerializer.Deserialize<InterruptsYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance))
            .Should().Throw<Exception>();
    }

    [Test]
    public void Deserialize_InvalidModes_NotArray_ShouldThrow()
    {
        var yaml = """
                   modes: not_an_array
                   """;

        AssertThat.Invoking(() => YamlSerializer.Deserialize<InterruptsYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance))
            .Should().Throw<Exception>();
    }

    [Test]
    public void Deserialize_InvalidHaltedCycle_NotArray_ShouldThrow()
    {
        var yaml = """
                   halted_cycle: not_an_array
                   """;

        AssertThat.Invoking(() => YamlSerializer.Deserialize<InterruptsYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance))
            .Should().Throw<Exception>();
    }
}