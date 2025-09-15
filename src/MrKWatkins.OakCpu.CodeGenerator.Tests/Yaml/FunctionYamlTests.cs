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
    public void Deserialize_MissingName_ShouldThrow()
    {
        var yaml = """
                   type: u8
                   expression: test
                   """;

        AssertThat.Invoking(() => YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance))
            .Should().Throw<Exception>();
    }

    [Test]
    public void Deserialize_MissingType_ShouldThrow()
    {
        var yaml = """
                   name: test_function
                   expression: test
                   """;

        AssertThat.Invoking(() => YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance))
            .Should().Throw<Exception>();
    }

    [Test]
    public void Deserialize_MissingExpression_ShouldThrow()
    {
        var yaml = """
                   name: test_function
                   type: u8
                   """;

        AssertThat.Invoking(() => YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance))
            .Should().Throw<Exception>();
    }

    [Test]
    public void Deserialize_EmptyName_ShouldThrow()
    {
        var yaml = """
                   name: ""
                   type: u8
                   expression: test
                   """;

        AssertThat.Invoking(() => YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance))
            .Should().Throw<Exception>();
    }

    [Test]
    public void Deserialize_EmptyType_ShouldThrow()
    {
        var yaml = """
                   name: test_function
                   type: ""
                   expression: test
                   """;

        AssertThat.Invoking(() => YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance))
            .Should().Throw<Exception>();
    }

    [Test]
    public void Deserialize_EmptyExpression_ShouldThrow()
    {
        var yaml = """
                   name: test_function
                   type: u8
                   expression: ""
                   """;

        AssertThat.Invoking(() => YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance))
            .Should().Throw<Exception>();
    }

    [Test]
    public void Deserialize_InvalidParametersList_ShouldThrow()
    {
        var yaml = """
                   name: test_function
                   type: u8
                   parameters: not_a_list
                   expression: test
                   """;

        AssertThat.Invoking(() => YamlSerializer.Deserialize<FunctionYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance))
            .Should().Throw<Exception>();
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