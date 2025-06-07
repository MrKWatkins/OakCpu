using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Lexing;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Parsing;

/// <summary>
/// Parses expressions.
/// </summary>
/// <remarks>Based on https://matklad.github.io/2020/04/13/simple-but-powerful-pratt-parsing.html.</remarks>
public static class Parser
{
    private const int UnaryBindingPower = int.MaxValue;

    [Pure]
    public static IReadOnlyList<Statement> ParseStatements(ParserContext context, string? input)
    {
        var statements = new List<Statement>();
        if (string.IsNullOrWhiteSpace(input))
        {
            return statements;
        }

        try
        {
            using var reader = new StringReader(input);
            var lexer = new Lexer(reader);

            while (!lexer.IsFinished)
            {
                if (lexer.Peek() is EndOfInput)
                {
                    break;
                }

                var parsed = Parse<AstNode>(context, lexer, 0);
                statements.Add(parsed switch
                {
                    Statement statement => statement,
                    Call { Function.Type: DataType.Void } call => new CallStatement(call),
                    _ => throw new InvalidOperationException($"Expression \"{parsed}\" did not parse to a statement.")
                });

                if (lexer.Peek() is SemiColon)
                {
                    lexer.Read();
                }
            }
        }
        catch (Exception exception)
        {
            throw new FormatException($"Exception parsing \"{input}\": {exception.Message}", exception);
        }

        return statements;
    }

    [MustUseReturnValue]
    public static Expression ParseExpression(ParserContext context, string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(input));
        }

        try
        {
            using var reader = new StringReader(input);
            var lexer = new Lexer(reader);
            return Parse<Expression>(context, lexer, 0);
        }
        catch (Exception exception)
        {
            throw new FormatException($"Exception parsing \"{input}\": {exception.Message}", exception);
        }
    }

    [MustUseReturnValue]
    private static TNode Parse<TNode>(ParserContext context, Lexer lexer, int minimumBindingPower)
        where TNode : AstNode
    {
        var parsed = Parse(context, lexer, minimumBindingPower);
        if (parsed is TNode node)
        {
            return node;
        }
        throw new InvalidOperationException($"Expression \"{parsed}\" did not parse to a {typeof(TNode).Name}.");
    }

    [MustUseReturnValue]
    private static AstNode Parse(ParserContext context, Lexer lexer, int minimumBindingPower)
    {
        AstNode left;
        switch (lexer.Read())
        {
            case Lexing.Number number:
                left = new Ast.Number(number.Value);
                break;

            case OpenBracket:
                left = Parse(context, lexer, 0);
                var next = lexer.Read();
                if (next is not CloseBracket)
                {
                    throw CreateUnexpectedTokenException(next);
                }
                break;

            case Identifier identifier:
                left = lexer.Peek() is OpenBracket ? ParseCall(context, lexer, identifier.Name) : ParseIdentifier(context, identifier.Name);
                break;

            case UnaryOperator unaryOperator:
                left = new UnaryOperation(unaryOperator.Operator, Parse(context, lexer, UnaryBindingPower));
                break;

            case var token:
                throw CreateUnexpectedTokenException(token);
        }

        while (true)
        {
            var token = lexer.Peek();
            if (token is EndOfInput or CloseBracket or SemiColon or Comma)
            {
                break;
            }

            if (token is not BinaryOperator @operator)
            {
                throw CreateUnexpectedTokenException(token);
            }

            var (leftBindingPower, rightBindingPower) = @operator.Operator.BindingPower;
            if (leftBindingPower < minimumBindingPower)
            {
                break;
            }

            lexer.Read();

            var right = Parse(context, lexer, rightBindingPower);
            left = @operator.Operator == Operator.Assignment ? new Assignment(left, right) : new BinaryOperation(@operator.Operator, left, right);
        }

        return left;
    }

    [Pure]
    private static AstNode ParseIdentifier(ParserContext context, string identifier)
    {
        if (context.Parameters.Contains(identifier))
        {
            return new ArgumentAccess(identifier);
        }

        if (context.Registers.TryGetValue(identifier, out var register))
        {
            return new RegisterAccess(register);
        }

        if (DataMember.All.TryGetValue(identifier, out var dataMember))
        {
            return new DataMemberAccess(dataMember);
        }

        if (identifier.StartsWith("$"))
        {
            return new TemporaryVariableAccess(identifier.Substring(1));
        }

        if (identifier.StartsWith("flag.") && context.Flags.TryGetValue(identifier.Substring(5), out var flag))
        {
            return new FlagAccess(flag);
        }

        if (identifier.StartsWith("condition.") && context.Conditions.TryGetValue(identifier.Substring(10), out var condition))
        {
            return new ConditionAccess(condition);
        }

        throw new NotSupportedException($"Unsupported identifier {identifier}.");
    }

    [Pure]
    private static AstNode ParseCall(ParserContext context, Lexer lexer, string identifier)
    {
        // Open bracket.
        lexer.Read();

        if (context.Actions.TryGetValue(identifier, out var action))
        {
            if (lexer.Read() is not CloseBracket)
            {
                throw new InvalidOperationException("Expected close bracket.");
            }

            return new RequestAction(action);
        }

        var function = ResolveFunction(context, identifier);

        var arguments = new List<Expression>();
        while (true)
        {
            switch (lexer.Peek())
            {
                case CloseBracket:
                    lexer.Read();
                    return new Call(function, arguments);

                case Comma:
                    lexer.Read();
                    break;

                default:
                    arguments.Add(Parse<Expression>(context, lexer, 0));
                    break;
            }
        }
    }

    [Pure]
    private static Function ResolveFunction(ParserContext context, string identifier)
    {
        if (PreDefinedFunction.All.TryGetValue(identifier, out var preDefinedFunction))
        {
            return preDefinedFunction;
        }

        if (context.UserDefinedFunctions.TryGetValue(identifier, out var function))
        {
            return function;
        }

        throw new NotSupportedException($"Unsupported function {identifier}.");
    }

    [Pure]
    private static FormatException CreateUnexpectedTokenException(Token token) => new($"Unexpected token {token.GetType().Name} {token} at index {token.StartIndex}.");
}