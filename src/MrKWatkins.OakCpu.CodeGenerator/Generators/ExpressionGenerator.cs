using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class ExpressionGenerator : Generator
{
    [Pure]
    public static ExpressionSyntax GenerateExpressionSyntax(Expression expression) => GenerateExpressionSyntax(expression, ImmutableDictionary<string, Expression>.Empty);

    [Pure]
    public static ExpressionSyntax GenerateExpressionSyntax(Expression expression, ImmutableDictionary<string, Expression> scope) => expression switch
    {
        ArgumentAccess argumentAccess => GenerateExpressionSyntax(argumentAccess, scope),
        BinaryOperation binaryOperation => GenerateExpressionSyntax(binaryOperation, scope),
        Call call => GenerateExpressionSyntax(call, scope),
        DataMemberAccess dataMemberAccess => GenerateExpressionSyntax(dataMemberAccess),
        Number number => GenerateExpressionSyntax(number),
        RegisterAccess registerAccess => GenerateExpressionSyntax(registerAccess),
        UnaryOperation unaryOperation => GenerateExpressionSyntax(unaryOperation, scope),
        _ => throw new NotSupportedException($"The expression type {expression.GetType().Name} is not supported.")
    };

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(BinaryOperation binaryOperation, ImmutableDictionary<string, Expression> scope)
    {
        var left = GenerateExpressionSyntax(binaryOperation.Left, scope);
        if (binaryOperation.Left is BinaryOperation leftBinary && leftBinary.OperatorPrecedence < binaryOperation.OperatorPrecedence)
        {
            left = ParenthesizedExpression(left);
        }

        var right = GenerateExpressionSyntax(binaryOperation.Right, scope);
        if (binaryOperation.Right is BinaryOperation rightBinary && rightBinary.OperatorPrecedence < binaryOperation.OperatorPrecedence)
        {
            right = ParenthesizedExpression(right);
        }

        return BinaryExpression(binaryOperation.OperatorSyntaxKind, left, right);
    }

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(Call call, ImmutableDictionary<string, Expression> scope)
    {
        if (call.Function is PreDefinedFunction)
        {
            return GeneratePreDefinedFunctionCallExpressionSyntax(call, scope);
        }

        return GenerateUserDefinedFunctionCallExpressionSyntax(call, scope);
    }

    [Pure]
    private static ExpressionSyntax GenerateUserDefinedFunctionCallExpressionSyntax(Call call, ImmutableDictionary<string, Expression> scope)
    {
        var function = (UserDefinedFunction)call.Function;

        scope = scope.AddRange(call.Function.Parameters.Zip(call.Arguments, (p, a) => new KeyValuePair<string, Expression>(p, a)));

        return ParenthesizedExpression(GenerateExpressionSyntax(function.Expression, scope));
    }

    [Pure]
    private static ExpressionSyntax GeneratePreDefinedFunctionCallExpressionSyntax(Call call, ImmutableDictionary<string, Expression> scope)
    {
        if (call.Function == PreDefinedFunction.PopCount)
        {
            return GeneratePopCountExpressionSyntax(call.Arguments[0], scope);
        }

        throw new NotSupportedException($"The function {call.Function} is not supported.");
    }

    [Pure]
    private static ExpressionSyntax GeneratePopCountExpressionSyntax(Expression argument, ImmutableDictionary<string, Expression> scope) =>
            InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("System.Numerics.BitOperations"),
                        IdentifierName("PopCount")))
                .WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList(
                            Argument(GenerateExpressionSyntax(argument, scope)))));

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(ArgumentAccess argumentAccess, ImmutableDictionary<string, Expression> scope)
    {
        if (!scope.TryGetValue(argumentAccess.Name, out var expression))
        {
            throw new InvalidOperationException($"No value for argument {argumentAccess.Name} found in scope.");
        }

        return GenerateExpressionSyntax(expression);
    }

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(DataMemberAccess dataMemberAccess) => dataMemberAccess.IdentifierName;

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(Number number) => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(number.NumberString, number.Value));

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(RegisterAccess registerAccess) => registerAccess.IdentifierName;

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(UnaryOperation unaryOperation, ImmutableDictionary<string, Expression> scope)
    {
        var expression = GenerateExpressionSyntax(unaryOperation.Expression, scope);
        if (unaryOperation.Expression is BinaryOperation)
        {
            expression = ParenthesizedExpression(expression);
        }

        return PrefixUnaryExpression(unaryOperation.OperatorSyntaxKind, expression);
    }
}