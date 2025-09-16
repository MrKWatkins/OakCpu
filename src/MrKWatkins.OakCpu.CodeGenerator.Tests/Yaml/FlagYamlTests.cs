using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public sealed class FlagYamlTests : TestFixture
{
    [Test]
    public void Deserialize_ValidFlagWithAllProperties()
    {
        var yaml = """
                   name: C
                   index: 0
                   condition: $carry
                   not_condition: $not_carry
                   """;

        var flag = YamlSerializer.Deserialize<FlagYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        flag.Name.Should().Equal("C");
        flag.Index.Should().Equal(0);
        flag.Condition.Should().Equal("$carry");
        flag.NotCondition.Should().Equal("$not_carry");

        // Verify round-trip serialization
        var serializedBytes = YamlSerializer.Serialize(flag, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);
        serializedYaml.Contains("name: C", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("index: 0", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("condition: $carry", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("not_condition: $not_carry", StringComparison.Ordinal).Should().BeTrue();
    }

    [Test]
    public void Deserialize_ValidFlagWithMinimalProperties()
    {
        var yaml = """
                   name: Z
                   index: 6
                   """;

        var flag = YamlSerializer.Deserialize<FlagYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        flag.Name.Should().Equal("Z");
        flag.Index.Should().Equal(6);
        flag.Condition.Should().BeNull();
        flag.NotCondition.Should().BeNull();

        // Verify round-trip serialization
        var serializedBytes = YamlSerializer.Serialize(flag, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);
        serializedYaml.Contains("name: Z", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("index: 6", StringComparison.Ordinal).Should().BeTrue();
    }

    [Test]
    public void Deserialize_ValidFlagWithOnlyCondition()
    {
        var yaml = """
                   name: S
                   index: 7
                   condition: $sign
                   """;

        var flag = YamlSerializer.Deserialize<FlagYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        flag.Name.Should().Equal("S");
        flag.Index.Should().Equal(7);
        flag.Condition.Should().Equal("$sign");
        flag.NotCondition.Should().BeNull();

        // Verify round-trip serialization
        var serializedBytes = YamlSerializer.Serialize(flag, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);
        serializedYaml.Contains("name: S", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("index: 7", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("condition: $sign", StringComparison.Ordinal).Should().BeTrue();
    }

    [Test]
    public void Deserialize_ValidFlagWithOnlyNotCondition()
    {
        var yaml = """
                   name: H
                   index: 4
                   not_condition: $no_half_carry
                   """;

        var flag = YamlSerializer.Deserialize<FlagYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        flag.Name.Should().Equal("H");
        flag.Index.Should().Equal(4);
        flag.Condition.Should().BeNull();
        flag.NotCondition.Should().Equal("$no_half_carry");
    }

    [TestCase(0)]
    [TestCase(3)]
    [TestCase(7)]
    public void Deserialize_ValidIndexValues(int index)
    {
        var yaml = $"""
                    name: FLAG{index}
                    index: {index}
                    """;

        var flag = YamlSerializer.Deserialize<FlagYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        flag.Index.Should().Equal(index);
    }

    [Test]
    public void Deserialize_WithMissingName()
    {
        var yaml = """
                   index: 0
                   condition: TestCondition
                   """;

        var flag = YamlSerializer.Deserialize<FlagYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        flag.Name.Should().BeNull();
        flag.Index.Should().Equal(0);
        flag.Condition.Should().Equal("TestCondition");
    }

    [Test]
    public void Deserialize_WithMissingIndex()
    {
        var yaml = """
                   name: TestFlag
                   condition: TestCondition
                   """;

        var flag = YamlSerializer.Deserialize<FlagYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        flag.Name.Should().Equal("TestFlag");
        flag.Index.Should().Equal(0); // Default int value
        flag.Condition.Should().Equal("TestCondition");
    }

    [Test]
    public void Deserialize_WithNegativeIndex()
    {
        var yaml = """
                   name: TestFlag
                   index: -1
                   """;

        var flag = YamlSerializer.Deserialize<FlagYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        flag.Name.Should().Equal("TestFlag");
        flag.Index.Should().Equal(-1);
    }

    [Test]
    public void Deserialize_WithLargeIndex()
    {
        var yaml = """
                   name: TestFlag
                   index: 256
                   """;

        var flag = YamlSerializer.Deserialize<FlagYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        flag.Name.Should().Equal("TestFlag");
        flag.Index.Should().Equal(256);
    }



    [Test]
    public void ToString_ReturnsName()
    {
        var yaml = """
                   name: MyFlag
                   index: 3
                   """;

        var flag = YamlSerializer.Deserialize<FlagYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        flag.ToString().Should().Equal("MyFlag");
    }
}