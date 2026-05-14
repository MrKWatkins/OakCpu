using System.Collections;
using System.Collections.Frozen;
using System.Globalization;
using System.Text;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Lexing;

/// <summary>
/// Hand rolled lexer to take an input stream and split it into tokens.
/// </summary>
internal sealed class Lexer(TextReader input) : IEnumerable<Token>
{
    private static readonly FrozenSet<char> NumberCharacters = new List<char>("x01234567890ABCDEF").ToFrozenSet();
    private static readonly FrozenSet<char> IdentifierCharacters = new List<char>("_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789'.$").ToFrozenSet();
    private int currentIndex;
    private Token? peeked;

    public bool IsFinished { get; private set; }

    internal Token Read()
    {
        if (IsFinished)
        {
            throw new InvalidOperationException("Input has already been consumed.");
        }

        // If we've already peeked reuse that token, otherwise read one in.
        Token token;
        if (peeked != null)
        {
            token = peeked;
            peeked = null;
        }
        else
        {
            token = ReadToken(currentIndex);
        }

        // Update the state from the token.
        currentIndex = token.StartIndex + token.Length;
        if (token is EndOfInput)
        {
            IsFinished = true;
        }
        return token;
    }

    internal Token Peek()
    {
        if (IsFinished)
        {
            throw new InvalidOperationException("Input has already been consumed.");
        }

        if (peeked != null)
        {
            return peeked;
        }

        peeked = ReadToken(currentIndex);
        return peeked;
    }

    private Token ReadToken(int startIndex)
    {
        while (true)
        {
            var value = input.Peek();
            if (value == -1)
            {
                return new EndOfInput(currentIndex);
            }

            var character = (char)value;
            if (char.IsWhiteSpace(character))
            {
                input.Read();
                startIndex += 1;
                continue;
            }

            if (character is >= '0' and <= '9')
            {
                return ReadNumber(startIndex);
            }

            if (IdentifierCharacters.Contains(character))
            {
                return ReadIdentifierOrKeyword(startIndex);
            }

            // Make more generic if we have multiple two character operators.
            if (Operator.OperatorCharacters.Contains(character))
            {
                return ReadOperator(startIndex);
            }

            input.Read();

            return character switch
            {
                '(' => new OpenBracket(startIndex),
                ')' => new CloseBracket(startIndex),
                ',' => new Comma(startIndex),
                ';' => new SemiColon(startIndex),
                _ => throw new InvalidOperationException($"Unexpected character '{character}'.")
            };
        }
    }

    [MustUseReturnValue]
    private Number ReadNumber(int startIndex)
    {
        var value = ReadString(NumberCharacters);

        var number = ParseNumber(value);

        return new Number(startIndex, value.Length, number);
    }

    [MustUseReturnValue]
    private Token ReadOperator(int startIndex)
    {
        var value = ReadString(Operator.OperatorCharacters);

        if (Operator.BinaryOperators.TryGetValue(value, out var binaryOperator))
        {
            return new BinaryOperator(startIndex, binaryOperator);
        }

        if (Operator.UnaryOperators.TryGetValue(value, out var unaryOperator))
        {
            return new UnaryOperator(startIndex, unaryOperator);
        }

        throw new NotSupportedException($"The operator {value} is not supported.");
    }

    [MustUseReturnValue]
    private Token ReadIdentifierOrKeyword(int startIndex)
    {
        var value = ReadString(IdentifierCharacters);

        return Keyword.All.Contains(value) ? new Keyword(startIndex, value) : new Identifier(startIndex, value);
    }

    [MustUseReturnValue]
    private string ReadString(FrozenSet<char> possibleCharacters)
    {
        var value = new StringBuilder();
        while (possibleCharacters.Contains((char)input.Peek()))
        {
            value.Append((char)input.Read());
        }

        return value.ToString();
    }

    public IEnumerator<Token> GetEnumerator()
    {
        Token token;
        do
        {
            token = Read();
            yield return token;
        } while (token is not EndOfInput);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    [Pure]
    private static int ParseNumber(string number) =>
        number.StartsWith("0x", StringComparison.Ordinal)
            ? int.Parse(number[2..], NumberStyles.HexNumber) // skipcq: CS-R1004
            : int.Parse(number); // skipcq: CS-R1004
}