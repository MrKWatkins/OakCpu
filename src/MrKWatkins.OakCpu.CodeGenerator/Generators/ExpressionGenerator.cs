using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using Boolean = MrKWatkins.OakCpu.CodeGenerator.Language.Ast.Boolean;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class ExpressionGenerator : Generator
{
    [Pure]
    public static ExpressionSyntax GenerateExpressionSyntax(StatementGeneratorContext context, Expression expression) => expression switch
    {
        ArgumentAccess argumentAccess => GenerateArgumentAccess(context, argumentAccess),
        BinaryOperation binaryOperation => GenerateBinaryOperation(context, binaryOperation),
        Boolean boolean => GenerateBoolean(boolean),
        Call call => GenerateCall(context, call),
        ConditionAccess conditionAccess => GenerateConditionAccess(context, conditionAccess),
        DataMemberAccess dataMemberAccess => GenerateDataMemberAccess(dataMemberAccess),
        FlagAccess flagAccess => GenerateFlagAccess(context, flagAccess),
        Number number => GenerateNumber(number),
        RegisterAccess registerAccess => GenerateRegisterAccess(registerAccess),
        TemporaryVariableAccess temporaryVariableAccess => GenerateTemporaryVariableAccess(context, temporaryVariableAccess),
        UnaryOperation unaryOperation => GenerateUnaryOperation(context, unaryOperation),
        _ => throw new NotSupportedException($"The expression type {expression.GetType().Name} is not supported.")
    };

    [Pure]
    private static ExpressionSyntax GenerateBinaryOperation(StatementGeneratorContext context, BinaryOperation binaryOperation)
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
    private static ExpressionSyntax GenerateCall(StatementGeneratorContext context, Call call) =>
        call.Function is PreDefinedFunction
            ? GeneratePreDefinedFunctionCallExpressionSyntax(context, call)
            : GenerateUserDefinedFunctionCallExpressionSyntax(context, call);

    [Pure]
    private static ExpressionSyntax GenerateUserDefinedFunctionCallExpressionSyntax(StatementGeneratorContext context, Call call)
    {
        var function = (UserDefinedFunction)call.Function;

        var childContext = context.WithArguments(function.Parameters, call.Arguments);

        return ParenthesizedExpression(GenerateExpressionSyntax(childContext, function.Expression));
    }

    [Pure]
    private static ExpressionSyntax GeneratePreDefinedFunctionCallExpressionSyntax(StatementGeneratorContext context, Call call)
    {
        if (call.Function == PreDefinedFunction.InstructionUpdatesFlags)
        {
            return GenerateInstructionUpdatesFlagsExpressionSyntax(context);
        }
        if (call.Function == PreDefinedFunction.IsZero)
        {
            return GenerateIsZeroExpressionSyntax(context, call.Arguments[0]);
        }
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
    private static ExpressionSyntax GeneratePopCountExpressionSyntax(StatementGeneratorContext context, Expression argument)
    {
        context.GeneratorContext.RequiredUsings.Add("System.Numerics");

        var argumentExpression = GenerateExpressionSyntax(context, argument);

        // PopCount has overloads for unsigned types, so we only need to cast if the argument is signed. Need to make sure
        // we check the type of the actual value being passed in the argument scope.
        var type = argument is Access access ? context.ArgumentScope[access.Name].Type : argument.Type;
        if (type.IsSigned)
        {
            argumentExpression = CastExpression(PreDefinedFunction.PopCount.TypeSyntax, ParenthesizedExpression(argumentExpression));
        }

        return InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("BitOperations"),
                    IdentifierName("PopCount")))
            .WithArgumentList(
                ArgumentList(
                    SingletonSeparatedList(
                        Argument(argumentExpression))));
    }

    [Pure]
    private static ExpressionSyntax GenerateSignedExpressionSyntax(StatementGeneratorContext context, Expression argument)
    {
        var expression = GenerateExpressionSyntax(context, argument);
        if (argument is not Access)
        {
            expression = ParenthesizedExpression(expression);
        }
        return CastExpression(PreDefinedFunction.Signed.TypeSyntax, expression);
    }

    [Pure]
    private static ExpressionSyntax GenerateInstructionUpdatesFlagsExpressionSyntax(StatementGeneratorContext context)
    {
        if (context.Step?.Sequence is not Instruction instruction)
        {
            throw new InvalidOperationException("Cannot use flags() outside of an instruction.");
        }
        return LiteralExpression(instruction.UpdatesFlags ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression);
    }

    [Pure]
    private static ExpressionSyntax GenerateIsZeroExpressionSyntax(StatementGeneratorContext context, Expression argument) =>
        BinaryExpression(SyntaxKind.EqualsExpression, GenerateExpressionSyntax(context, argument), GenerateNumericLiteralExpression(0));

    [Pure]
    private static ExpressionSyntax GenerateArgumentAccess(StatementGeneratorContext context, ArgumentAccess argumentAccess)
    {
        if (!context.ArgumentScope.TryGetValue(argumentAccess.Name, out var expression))
        {
            throw new InvalidOperationException($"No value for argument {argumentAccess.Name} found in scope.");
        }

        return GenerateExpressionSyntax(context, expression);
    }

    [Pure]
    private static ExpressionSyntax GenerateDataMemberAccess(DataMemberAccess dataMemberAccess) => EmulatorMemberIdentifier(dataMemberAccess.DataMember.FieldName);

    [Pure]
    private static ExpressionSyntax GenerateBoolean(Boolean boolean) => LiteralExpression(boolean.Value ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression);

    [Pure]
    private static ExpressionSyntax GenerateNumber(Number number) => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(number.NumberString, number.Value));

    [Pure]
    private static ExpressionSyntax GenerateRegisterAccess(RegisterAccess registerAccess) => EmulatorMemberIdentifier(registerAccess.Register.FieldName);

    [Pure]
    private static ExpressionSyntax GenerateConditionAccess(StatementGeneratorContext context, ConditionAccess conditionAccess, bool invert = false)
    {
        var bitMask = (byte)(1 << conditionAccess.Condition.Flag.Index);

        // Isolate the flag bit.
        var isolate = ParenthesizedExpression(BinaryExpression(SyntaxKind.BitwiseAndExpression, EmulatorMemberIdentifier(context.Configuration.FlagsRegister.FieldName), GenerateBinaryLiteralExpression(bitMask)));

        // Comparison.
        var positive = invert ? conditionAccess.Condition.IsNot : !conditionAccess.Condition.IsNot;
        var comparison = BinaryExpression(SyntaxKind.EqualsExpression, isolate, GenerateBinaryLiteralExpression(positive ? bitMask : (byte)0));

        return comparison.WithTrailingTrivia(Comment($"/* {(invert ? "!" : "")}condition.{conditionAccess.Condition.Name} */"));
    }

    [Pure]
    private static ExpressionSyntax GenerateFlagAccess(StatementGeneratorContext context, FlagAccess flagAccess)
    {
        var bitMask = (byte)(1 << flagAccess.Flag.Index);

        // Isolate the flag bit.
        var expression = BinaryExpression(SyntaxKind.BitwiseAndExpression, EmulatorMemberIdentifier(context.Configuration.FlagsRegister.FieldName), GenerateBinaryLiteralExpression(bitMask));

        // If we're in a boolean context, return a bool, otherwise return an int.
        if (context.InBooleanContext)
        {
            expression = BinaryExpression(SyntaxKind.EqualsExpression, ParenthesizedExpression(expression), GenerateBinaryLiteralExpression(bitMask));
        }
        // Else if the index is not 0, shift the bit to the rightmost position.
        else if (flagAccess.Flag.Index != 0)
        {
            expression = BinaryExpression(SyntaxKind.RightShiftExpression, expression, GenerateNumericLiteralExpression(flagAccess.Flag.Index));
        }

        return ParenthesizedExpression(expression.WithTrailingTrivia(Comment($"/* flag.{flagAccess.Flag.Name} */")));
    }

    [Pure]
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private static ExpressionSyntax GenerateTemporaryVariableAccess(StatementGeneratorContext context, TemporaryVariableAccess temporaryVariableAccess) =>
        context.InitializedTemporaryVariables.Contains(temporaryVariableAccess.Name)
            ? temporaryVariableAccess.Identifier
            : throw new InvalidOperationException($"The temporary variable {temporaryVariableAccess.Name} has not been initialized.");

    [Pure]
    private static ExpressionSyntax GenerateUnaryOperation(StatementGeneratorContext context, UnaryOperation unaryOperation)
    {
        // Special case for !condition.
        if (unaryOperation.Expression is ConditionAccess conditionAccess)
        {
            return GenerateConditionAccess(context, conditionAccess, true);
        }

        var expression = GenerateExpressionSyntax(context, unaryOperation.Expression);
        if (unaryOperation.Expression is BinaryOperation)
        {
            expression = ParenthesizedExpression(expression);
        }

        return PrefixUnaryExpression(unaryOperation.Operator.SyntaxKind, expression);
    }
}