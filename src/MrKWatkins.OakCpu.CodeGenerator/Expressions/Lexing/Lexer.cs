using System.Collections;
using System.Globalization;
using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Lexing;

/// <summary>
/// Hand rolled lexer to take an input stream and split it into tokens.
/// </summary>
internal sealed class Lexer(TextReader input) : IEnumerable<Token>
{
    private static readonly HashSet<char> NumberCharacters = new("x01234567890ABCDEF");
    private static readonly HashSet<char> IdentifierCharacters = new("_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
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
        if (token is EndOfExpression)
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
                return new EndOfExpression(currentIndex);
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
                return ReadIdentifier(startIndex);
            }

            input.Read();

            // Make more generic if we have multiple two character operators.
            if (character == '=')
            {
                if (input.Peek() == '=')
                {
                    input.Read();
                    return new BinaryOperator(startIndex, "==");
                }
            }

            var operatorString = new string(character, 1);
            if (BinaryOperator.Operators.Contains(operatorString))
            {
                return new BinaryOperator(startIndex, operatorString);
            }

            if (UnaryOperator.Operators.Contains(character))
            {
                return new UnaryOperator(startIndex, character);
            }

            return character switch
            {
                '(' => new OpenBracket(startIndex),
                ')' => new CloseBracket(startIndex),
                ',' => new Comma(startIndex),
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
    private Identifier ReadIdentifier(int startIndex)
    {
        var value = ReadString(IdentifierCharacters);

        return new Identifier(startIndex, value);
    }

    [MustUseReturnValue]
    private string ReadString(HashSet<char> searchValues)
    {
        var value = new StringBuilder();
        while (searchValues.Contains((char)input.Peek()))
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
        } while (token is not EndOfExpression);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    [Pure]
    private static int ParseNumber(string number)
    {
        if (number.StartsWith("0x", StringComparison.Ordinal))
        {
            return int.Parse(number.Substring(2), NumberStyles.HexNumber);
        }

        return int.Parse(number);
    }
}