using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Expressions.Parsing;

public sealed class ParserTests
{
    [TestCase("")]
    [TestCase("R = R + 1;", "R = R + 0x01")]
    [TestCase("R = R & 1;R=R^1;", "R = R & 0x01", "R = R ^ 0x01")]
    [TestCase("R = R + R & 1;", "R = R + R & 0x01")]
    [TestCase("R = R + (R & 1);", "R = R + (R & 0x01)")]
    public void ParseStatements(string statementsText, params string[] expectedParsedExpressions)
    {
        var context = CreateContext();
        var statements = Parser.ParseStatements(context, statementsText);
        statements.Select(s => s.ToString()).Should().SequenceEqual(expectedParsedExpressions);
    }

    [TestCase("RP0 ^ RP1 ^ (RP0 + RP1)", "RP0 ^ RP1 ^ RP0 + RP1")]
    [TestCase("true", "True")]
    [TestCase("false", "False")]
    [TestCase("!true", "!(True)")]
    [TestCase("~0xFF", "~(0xFF)")]
    [TestCase("R << 2", "R << 0x02")]
    [TestCase("R >> 1", "R >> 0x01")]
    [TestCase("R < 10", "R < 0x0A")]
    [TestCase("R <= 10", "R <= 0x0A")]
    [TestCase("R > 10", "R > 0x0A")]
    [TestCase("R >= 10", "R >= 0x0A")]
    [TestCase("R == 10", "R == 0x0A")]
    [TestCase("R != 10", "R != 0x0A")]
    [TestCase("true && false", "True && False")]
    [TestCase("true || false", "True || False")]
    [TestCase("((R + 1))", "R + 0x01")]
    // Logical operator precedence tests
    [TestCase("true && false || true", "True && False || True")] // && has higher precedence than ||
    [TestCase("false || true && false", "False || True && False")] // && has higher precedence than ||
    [TestCase("true && (false || true)", "True && (False || True)")] // Parentheses override precedence
    [TestCase("(true && false) || true", "True && False || True")] // Explicit parentheses are removed when not needed
    [TestCase("R > 0 && R < 10", "R > 0x00 && R < 0x0A")] // Comparison with logical AND
    [TestCase("R == 5 || R == 10", "R == 0x05 || R == 0x0A")] // Comparison with logical OR
    [TestCase("R + 1 > 5 && R - 1 < 10", "R + 0x01 > 0x05 && R - 0x01 < 0x0A")] // Arithmetic, comparison, and logical operators
    [TestCase("!R == 0 || R > 10", "!(R) == 0x00 || R > 0x0A")] // Unary, comparison, and logical operators
    [TestCase("R & 1 == 1 && R > 0", "R & 0x01 == 0x01 && R > 0x00")] // Bitwise, comparison, and logical operators
    public void ParseExpression(string expressionText, string expectedParsedExpression)
    {
        var context = CreateContext();
        var expression = Parser.ParseExpression(context, expressionText);
        expression.ToString().Should().Equal(expectedParsedExpression);
    }

    [Test]
    public void ParseExpression_WithTemporaryVariable()
    {
        var context = CreateContext();
        var expression = Parser.ParseExpression(context, "$temp + R");
        expression.ToString().Should().Equal("temp + R"); // $ prefix is stripped in the AST representation
    }

    [Test]
    public void ParseExpression_WithFlagAccess()
    {
        var context = CreateContext();
        var expression = Parser.ParseExpression(context, "flag.X");
        expression.ToString().Should().Equal("X"); // flag. prefix is stripped in the AST representation
    }

    [Test]
    public void ParseExpression_NullOrWhitespace_ThrowsArgumentException()
    {
        var context = CreateContext();

        Assert.Throws<ArgumentException>(() => Parser.ParseExpression(context, null!));
        Assert.Throws<ArgumentException>(() => Parser.ParseExpression(context, ""));
        Assert.Throws<ArgumentException>(() => Parser.ParseExpression(context, "   "));
    }

    [Test]
    public void ParseExpression_InvalidExpression_ThrowsFormatException()
    {
        var context = CreateContext();

        var exception = Assert.Throws<FormatException>(() => Parser.ParseExpression(context, "(R +"));
        Assert.That(exception!.Message, Does.StartWith("Exception parsing \"(R +\":"));
    }

