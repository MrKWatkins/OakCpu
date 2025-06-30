using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators;
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
        statements.Select(s => s.ToString()).Should().BeEquivalentTo(expectedParsedExpressions);
    }

    [TestCase("RP0 ^ RP1 ^ (RP0 + RP1)", "RP0 ^ RP1 ^ RP0 + RP1")]
    public void ParseExpression(string expressionText, string expectedParsedExpression)
    {
    }

    [Pure]
    private static ParserContext CreateContext()
    {
        var configuration = new Configuration(
            new[] { Action.None, new Action("memory_read", 1) }.ToDictionary(a => a.Name),
            new Dictionary<string, Register>
            {
                ["R"] = new("R", DataType.U8, false, false, null, 0, true),
                ["RP0"] = new("RP0", DataType.U16, false, false, null, 0, false),
                ["RP1"] = new("RP1", DataType.U16, false, false, null, 0, false)
            },
            new Dictionary<string, Flag>
            {
                ["X"] = new("X", 0, "S", "NS")
            },
            new OpcodeStepTables([]),
            new Dictionary<string, UserDefinedDataMember>());

        return new ParserContext(configuration);
    }
}