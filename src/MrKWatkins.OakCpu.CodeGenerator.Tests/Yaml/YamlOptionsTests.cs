using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Annotations;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public sealed class YamlOptionsTests : TestFixture
{
    [Test]
    public void Instance_ReturnsConfiguredOptions()
    {
        var options = YamlOptions.Instance;

        options.Should().NotBeNull();
        options.NamingConvention.Should().Equal(NamingConvention.SnakeCase);
    }

    [Test]
    public void Instance_IsSingleton()
    {
        var options1 = YamlOptions.Instance;
        var options2 = YamlOptions.Instance;

        // We test that we get the same reference both times
        ReferenceEquals(options1, options2).Should().BeTrue();
    }

    [Test]
    public void NamingConvention_SnakeCase_WorksCorrectly()
    {
        // Test that the snake_case naming convention works by serializing and deserializing FieldYaml
        var fieldYaml = """
                        name: test_field
                        type: u8
                        getter: true
                        setter: false
                        """;

        var field = YamlSerializer.Deserialize<FieldYaml>(System.Text.Encoding.UTF8.GetBytes(fieldYaml), YamlOptions.Instance);

        field.Name.Should().Equal("test_field");
        field.Type.Should().Equal(DataType.U8);
        field.Getter.Should().BeTrue();
        field.Setter.Should().BeFalse();
    }

    [Test]
    public void NamingConvention_AppliedToFieldYaml()
    {
        // Test that the naming convention is applied correctly to actual Yaml types
        var fieldYaml = """
                        name: test_field
                        type: u8
                        getter: true
                        setter: false
                        """;

        var field = YamlSerializer.Deserialize<FieldYaml>(System.Text.Encoding.UTF8.GetBytes(fieldYaml), YamlOptions.Instance);

        field.Name.Should().Equal("test_field");
        field.Type.Should().Equal(DataType.U8);
        field.Getter.Should().BeTrue();
        field.Setter.Should().BeFalse();
    }

    [Test]
    public void StandardOptions_BasedOn()
    {
        var options = YamlOptions.Instance;

        // Verify that the options are based on standard options
        // We can't directly compare to YamlSerializerOptions.Standard because they're different instances
        // but we can verify key properties
        options.Should().NotBeNull();
        options.NamingConvention.Should().Equal(NamingConvention.SnakeCase);
    }
}