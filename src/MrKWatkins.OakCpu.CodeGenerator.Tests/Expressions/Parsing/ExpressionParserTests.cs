using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Parsing;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Expressions.Parsing;

public sealed class ExpressionParserTests
{
    [TestCase("TestAction", "Action.TestAction")]
    [TestCase("R = R + 1", "R = (R + 0x01)")]
    [TestCase("R = R & 1", "R = (R & 0x01)")]
    [TestCase("R = R + R & 1", "R = (R + (R & 0x01))")]
    [TestCase("R = (R + R) & 1", "R = ((R + R) & 0x01)")]
    public void Parse(string expressionText, string expectedParsedExpression)
    {
        var parseContext = new ParserContext(
            new HashSet<string> { "TestAction" },
            new Dictionary<string, Register>
            {
                ["R"] = new("R", DataType.U8, false, false, null, 0),
                ["RP"] = new("RP", DataType.U16, false, false, null, 0)
            });

        var expression = ExpressionParser.Parse(parseContext, expressionText);
        expression.ToString().Should().BeEquivalentTo(expectedParsedExpression);
    }
}