    [TestCase("if R > 0; R = R - 1; else; R = 0; endif;", 1, typeof(IfStatement))]
    [TestCase("if R > 0; if R > 5; R = 10; endif; endif;", 1, typeof(IfStatement))]
    [TestCase("R = 5;", 1, typeof(Assignment))]
    [TestCase("$temp;", 1, typeof(TemporaryVariableDeclarationStatement))]
    public void ParseStatements_ValidStatements(string statementsText, int expectedCount, Type expectedStatementType)
    {
        var context = CreateContext();
        var statements = Parser.ParseStatements(context, statementsText);
        statements.Should().HaveCount(expectedCount);
        statements[0].GetType().Should().Equal(expectedStatementType);
    }

    [TestCase("R = 1", typeof(FormatException), "Expected semi-colon after statement")]
    [TestCase("if R > 0; R = 1;", typeof(FormatException), "if without endif")]
    [TestCase("if; R = 1; endif;", typeof(FormatException), "Unexpected token SemiColon")]
    [TestCase("if R > 0; if R > 5; R = 10; endif;", typeof(FormatException), "if without endif")]
    [TestCase("if R > 0 R = 1; endif;", typeof(FormatException), "Unexpected token Identifier")]
    [TestCase("if R > 0; R = 1; else; R = 2; else; R = 3; endif;", typeof(FormatException), "Multiple else statements")]
    [TestCase("123;", typeof(FormatException), "did not parse to a statement")]
    public void ParseStatements_InvalidStatements_ThrowsException(string statementsText, Type expectedExceptionType, string expectedMessageSubstring)
    {
        var context = CreateContext();

        var exception = Assert.Throws(expectedExceptionType, () => Parser.ParseStatements(context, statementsText));
        Assert.That(exception!.Message, Does.Contain(expectedMessageSubstring));
    }

    [Test]
    public void ParseExpression_UnsupportedIdentifier_ThrowsNotSupportedException()
    {
        var context = CreateContext();

        var exception = Assert.Throws<FormatException>(() => Parser.ParseExpression(context, "unknown_identifier"));
        Assert.That(exception!.Message, Does.Contain("Unsupported identifier unknown_identifier"));
    }

    [Test]
    public void ParseExpression_UnexpectedToken_ThrowsFormatException()
    {
        var context = CreateContext();

        var exception = Assert.Throws<FormatException>(() => Parser.ParseExpression(context, "R +"));
        Assert.That(exception!.Message, Does.Contain("Unexpected token"));
    }

    [Test]
    public void ParseExpression_UnexpectedCloseBracket_ThrowsFormatException()
    {
        var context = CreateContext();

        var exception = Assert.Throws<FormatException>(() => Parser.ParseExpression(context, ")"));
        Assert.That(exception!.Message, Does.Contain("Unexpected token"));
    }

    [Test]
    public void ParseExpression_MismatchedBrackets_ThrowsFormatException()
    {
        var context = CreateContext();

        var exception = Assert.Throws<FormatException>(() => Parser.ParseExpression(context, "(R + 1"));
        Assert.That(exception!.Message, Does.Contain("Unexpected token"));
    }

    [Pure]
    private static ParserContext CreateContext()
    {
        var configuration = new Configuration(
            new[] { Action.None, new Action("memory_read", 1) }.ToDictionary(a => a.Name),
            new Dictionary<string, Register>
            {
                ["R"] = new("R", DataType.U8, false, false, null, 0, true, null),
                ["RP0"] = new("RP0", DataType.U16, false, false, null, 0, false, null),
                ["RP1"] = new("RP1", DataType.U16, false, false, null, 0, false, null)
            },
            new Dictionary<string, Flag>
            {
                ["X"] = new("X", 0, "S", "NS"),
                ["Z"] = new("Z", 1, "NZ", "Z")
            },
            new OpcodeStepTables([]),
            new Dictionary<string, UserDefinedDataMember>());

        // Initialize empty user-defined functions - tests will focus on basic expressions
        configuration.UserDefinedFunctions = new Dictionary<string, UserDefinedFunction>();

        return new ParserContext(configuration);
    }
}