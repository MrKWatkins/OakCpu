using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Parsing;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Expressions.Parsing;

public sealed class ExpressionParserTests
{
    [TestCase("TestAction", "Action.TestAction")]
    [TestCase("R = R + 1", "R = R + 0x01")]
    [TestCase("R = R & 1", "R = R & 0x01")]
    [TestCase("R = R + R & 1", "R = R + R & 0x01")]
    [TestCase("R = R + (R & 1)", "R = R + (R & 0x01)")]
    [TestCase("RP0 ^ RP1 ^ (RP0 + RP1)", "RP0 ^ RP1 ^ RP0 + RP1")]
    public void Parse(string expressionText, string expectedParsedExpression)
    {
        var parseContext = new ParserContext(
            new HashSet<string> { "TestAction" },
            new Dictionary<string, Register>
            {
                ["R"] = new("R", DataType.U8, false, false, null, 0),
                ["RP0"] = new("RP0", DataType.U16, false, false, null, 0),
                ["RP1"] = new("RP1", DataType.U16, false, false, null, 0)
            },
            new Dictionary<string, Flag>
            {
                ["X"] = new("X", 0)
            });

        var expression = ExpressionParser.ParseStatement(parseContext, expressionText);
        expression.ToString().Should().BeEquivalentTo(expectedParsedExpression);
    }
}