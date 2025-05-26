using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Lexing;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Parsing;

/// <summary>
/// Parses expressions.
/// </summary>
/// <remarks>Based on https://matklad.github.io/2020/04/13/simple-but-powerful-pratt-parsing.html.</remarks>
public static class ExpressionParser
{
    [Pure]
    public static Expression Parse(ParserContext context, string expression)
    {
        using var reader = new StringReader(expression);
        var lexer = new Lexer(reader);
        var parsed = ParseExpression(context, lexer, 0);
        return parsed;
    }

    [MustUseReturnValue]
    private static Expression ParseExpression(ParserContext context, Lexer lexer, int minimumBindingPower)
    {
        Expression left;
        switch (lexer.Read())
        {
            case Lexing.Number number:
                left = new Ast.Number(number.Value);
                break;

            case OpenBracket:
                left = ParseExpression(context, lexer, 0);
                var next = lexer.Read();
                if (next is not CloseBracket)
                {
                    throw CreateUnexpectedTokenException(next);
                }
                break;

            case Identifier identifier:
                left = ParseIdentifier(context, identifier.Name);
                break;

            case var token:
                throw CreateUnexpectedTokenException(token);
        }

        while (true)
        {
            var token = lexer.Peek();
            if (token is EndOfExpression or CloseBracket)
            {
                break;
            }

            if (token is not Operator @operator)
            {
                throw CreateUnexpectedTokenException(token);
            }

            var (leftBindingPower, rightBindingPower) = GetBindingPower(@operator);
            if (leftBindingPower < minimumBindingPower)
            {
                break;
            }

            lexer.Read();

            var right = ParseExpression(context, lexer, rightBindingPower);
            left = @operator.Symbol == '=' ? new Assignment(left, right) : new BinaryOperation(@operator.Symbol, left, right);
        }

        return left;
    }

    [Pure]
    private static Expression ParseIdentifier(ParserContext context, string identifier)
    {
        if (context.Actions.Contains(identifier))
        {
            return new RequestAction(identifier);
        }

        if (context.Registers.TryGetValue(identifier, out var register))
        {
            return new RegisterAccess(register);
        }

        if (MemberAccess.KnownFields.Contains(identifier))
        {
            return new MemberAccess(identifier);
        }

        throw new NotSupportedException($"Unsupported identifier {identifier}.");
    }

    [Pure]
    private static (int Left, int Right) GetBindingPower(Operator @operator) =>
        @operator.Symbol switch
        {
            '=' => (1, 2),
            '+' => (3, 4),
            '-' => (3, 4),
            '&' => (5, 6),
            '|' => (5, 6),
            '^' => (5, 6),
            _ => throw new NotSupportedException($"Unsupported operator '{@operator.Symbol}'.")
        };

    [Pure]
    private static FormatException CreateUnexpectedTokenException(Token token) => new($"Unexpected token {token.GetType().Name} {token} at index {token.StartIndex}.");
}