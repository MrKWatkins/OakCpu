using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Yaml;

public sealed class FunctionYamlTests : TestFixture
{
    [Test]
    public void Deserialize_ValidFunctionWithAllProperties()
    {
        var yaml = """
                   name: add_bytes
                   type: u8
                   parameters:
                     - a
                     - b
                   expression: $a + $b
                   """;

        var function = YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        function.Name.Should().Equal("add_bytes");
        function.Type.Should().Equal("u8");
        function.Parameters.Should().HaveCount(2);
        function.Parameters[0].Should().Equal("a");
        function.Parameters[1].Should().Equal("b");
        function.Expression.Should().Equal("$a + $b");

        // Verify round-trip serialization
        var serializedBytes = YamlSerializer.Serialize(function, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);
        serializedYaml.Contains("name: add_bytes", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("type: u8", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("expression: $a + $b", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("- a", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("- b", StringComparison.Ordinal).Should().BeTrue();
    }

    [Test]
    public void Deserialize_ValidFunctionWithNoParameters()
    {
        var yaml = """
                   name: get_zero
                   type: u8
                   expression: 0
                   """;

        var function = YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        function.Name.Should().Equal("get_zero");
        function.Type.Should().Equal("u8");
        function.Parameters.Should().BeEmpty();
        function.Expression.Should().Equal("0");

        // Verify round-trip serialization
        var serializedBytes = YamlSerializer.Serialize(function, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);
        serializedYaml.Contains("name: get_zero", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("type: u8", StringComparison.Ordinal).Should().BeTrue();
    }

    [Test]
    public void Deserialize_ValidFunctionWithEmptyParameters()
    {
        var yaml = """
                   name: simple_function
                   type: bool
                   parameters: []
                   expression: true
                   """;

        var function = YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        function.Name.Should().Equal("simple_function");
        function.Type.Should().Equal("bool");
        function.Parameters.Should().BeEmpty();
        function.Expression.Should().Equal("true");
    }

    [Test]
    public void Deserialize_ValidFunctionWithSingleParameter()
    {
        var yaml = """
                   name: increment
                   type: u16
                   parameters:
                     - value
                   expression: $value + 1
                   """;

        var function = YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        function.Name.Should().Equal("increment");
        function.Type.Should().Equal("u16");
        function.Parameters.Should().HaveCount(1);
        function.Parameters[0].Should().Equal("value");
        function.Expression.Should().Equal("$value + 1");
    }

    [Test]
    public void Deserialize_ValidFunctionWithMultipleParameters()
    {
        var yaml = """
                   name: complex_calc
                   type: i32
                   parameters:
                     - x
                     - y
                     - z
                     - flag
                   expression: ($x * $y) + ($z & $flag)
                   """;

        var function = YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        function.Name.Should().Equal("complex_calc");
        function.Type.Should().Equal("i32");
        function.Parameters.Should().HaveCount(4);
        function.Parameters.Should().HaveCount(4);
        function.Parameters[0].Should().Equal("x");
        function.Parameters[1].Should().Equal("y");
        function.Parameters[2].Should().Equal("z");
        function.Parameters[3].Should().Equal("flag");
        function.Expression.Should().Equal("($x * $y) + ($z & $flag)");
    }

    [TestCase("u8")]
    [TestCase("i8")]
    [TestCase("u16")]
    [TestCase("i32")]
    [TestCase("i32_bool")]
    [TestCase("bool")]
    [TestCase("void")]
    public void Deserialize_ValidReturnTypes(string returnType)
    {
        var yaml = $"""
                    name: test_function
                    type: {returnType}
                    expression: test_value
                    """;

        var function = YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        function.Type.Should().Equal(returnType);
    }

    [Test]
    public void Deserialize_WithMissingName()
    {
        var yaml = """
                   type: u8
                   expression: test
                   """;

        var function = YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        function.Name.Should().BeNull();
        function.Type.Should().Equal("u8");
        function.Expression.Should().Equal("test");
    }

    [Test]
    public void Deserialize_WithMissingType()
    {
        var yaml = """
                   name: test_function
                   expression: test
                   """;

        var function = YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        function.Name.Should().Equal("test_function");
        function.Type.Should().BeNull();
        function.Expression.Should().Equal("test");
    }

    [Test]
    public void Deserialize_WithMissingExpression()
    {
        var yaml = """
                   name: test_function
                   type: u8
                   """;

        var function = YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        function.Name.Should().Equal("test_function");
        function.Type.Should().Equal("u8");
        function.Expression.Should().BeNull();
    }

    [Test]
    public void Deserialize_WithEmptyName()
    {
        var yaml = """
                   name: ""
                   type: u8
                   expression: test
                   """;

        var function = YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        function.Name.Should().Equal("");
        function.Type.Should().Equal("u8");
        function.Expression.Should().Equal("test");
    }

    [Test]
    public void Deserialize_WithEmptyType()
    {
        var yaml = """
                   name: test_function
                   type: ""
                   expression: test
                   """;

        var function = YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        function.Name.Should().Equal("test_function");
        function.Type.Should().Equal("");
        function.Expression.Should().Equal("test");
    }

    [Test]
    public void Deserialize_WithEmptyExpression()
    {
        var yaml = """
                   name: test_function
                   type: u8
                   expression: ""
                   """;

        var function = YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        function.Name.Should().Equal("test_function");
        function.Type.Should().Equal("u8");
        function.Expression.Should().Equal("");
    }



    [Test]
    public void ToString_ReturnsExpectedFormat()
    {
        var yaml = """
                   name: my_function
                   type: u16
                   parameters:
                     - param1
                     - param2
                   expression: $param1 + $param2
                   """;

        var function = YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        function.ToString().Should().Equal("u16 my_function(param1, param2)");
    }

    [Test]
    public void ToString_NoParameters_ReturnsExpectedFormat()
    {
        var yaml = """
                   name: simple_func
                   type: bool
                   expression: true
                   """;

        var function = YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        function.ToString().Should().Equal("bool simple_func()");
    }
}