using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class StatementGenerator : Generator
{
    [Pure]
    public static StatementSyntax GenerateStatement(Expression expression) => SyntaxFactory.ExpressionStatement(Generate(expression));

    [Pure]
    private static ExpressionSyntax Generate(Expression expression) => expression switch
    {
        Assignment assignment => Generate(assignment),
        BinaryOperation binaryOperation => Generate(binaryOperation),
        DataMemberAccess dataMemberAccess => Generate(dataMemberAccess),
        Number number => Generate(number),
        RegisterAccess registerAccess => Generate(registerAccess),
        RequestAction requestAction => Generate(requestAction),
        _ => throw new NotSupportedException($"The expression type {expression.GetType().Name} is not supported.")
    };

    [Pure]
    private static ExpressionSyntax Generate(Assignment assignment)
    {
        var target = Generate(assignment.Target);
        var value = Generate(assignment.Value);
        if (assignment.Value.Type != assignment.Target.Type)
        {
            if (assignment.Value is BinaryOperation)
            {
                value = SyntaxFactory.ParenthesizedExpression(value);
            }

            value = SyntaxFactory.CastExpression(assignment.Target.TypeSyntax, value);
        }

        return SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, target, value);
    }

    [Pure]
    private static ExpressionSyntax Generate(BinaryOperation binaryOperation)
    {
        var left = Generate(binaryOperation.Left);
        if (binaryOperation.Left is BinaryOperation leftBinary && leftBinary.OperatorPrecedence < binaryOperation.OperatorPrecedence)
        {
            left = SyntaxFactory.ParenthesizedExpression(left);
        }

        var right = Generate(binaryOperation.Right);
        if (binaryOperation.Right is BinaryOperation rightBinary && rightBinary.OperatorPrecedence < binaryOperation.OperatorPrecedence)
        {
            right = SyntaxFactory.ParenthesizedExpression(right);
        }

        return SyntaxFactory.BinaryExpression(binaryOperation.ExpressionSyntaxKind, left, right);
    }

    [Pure]
    private static ExpressionSyntax Generate(DataMemberAccess dataMemberAccess) => dataMemberAccess.IdentifierName;

    [Pure]
    private static ExpressionSyntax Generate(Number number) => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(number.NumberString, number.Value));

    [Pure]
    private static ExpressionSyntax Generate(RegisterAccess registerAccess) => registerAccess.IdentifierName;

    [Pure]
    private static ExpressionSyntax Generate(RequestAction requestAction) =>
        SyntaxFactory.AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            SyntaxFactory.IdentifierName(ActionVariableName),
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(ActionRequiredEnumName),
                SyntaxFactory.IdentifierName(requestAction.Name)));
}