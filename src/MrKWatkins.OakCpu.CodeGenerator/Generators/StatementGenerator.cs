using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

// TODO: Comments.
// TODO: Remove assignment to self.
public abstract class StatementGenerator : Generator
{
    [Pure]
    public static IEnumerable<StatementSyntax> GenerateStatements(IEnumerable<Expression> expressions)
    {
        var commentsAheadOfNextStatement = new List<string>();
        foreach (var expression in expressions)
        {
            foreach (var statement in GenerateStatements(expression, commentsAheadOfNextStatement))
            {
                if (commentsAheadOfNextStatement.Any())
                {
                    yield return statement.WithLeadingTrivia(commentsAheadOfNextStatement.Select(comment => SyntaxFactory.Comment($"// {comment}")));
                    commentsAheadOfNextStatement.Clear();
                }
                else
                {
                    yield return statement;
                }
            }
        }
    }

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateStatements(Expression expression, List<string> commentsAheadOfNextStatement) =>
        expression switch
        {
            Assignment assignment => GenerateStatements(assignment, commentsAheadOfNextStatement),
            OpcodeReadOverlap overlap => GenerateStatements(overlap, commentsAheadOfNextStatement),
            RequestAction requestAction => GenerateStatements(requestAction),
            _ => [SyntaxFactory.ExpressionStatement(GenerateExpression(expression))]
        };

    [Pure]
    private static ExpressionSyntax GenerateExpression(Expression expression) => expression switch
    {
        BinaryOperation binaryOperation => GenerateExpression(binaryOperation),
        DataMemberAccess dataMemberAccess => GenerateExpression(dataMemberAccess),
        Number number => GenerateExpression(number),
        RegisterAccess registerAccess => GenerateExpression(registerAccess),
        _ => throw new NotSupportedException($"The expression type {expression.GetType().Name} is not supported.")
    };

    [Pure]
    private static ExpressionSyntax GenerateExpression(BinaryOperation binaryOperation)
    {
        var left = GenerateExpression(binaryOperation.Left);
        if (binaryOperation.Left is BinaryOperation leftBinary && leftBinary.OperatorPrecedence < binaryOperation.OperatorPrecedence)
        {
            left = SyntaxFactory.ParenthesizedExpression(left);
        }

        var right = GenerateExpression(binaryOperation.Right);
        if (binaryOperation.Right is BinaryOperation rightBinary && rightBinary.OperatorPrecedence < binaryOperation.OperatorPrecedence)
        {
            right = SyntaxFactory.ParenthesizedExpression(right);
        }

        return SyntaxFactory.BinaryExpression(binaryOperation.ExpressionSyntaxKind, left, right);
    }

    [Pure]
    private static ExpressionSyntax GenerateExpression(DataMemberAccess dataMemberAccess) => dataMemberAccess.IdentifierName;

    private static ExpressionSyntax GenerateExpression(Number number) => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(number.NumberString, number.Value));

    [Pure]
    private static ExpressionSyntax GenerateExpression(RegisterAccess registerAccess) => registerAccess.IdentifierName;

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateStatements(Assignment assignment, List<string> commentsAheadOfNextStatement)
    {
        var target = GenerateExpression(assignment.Target);
        var value = GenerateExpression(assignment.Value);

        if (target.ToString() == value.ToString())
        {
            commentsAheadOfNextStatement.Add($"Skipping {assignment}.");
            yield break;
        }

        if (assignment.Value.Type != assignment.Target.Type)
        {
            if (assignment.Value is BinaryOperation)
            {
                value = SyntaxFactory.ParenthesizedExpression(value);
            }

            value = SyntaxFactory.CastExpression(assignment.Target.TypeSyntax, value);
        }

        yield return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, target, value));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateStatements(RequestAction requestAction)
    {
        yield return
            SyntaxFactory.ReturnStatement(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(ActionRequiredEnumName),
                    SyntaxFactory.IdentifierName(requestAction.Name)));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateStatements(OpcodeReadOverlap _, List<string> commentsAheadOfNextStatement)
    {
        commentsAheadOfNextStatement.Add("Opcode read overlap.");

        // Set step = 1 so the counter is after case 0.
        yield return SyntaxFactory.ExpressionStatement(
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName(StepVariableName),
                GetNumericLiteralExpression(1)));

        // goto case 0.
        yield return SyntaxFactory.GotoStatement(SyntaxKind.GotoCaseStatement, SyntaxFactory.Token(SyntaxKind.CaseKeyword), GetNumericLiteralExpression(0));
    }
}