using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Expressions.Parsing;

public sealed class ParserTests
{
    // Expression positive tests
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
    [TestCase("$temp + R", "temp + R")] // $ prefix is stripped in the AST representation
    [TestCase("flag.X", "X")] // flag. prefix is stripped in the AST representation
    public void ParseExpression(string expressionText, string expectedParsedExpression)
    {
        var context = CreateContext();
        var expression = Parser.ParseExpression(context, expressionText);
        expression.ToString().Should().Equal(expectedParsedExpression);
    }

    // Expression exception tests
    [TestCase(null, typeof(ArgumentException), "")]
    [TestCase("", typeof(ArgumentException), "")]
    [TestCase("   ", typeof(ArgumentException), "")]
    [TestCase("(R +", typeof(FormatException), "Exception parsing \"(R +\":")]
    [TestCase("unknown_identifier", typeof(FormatException), "Unsupported identifier unknown_identifier")]
    [TestCase("R +", typeof(FormatException), "Unexpected token")]
    [TestCase(")", typeof(FormatException), "Unexpected token")]
    [TestCase("(R + 1", typeof(FormatException), "Unexpected token")]
    public void ParseExpression_ThrowsException(string expressionText, Type expectedExceptionType, string expectedMessageSubstring)
    {
        var context = CreateContext();

        var exception = Assert.Throws(expectedExceptionType, () => Parser.ParseExpression(context, expressionText!));
        if (!string.IsNullOrEmpty(expectedMessageSubstring))
        {
            Assert.That(exception!.Message, Does.Contain(expectedMessageSubstring));
        }
    }

    // Statement positive tests
    [TestCase("", 0)]
    [TestCase("R = R + 1;", 1, "R = R + 0x01")]
    [TestCase("R = R & 1;R=R^1;", 2, "R = R & 0x01", "R = R ^ 0x01")]
    [TestCase("R = R + R & 1;", 1, "R = R + R & 0x01")]
    [TestCase("R = R + (R & 1);", 1, "R = R + (R & 0x01)")]
    public void ParseStatements(string statementsText, int expectedCount, params string[] expectedParsedExpressions)
    {
        var context = CreateContext();
        var statements = Parser.ParseStatements(context, statementsText);
        statements.Should().HaveCount(expectedCount);
        statements.Select(s => s.ToString()).Should().SequenceEqual(expectedParsedExpressions);
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

    // Statement exception tests
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