using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public sealed class InterruptModeYamlTests : TestFixture
{
    [Test]
    public void Deserialize_ValidInterruptModeWithAllProperties()
    {
        var yaml = """
                   number: 0
                   steps:
                     - step_one
                     - step_two
                     - step_three
                   next_opcode: read
                   """;

        var interruptMode = YamlSerializer.Deserialize<InterruptModeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        interruptMode.Number.Should().Equal(0);
        interruptMode.Steps.Should().HaveCount(3);
        interruptMode.Steps[0].Should().Equal("step_one");
        interruptMode.Steps[1].Should().Equal("step_two");
        interruptMode.Steps[2].Should().Equal("step_three");
        interruptMode.NextOpcode.Should().Equal(NextOpcodeMode.Read);

        // Verify round-trip serialization
        var serializedBytes = YamlSerializer.Serialize(interruptMode, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);
        serializedYaml.Contains("number: 0", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("next_opcode: read", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("- step_one", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("- step_two", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("- step_three", StringComparison.Ordinal).Should().BeTrue();
    }

    [Test]
    public void Deserialize_ValidInterruptModeWithMinimalProperties()
    {
        var yaml = """
                   number: 1
                   next_opcode: overlapped
                   """;

        var interruptMode = YamlSerializer.Deserialize<InterruptModeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        interruptMode.Number.Should().Equal(1);
        interruptMode.Steps.Should().BeEmpty();
        interruptMode.NextOpcode.Should().Equal(NextOpcodeMode.Overlapped);
    }

    [Test]
    public void Deserialize_ValidInterruptModeWithEmptySteps()
    {
        var yaml = """
                   number: 2
                   steps: []
                   next_opcode: custom
                   """;

        var interruptMode = YamlSerializer.Deserialize<InterruptModeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        interruptMode.Number.Should().Equal(2);
        interruptMode.Steps.Should().BeEmpty();
        interruptMode.NextOpcode.Should().Equal(NextOpcodeMode.Custom);
    }

    [Test]
    public void Deserialize_ValidInterruptModeWithSingleStep()
    {
        var yaml = """
                   number: 1
                   steps:
                     - single_step
                   next_opcode: loop
                   """;

        var interruptMode = YamlSerializer.Deserialize<InterruptModeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        interruptMode.Number.Should().Equal(1);
        interruptMode.Steps.Should().HaveCount(1);
        interruptMode.Steps[0].Should().Equal("single_step");
        interruptMode.NextOpcode.Should().Equal(NextOpcodeMode.Loop);
    }

    [Test]
    public void Deserialize_ValidInterruptModeWithNullSteps()
    {
        var yaml = """
                   number: 3
                   steps:
                     - step_one
                     - null
                     - step_three
                   next_opcode: read
                   """;

        var interruptMode = YamlSerializer.Deserialize<InterruptModeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        interruptMode.Number.Should().Equal(3);
        interruptMode.Steps.Should().HaveCount(3);
        interruptMode.Steps[0].Should().Equal("step_one");
        interruptMode.Steps[1].Should().BeNull();
        interruptMode.Steps[2].Should().Equal("step_three");
        interruptMode.NextOpcode.Should().Equal(NextOpcodeMode.Read);
    }

    [TestCase(0, (byte)0)]
    [TestCase(1, (byte)1)]
    [TestCase(2, (byte)2)]
    [TestCase(255, (byte)255)]
    public void Deserialize_ValidNumberValues(int numberValue, byte expectedNumber)
    {
        var yaml = $"""
                    number: {numberValue}
                    next_opcode: read
                    """;

        var interruptMode = YamlSerializer.Deserialize<InterruptModeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        interruptMode.Number.Should().Equal(expectedNumber);
    }

    [TestCase("read", NextOpcodeMode.Read)]
    [TestCase("overlapped", NextOpcodeMode.Overlapped)]
    [TestCase("custom", NextOpcodeMode.Custom)]
    [TestCase("loop", NextOpcodeMode.Loop)]
    public void Deserialize_ValidNextOpcodeModes(string nextOpcodeMode, NextOpcodeMode expectedMode)
    {
        var yaml = $"""
                    number: 0
                    next_opcode: {nextOpcodeMode}
                    """;

        var interruptMode = YamlSerializer.Deserialize<InterruptModeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        interruptMode.NextOpcode.Should().Equal(expectedMode);
    }

    [Test]
    public void Deserialize_InvalidNextOpcodeMode_ShouldThrow()
    {
        var yaml = """
                   number: 0
                   next_opcode: invalid_mode
                   """;

        AssertThat.Invoking(() => YamlSerializer.Deserialize<InterruptModeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance))
            .Should().Throw<Exception>();
    }

    [Test]
    public void Deserialize_InvalidNumber_TooLarge_ShouldThrow()
    {
        var yaml = """
                   number: 256
                   next_opcode: read
                   """;

        AssertThat.Invoking(() => YamlSerializer.Deserialize<InterruptModeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance))
            .Should().Throw<Exception>();
    }

    [Test]
    public void Deserialize_InvalidNumber_Negative_ShouldThrow()
    {
        var yaml = """
                   number: -1
                   next_opcode: read
                   """;

        AssertThat.Invoking(() => YamlSerializer.Deserialize<InterruptModeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance))
            .Should().Throw<Exception>();
    }

    [Test]
    public void Deserialize_InvalidSteps_NotArray_ShouldThrow()
    {
        var yaml = """
                   number: 0
                   steps: not_an_array
                   next_opcode: read
                   """;

        AssertThat.Invoking(() => YamlSerializer.Deserialize<InterruptModeYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance))
            .Should().Throw<Exception>();
    }
}