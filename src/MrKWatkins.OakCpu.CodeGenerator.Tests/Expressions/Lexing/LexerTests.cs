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
        using var disposable = enumerator as IDisposable;
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

        yield return Create("+", new BinaryOperator(0, Operator.Add), new EndOfInput(1));
        yield return Create("-", new BinaryOperator(0, Operator.Subtract), new EndOfInput(1));
        yield return Create("&", new BinaryOperator(0, Operator.And), new EndOfInput(1));
        yield return Create("|", new BinaryOperator(0, Operator.Or), new EndOfInput(1));
        yield return Create("^", new BinaryOperator(0, Operator.Xor), new EndOfInput(1));
        yield return Create("=", new BinaryOperator(0, Operator.Assignment), new EndOfInput(1));
        yield return Create("==", new BinaryOperator(0, Operator.Equality), new EndOfInput(2));

        yield return Create("(", new OpenBracket(0), new EndOfInput(1));
        yield return Create(")", new CloseBracket(0), new EndOfInput(1));
        yield return Create(",", new Comma(0), new EndOfInput(1));
        yield return Create(";", new SemiColon(0), new EndOfInput(1));

        yield return Create("(x + 4)", new OpenBracket(0), new Identifier(1, "x"), new BinaryOperator(3, Operator.Add), new Number(5, 1, 4), new CloseBracket(6), new EndOfInput(7));
    }
}