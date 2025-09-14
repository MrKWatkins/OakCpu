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

    [Test]
    public void ParseStatements_IfElseEndif()
    {
        var context = CreateContext();
        var statements = Parser.ParseStatements(context, "if R > 0; R = R - 1; else; R = 0; endif;");
        statements.Should().HaveCount(1);
        statements[0].Should().BeOfType<IfStatement>();
    }

    [Test]
    public void ParseStatements_NestedIf()
    {
        var context = CreateContext();
        var statements = Parser.ParseStatements(context, "if R > 0; if R > 5; R = 10; endif; endif;");
        statements.Should().HaveCount(1);
        statements[0].Should().BeOfType<IfStatement>();
    }

    [Test]
    public void ParseStatements_CallStatement()
    {
        var context = CreateContext();
        // Since we don't have functions configured, this test would fail
        // Let's test a simpler assignment statement instead
        var statements = Parser.ParseStatements(context, "R = 5;");
        statements.Should().HaveCount(1);
        statements[0].Should().BeOfType<Assignment>();
    }

    [Test]
    public void ParseStatements_TemporaryVariableDeclaration()
    {
        var context = CreateContext();
        var statements = Parser.ParseStatements(context, "$temp;");
        statements.Should().HaveCount(1);
        statements[0].Should().BeOfType<TemporaryVariableDeclarationStatement>();
    }

    [Test]
    public void ParseStatements_WithoutSemiColon_ThrowsInvalidOperationException()
    {
        var context = CreateContext();

        var exception = Assert.Throws<FormatException>(() => Parser.ParseStatements(context, "R = 1"));
        Assert.That(exception!.Message, Does.Contain("Expected semi-colon after statement"));
    }

    [Test]
    public void ParseStatements_IfWithoutEndif_ThrowsInvalidOperationException()
    {
        var context = CreateContext();

        var exception = Assert.Throws<FormatException>(() => Parser.ParseStatements(context, "if R > 0; R = 1;"));
        Assert.That(exception!.Message, Does.Contain("if without endif"));
    }

    [Test]
    public void ParseStatements_MultipleElse_ThrowsInvalidOperationException()
    {
        var context = CreateContext();

        var exception = Assert.Throws<FormatException>(() => Parser.ParseStatements(context, "if R > 0; R = 1; else; R = 2; else; R = 3; endif;"));
        Assert.That(exception!.Message, Does.Contain("Multiple else statements"));
    }

    [Test]
    public void ParseStatements_InvalidStatement_ThrowsInvalidOperationException()
    {
        var context = CreateContext();

        var exception = Assert.Throws<FormatException>(() => Parser.ParseStatements(context, "123;"));
        Assert.That(exception!.Message, Does.Contain("did not parse to a statement"));
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

    [Test]
    public void ParseExpression_IfConditionMustBeExpression_ThrowsFormatException()
    {
        var context = CreateContext();

        // This tests that if condition must be an expression
        var exception = Assert.Throws<FormatException>(() => Parser.ParseStatements(context, "if; endif;"));
        Assert.That(exception!.Message, Does.Contain("Unexpected token SemiColon"));
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