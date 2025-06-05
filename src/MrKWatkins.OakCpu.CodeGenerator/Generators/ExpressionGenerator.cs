using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class ExpressionGenerator : Generator
{
    [Pure]
    public static ExpressionSyntax GenerateExpressionSyntax(StepContext context, Expression expression) => expression switch
    {
        ArgumentAccess argumentAccess => GenerateExpressionSyntax(context, argumentAccess),
        BinaryOperation binaryOperation => GenerateExpressionSyntax(context, binaryOperation),
        Call call => GenerateExpressionSyntax(context, call),
        DataMemberAccess dataMemberAccess => GenerateExpressionSyntax(dataMemberAccess),
        ConditionAccess conditionAccess => GenerateExpressionSyntax(context, conditionAccess),
        FlagAccess flagAccess => GenerateExpressionSyntax(context, flagAccess),
        Number number => GenerateExpressionSyntax(number),
        RegisterAccess registerAccess => GenerateExpressionSyntax(registerAccess),
        TemporaryVariableAccess temporaryVariableAccess => GenerateExpressionSyntax(context, temporaryVariableAccess),
        UnaryOperation unaryOperation => GenerateExpressionSyntax(context, unaryOperation),
        _ => throw new NotSupportedException($"The expression type {expression.GetType().Name} is not supported.")
    };

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(StepContext context, BinaryOperation binaryOperation)
    {
        var left = GenerateExpressionSyntax(context, binaryOperation.Left);
        if (binaryOperation.Left is BinaryOperation leftBinary && leftBinary.Operator.Precedence < binaryOperation.Operator.Precedence)
        {
            left = ParenthesizedExpression(left);
        }

        var right = GenerateExpressionSyntax(context, binaryOperation.Right);
        if (binaryOperation.Right is BinaryOperation rightBinary && rightBinary.Operator.Precedence < binaryOperation.Operator.Precedence)
        {
            right = ParenthesizedExpression(right);
        }

        return BinaryExpression(binaryOperation.Operator.SyntaxKind, left, right);
    }

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(StepContext context, Call call)
    {
        if (call.Function is PreDefinedFunction)
        {
            return GeneratePreDefinedFunctionCallExpressionSyntax(context, call);
        }

        return GenerateUserDefinedFunctionCallExpressionSyntax(context, call);
    }

    [Pure]
    private static ExpressionSyntax GenerateUserDefinedFunctionCallExpressionSyntax(StepContext context, Call call)
    {
        var function = (UserDefinedFunction)call.Function;

        var childContext = context.WithArguments(function.Parameters, call.Arguments);

        return ParenthesizedExpression(GenerateExpressionSyntax(childContext, function.Expression));
    }

    [Pure]
    private static ExpressionSyntax GeneratePreDefinedFunctionCallExpressionSyntax(StepContext context, Call call)
    {
        if (call.Function == PreDefinedFunction.PopCount)
        {
            return GeneratePopCountExpressionSyntax(context, call.Arguments[0]);
        }
        if (call.Function == PreDefinedFunction.Signed)
        {
            return GenerateSignedExpressionSyntax(context, call.Arguments[0]);
        }

        throw new NotSupportedException($"The function {call.Function} is not supported.");
    }

    [Pure]
    private static ExpressionSyntax GeneratePopCountExpressionSyntax(StepContext context, Expression argument) =>
            InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("System.Numerics.BitOperations"),
                        IdentifierName("PopCount")))
                .WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList(
                            Argument(GenerateExpressionSyntax(context, argument)))));

    [Pure]
    private static ExpressionSyntax GenerateSignedExpressionSyntax(StepContext context, Expression argument)
    {
        var expression = GenerateExpressionSyntax(context, argument);
        if (argument is not Access)
        {
            expression = ParenthesizedExpression(expression);
        }
        return CastExpression(PreDefinedFunction.Signed.TypeSyntax, expression);
    }

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(StepContext context, ArgumentAccess argumentAccess)
    {
        if (!context.ArgumentScope.TryGetValue(argumentAccess.Name, out var expression))
        {
            throw new InvalidOperationException($"No value for argument {argumentAccess.Name} found in scope.");
        }

        return GenerateExpressionSyntax(context, expression);
    }

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(DataMemberAccess dataMemberAccess) => dataMemberAccess.Identifier;

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(Number number) => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(number.NumberString, number.Value));

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(RegisterAccess registerAccess) => registerAccess.Identifier;

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(StepContext context, ConditionAccess conditionAccess, bool invert = false)
    {
        var bitMask = (byte)(1 << conditionAccess.Condition.Flag.Index);

        // Isolate the flag bit.
        var isolate = ParenthesizedExpression(BinaryExpression(SyntaxKind.BitwiseAndExpression, IdentifierName(context.Input.FlagsRegister.FieldName), GenerateBinaryLiteralExpression(bitMask)));

        // Comparison.
        var positive = invert ? conditionAccess.Condition.IsNot : !conditionAccess.Condition.IsNot;
        var comparison = BinaryExpression(SyntaxKind.EqualsExpression, isolate, GenerateBinaryLiteralExpression(positive ? bitMask : (byte)0));

        return comparison.WithTrailingTrivia(Comment($"/* {(invert ? "!" : "")}condition.{conditionAccess.Condition.Name} */"));
    }

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(StepContext context, FlagAccess flagAccess)
    {
        var bitMask = (byte)(1 << flagAccess.Flag.Index);

        // Isolate the flag bit.
        var expression = BinaryExpression(SyntaxKind.BitwiseAndExpression, IdentifierName(context.Input.FlagsRegister.FieldName), GenerateBinaryLiteralExpression(bitMask));

        // If the index is not 0, shift the bit to the rightmost position.
        if (flagAccess.Flag.Index != 0)
        {
            expression = BinaryExpression(SyntaxKind.RightShiftExpression, expression, GenerateNumericLiteralExpression(flagAccess.Flag.Index));
        }

        return ParenthesizedExpression(expression.WithTrailingTrivia(Comment($"/* flag.{flagAccess.Flag.Name} */")));
    }

    [Pure]
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private static ExpressionSyntax GenerateExpressionSyntax(StepContext context, TemporaryVariableAccess temporaryVariableAccess)
    {
        if (!context.InitializedTemporaryVariables.Contains(temporaryVariableAccess.Name))
        {
            throw new InvalidOperationException($"The temporary variable {temporaryVariableAccess.Name} has not been initialized.");
        }

        return temporaryVariableAccess.Identifier;
    }

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(StepContext context, UnaryOperation unaryOperation)
    {
        // Special case for !condition.
        if (unaryOperation.Expression is ConditionAccess conditionAccess)
        {
            return GenerateExpressionSyntax(context, conditionAccess, true);
        }

        var expression = GenerateExpressionSyntax(context, unaryOperation.Expression);
        if (unaryOperation.Expression is BinaryOperation)
        {
            expression = ParenthesizedExpression(expression);
        }

        return PrefixUnaryExpression(unaryOperation.Operator.SyntaxKind, expression);
    }
}