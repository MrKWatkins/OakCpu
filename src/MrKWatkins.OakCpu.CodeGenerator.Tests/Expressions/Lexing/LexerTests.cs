using System.Collections;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Lexing;
using Number = MrKWatkins.OakCpu.CodeGenerator.Language.Lexing.Number;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Expressions.Lexing;

public sealed class LexerTests
{
    [Test]
    public void Peek_And_Read()
    {
        using var reader = new StringReader("x");
        var lexer = new Lexer(reader);

        lexer.Peek().Should().Equal(new Identifier(0, "x"));
        lexer.Peek().Should().Equal(new Identifier(0, "x"));
        lexer.IsFinished.Should().BeFalse();

        lexer.Read().Should().Equal(new Identifier(0, "x"));
        lexer.IsFinished.Should().BeFalse();

        lexer.Peek().Should().Equal(new EndOfInput(1));
        lexer.Peek().Should().Equal(new EndOfInput(1));
        lexer.IsFinished.Should().BeFalse();

        lexer.Read().Should().Equal(new EndOfInput(1));
        lexer.IsFinished.Should().BeTrue();

        lexer.Invoking(l => l.Peek()).Should().Throw<InvalidOperationException>().That.Should().HaveMessage("Input has already been consumed.");
        lexer.Invoking(l => l.Read()).Should().Throw<InvalidOperationException>().That.Should().HaveMessage("Input has already been consumed.");
    }

    [Test]
    public void Peek_UnexpectedToken()
    {
        using var reader = new StringReader("[");
        var lexer = new Lexer(reader);

        lexer.Invoking(l => l.Peek()).Should().Throw<InvalidOperationException>().That.Should().HaveMessage("Unexpected character '['.");
    }

    [Test]
    public void Read_UnexpectedToken()
    {
        using var reader = new StringReader("[");
        var lexer = new Lexer(reader);

        lexer.Invoking(l => l.Read()).Should().Throw<InvalidOperationException>().That.Should().HaveMessage("Unexpected character '['.");
    }

    [Test]
    public void UnsupportedOperator()
    {
        using var reader = new StringReader("?");
        var lexer = new Lexer(reader);

        lexer.Invoking(l => l.Read()).Should().Throw<InvalidOperationException>().That.Should().HaveMessage("Unexpected character '?'.");
    }

    [Test]
    public void UnsupportedOperator_Asterisk()
    {
        using var reader = new StringReader("*");
        var lexer = new Lexer(reader);

        lexer.Invoking(l => l.Read()).Should().Throw<InvalidOperationException>().That.Should().HaveMessage("Unexpected character '*'.");
    }

    [Test]
    public void HexadecimalParsing_EdgeCases()
    {
        using var reader = new StringReader("0xABC");
        var lexer = new Lexer(reader);

        // Should read valid hex number
        var token = lexer.Read();
        token.Should().BeOfType<Number>();
        var number = (Number)token;
        number.Value.Should().Equal(0xABC);
    }

    [Test]
    public void ReadString_EmptyInput()
    {
        using var reader = new StringReader("");
        var lexer = new Lexer(reader);

        var token = lexer.Read();
        token.Should().BeOfType<EndOfInput>();
        lexer.IsFinished.Should().BeTrue();
    }

    [Test]
    public void ReadString_OnlyWhitespace()
    {
        using var reader = new StringReader("   \t\n  ");
        var lexer = new Lexer(reader);

        var token = lexer.Read();
        token.Should().BeOfType<EndOfInput>();
    }

    [Test]
    public void NumberParsing_DecimalZero()
    {
        using var reader = new StringReader("0");
        var lexer = new Lexer(reader);

        var token = lexer.Read();
        token.Should().BeOfType<Number>();
        var number = (Number)token;
        number.Value.Should().Equal(0);
    }

    [Test]
    public void NumberParsing_HexZero()
    {
        using var reader = new StringReader("0x0");
        var lexer = new Lexer(reader);

        var token = lexer.Read();
        token.Should().BeOfType<Number>();
        var number = (Number)token;
        number.Value.Should().Equal(0);
    }

    [TestCaseSource(nameof(EnumerateTestCases))]
    public void IEnumerable_Generic(string input, Token[] expectedTokens)
    {
        using var reader = new StringReader(input);
        var lexer = new Lexer(reader);

        var actualTokens = lexer.ToList();
        actualTokens.Should().SequenceEqual(expectedTokens);
    }

    [TestCaseSource(nameof(EnumerateTestCases))]
    // ReSharper disable once InconsistentNaming
    public void IEnumerable(string input, Token[] expectedTokens)
    {
        using var reader = new StringReader(input);
        var lexer = new Lexer(reader);

        var enumerator = ((IEnumerable)lexer).GetEnumerator();
        using var _ = enumerator as IDisposable;
        var actualTokens = new List<object?>();
        while (enumerator.MoveNext())
        {
            actualTokens.Add(enumerator.Current);
        }
        actualTokens.Should().SequenceEqual(expectedTokens);
    }

