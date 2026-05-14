using MrKWatkins.OakCpu.CodeGenerator.Language.Lexing;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Expressions.Lexing;

public sealed class TokenTests
{
    [Test]
    public void Comma()
    {
        var token = new Comma(4);

        token.StartIndex.Should().Equal(4);
        token.Length.Should().Equal(1);
        token.ToString().Should().Equal(",");
    }

    [Test]
    public void OpenBracket()
    {
        var token = new OpenBracket(7);

        token.StartIndex.Should().Equal(7);
        token.Length.Should().Equal(1);
        token.ToString().Should().Equal("(");
    }
}