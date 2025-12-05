using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast.Optimization;
using MrKWatkins.OakCpu.CodeGenerator.Language.Lexing;
using Boolean = MrKWatkins.OakCpu.CodeGenerator.Language.Ast.Boolean;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;

/// <summary>
/// Parses expressions.
/// </summary>
/// <remarks>Based on https://matklad.github.io/2020/04/13/simple-but-powerful-pratt-parsing.html.</remarks>
public static class Parser
{
    private const int UnaryBindingPower = int.MaxValue;

    [Pure]
    public static List<Statement> ParseStatements(ParserContext context, string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return [];
        }

        using var reader = new StringReader(input);
        var lexer = new Lexer(reader);

        try
        {
            return ParseStatements(context.WithChildVariableScope(), lexer).ToList();
        }
        catch (Exception exception)
        {
            throw new FormatException($"Exception parsing \"{input}\": {exception.Message}", exception);
        }
    }

    [Pure]
    private static IEnumerable<Statement> ParseStatements(ParserContext context, Lexer lexer)
    {
        while (!lexer.IsFinished)
        {
            if (lexer.Peek() is EndOfInput)
            {
                yield break;
            }

            var parsed = Parse<AstNode>(context, lexer, 0);
            if (lexer.Peek() is not SemiColon)
            {
                throw new InvalidOperationException("Expected semi-colon after statement.");
            }

            lexer.Read();

            yield return Optimizer.Optimize(parsed switch
            {
                IfStatement ifStatement => ParseIfStatements(context, lexer, ifStatement),
                Statement statement => statement,
                Call { Function.Type: DataType.Void } call => new CallStatement(call),
                TemporaryVariableAccess temporaryVariableAccess => new TemporaryVariableDeclarationStatement(temporaryVariableAccess.Variable),
                _ => throw new InvalidOperationException($"Expression \"{parsed}\" did not parse to a statement.")
            });
        }
    }

    [MustUseReturnValue]
    private static IfStatement ParseIfStatements(ParserContext context, Lexer lexer, IfStatement ifStatement)
    {
        var ifStatements = new List<Statement>();
        var elseStatements = new List<Statement>();
        var statements = ifStatements;
        foreach (var statement in ParseStatements(context.WithChildVariableScope(), lexer))
        {
            switch (statement)
            {
                case EndIfStatement:
                    return ifStatement.WithStatements(ifStatements, elseStatements);

                case ElseStatement when statements == elseStatements:
                    throw new InvalidOperationException("Multiple else statements.");

                case ElseStatement:
                    statements = elseStatements;
                    break;

                default:
                    statements.Add(statement);
                    break;
            }
        }

        throw new InvalidOperationException("if without endif.");
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
            return Parse<Expression>(context.WithChildVariableScope(), lexer, 0);
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

            case Keyword keyword:
                left = ParseKeyword(context, lexer, keyword);
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

    [MustUseReturnValue]
    private static AstNode ParseKeyword(ParserContext context, Lexer lexer, Keyword keyword)
    {
        switch (keyword.Name)
        {
            case Keyword.If:
                var condition = Parse(context, lexer, 0);
                if (condition is not Expression expression)
                {
                    throw new FormatException("if condition must be an expression.");
                }
                return new IfStatement(expression);

            case Keyword.Else:
                return ElseStatement.Instance;

            case Keyword.EndIf:
                return EndIfStatement.Instance;

            case Keyword.False:
                return Boolean.False;

            case Keyword.True:
                return Boolean.True;
        }
        throw new NotSupportedException($"Unsupported keyword {keyword.Name}.");
    }

    [Pure]
    private static AstNode ParseIdentifier(ParserContext context, string identifier)
    {
        // TODO: This feels like it could be done in a better way.
        if (context.Arguments.Contains(identifier))
        {
            return new ArgumentAccess(identifier);
        }

        if (context.Configuration.Registers.TryGetValue(identifier, out var register))
        {
            return new RegisterAccess(register);
        }

        if (context.Configuration.AllDataMembers.TryGetValue(identifier, out var dataMember))
        {
            return new DataMemberAccess(dataMember);
        }

        if (identifier.StartsWith("$", StringComparison.Ordinal))
        {
            return new TemporaryVariableAccess(DeclareOrReferenceTemporaryVariable(context, identifier[1..]));
        }

        if (identifier.StartsWith("flag.", StringComparison.Ordinal) && context.Configuration.Flags.TryGetValue(identifier[5..], out var flag))
        {
            return new FlagAccess(flag);
        }

        if (identifier.StartsWith("condition.", StringComparison.Ordinal) && context.Configuration.Conditions.TryGetValue(identifier[10..], out var condition))
        {
            return new ConditionAccess(condition);
        }

        if (identifier.StartsWith("action.", StringComparison.Ordinal) && context.Configuration.Actions.TryGetValue(identifier[7..], out var action))
        {
            return new ActionAccess(action);
        }

        if (identifier.StartsWith("opcode_table.", StringComparison.Ordinal) && context.Configuration.OpcodeStepTables.Custom.TryGetValue(identifier[13..], out var opcodeStepTable))
        {
            return new OpcodeStepTableAccess(opcodeStepTable);
        }

        throw new NotSupportedException($"Unsupported identifier {identifier}.");
    }

    [Pure]
    private static AstNode ParseCall(ParserContext context, Lexer lexer, string identifier)
    {
        // Open bracket.
        lexer.Read();

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
    private static Function ResolveFunction(ParserContext context, string identifier) =>
        context.Configuration.AllFunctions.TryGetValue(identifier, out var function) ? function : throw new NotSupportedException($"Unsupported function {identifier}.");

    [Pure]
    private static FormatException CreateUnexpectedTokenException(Token token) => new($"Unexpected token {token.GetType().Name} {token} at index {token.StartIndex}.");

    [MustUseReturnValue]
    private static TemporaryVariable DeclareOrReferenceTemporaryVariable(ParserContext context, string name)
    {
        if (!context.TemporaryVariables.TryGetValue(name, out var variable))
        {
            variable = new TemporaryVariable(name);
            context.TemporaryVariables.Add(name, variable);
        }

        return variable;
    }
}