    [Pure]
    public static IEnumerable<TestCaseData> EnumerateTestCases()
    {
        static TestCaseData Create(string input, params Token[] expectedTokens) => new TestCaseData(input, expectedTokens).SetArgDisplayNames($"\"{input}\"");

        yield return Create("1234", new Number(0, 4, 1234), new EndOfInput(4));
        yield return Create("0x1234", new Number(0, 6, 0x1234), new EndOfInput(6));
        yield return Create("0x9ABC", new Number(0, 6, 0x9ABC), new EndOfInput(6));

        yield return Create("identifier", new Identifier(0, "identifier"), new EndOfInput(10));
        yield return Create("ident_ifier", new Identifier(0, "ident_ifier"), new EndOfInput(11));
        yield return Create("$temp", new Identifier(0, "$temp"), new EndOfInput(5));
        yield return Create("AF'", new Identifier(0, "AF'"), new EndOfInput(3));

        // Binary operators
        yield return Create("+", new BinaryOperator(0, Operator.Add), new EndOfInput(1));
        yield return Create("-", new BinaryOperator(0, Operator.Subtract), new EndOfInput(1));
        yield return Create("&", new BinaryOperator(0, Operator.And), new EndOfInput(1));
        yield return Create("|", new BinaryOperator(0, Operator.Or), new EndOfInput(1));
        yield return Create("^", new BinaryOperator(0, Operator.Xor), new EndOfInput(1));
        yield return Create("=", new BinaryOperator(0, Operator.Assignment), new EndOfInput(1));
        yield return Create("==", new BinaryOperator(0, Operator.Equality), new EndOfInput(2));
        yield return Create("!=", new BinaryOperator(0, Operator.NotEquals), new EndOfInput(2));
        yield return Create("<<", new BinaryOperator(0, Operator.LeftShift), new EndOfInput(2));
        yield return Create(">>", new BinaryOperator(0, Operator.RightShift), new EndOfInput(2));
        yield return Create("<", new BinaryOperator(0, Operator.LessThan), new EndOfInput(1));
        yield return Create("<=", new BinaryOperator(0, Operator.LessThanOrEqual), new EndOfInput(2));
        yield return Create(">", new BinaryOperator(0, Operator.GreaterThan), new EndOfInput(1));
        yield return Create(">=", new BinaryOperator(0, Operator.GreaterThanOrEqual), new EndOfInput(2));
        yield return Create("&&", new BinaryOperator(0, Operator.LogicalAnd), new EndOfInput(2));
        yield return Create("||", new BinaryOperator(0, Operator.LogicalOr), new EndOfInput(2));

        // Unary operators
        yield return Create("!", new UnaryOperator(0, Operator.Not), new EndOfInput(1));
        yield return Create("~", new UnaryOperator(0, Operator.BitwiseNot), new EndOfInput(1));

        // Punctuation
        yield return Create("(", new OpenBracket(0), new EndOfInput(1));
        yield return Create(")", new CloseBracket(0), new EndOfInput(1));
        yield return Create(",", new Comma(0), new EndOfInput(1));
        yield return Create(";", new SemiColon(0), new EndOfInput(1));

        // Keywords
        yield return Create("if", new Keyword(0, "if"), new EndOfInput(2));
        yield return Create("else", new Keyword(0, "else"), new EndOfInput(4));
        yield return Create("endif", new Keyword(0, "endif"), new EndOfInput(5));
        yield return Create("false", new Keyword(0, "false"), new EndOfInput(5));
        yield return Create("true", new Keyword(0, "true"), new EndOfInput(4));

        // Complex expressions
        yield return Create("(x + 4)", new OpenBracket(0), new Identifier(1, "x"), new BinaryOperator(3, Operator.Add), new Number(5, 1, 4), new CloseBracket(6), new EndOfInput(7));

        // Whitespace handling
        yield return Create("  x  ", new Identifier(2, "x"), new EndOfInput(3));
        yield return Create("\tx\n", new Identifier(1, "x"), new EndOfInput(2));
        yield return Create("x + y", new Identifier(0, "x"), new BinaryOperator(2, Operator.Add), new Identifier(4, "y"), new EndOfInput(5));

        // Mixed hex cases
        yield return Create("0xDEAD", new Number(0, 6, 0xDEAD), new EndOfInput(6));
        yield return Create("0xBEEF", new Number(0, 6, 0xBEEF), new EndOfInput(6));

        // Identifiers with special characters
        yield return Create("flag.Z", new Identifier(0, "flag.Z"), new EndOfInput(6));
        yield return Create("condition.NZ", new Identifier(0, "condition.NZ"), new EndOfInput(12));
        yield return Create("action.read", new Identifier(0, "action.read"), new EndOfInput(11));
        yield return Create("opcode_table.main", new Identifier(0, "opcode_table.main"), new EndOfInput(17));
    }
}