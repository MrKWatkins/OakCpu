using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class StatementGenerator : Generator
{
    [Pure]
    public static IEnumerable<StatementSyntax> GenerateStatementSyntaxes(IEnumerable<Statement> expressions)
    {
        var commentsAheadOfNextStatement = new List<string>();
        foreach (var expression in expressions)
        {
            foreach (var statement in GenerateStatementSyntaxes(expression, commentsAheadOfNextStatement))
            {
                if (commentsAheadOfNextStatement.Any())
                {
                    yield return statement.WithLeadingTrivia(commentsAheadOfNextStatement.Select(comment => Comment($"// {comment}")));
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
    public static IEnumerable<StatementSyntax> GenerateStatementSyntaxes(Statement statement, List<string> commentsAheadOfNextStatement) =>
        statement switch
        {
            Assignment assignment => GenerateStatementSyntaxes(assignment, commentsAheadOfNextStatement),
            MoveToOpcodeRead => GenerateMoveToOpcodeReadStatementSyntaxes(),
            OpcodeJump => GenerateOpcodeJump(),
            OverlappedOpcodeRead => GenerateOverlappedOpcodeReadStatementSyntaxes(commentsAheadOfNextStatement),
            RequestAction requestAction => GenerateStatementSyntaxes(requestAction),
            _ => [ExpressionStatement(GenerateExpressionSyntax(statement))]
        };

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(Expression expression) => expression switch
    {
        BinaryOperation binaryOperation => GenerateExpressionSyntax(binaryOperation),
        DataMemberAccess dataMemberAccess => GenerateExpressionSyntax(dataMemberAccess),
        Number number => GenerateExpressionSyntax(number),
        RegisterAccess registerAccess => GenerateExpressionSyntax(registerAccess),
        _ => throw new NotSupportedException($"The expression type {expression.GetType().Name} is not supported.")
    };

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(BinaryOperation binaryOperation)
    {
        var left = GenerateExpressionSyntax(binaryOperation.Left);
        if (binaryOperation.Left is BinaryOperation leftBinary && leftBinary.OperatorPrecedence < binaryOperation.OperatorPrecedence)
        {
            left = ParenthesizedExpression(left);
        }

        var right = GenerateExpressionSyntax(binaryOperation.Right);
        if (binaryOperation.Right is BinaryOperation rightBinary && rightBinary.OperatorPrecedence < binaryOperation.OperatorPrecedence)
        {
            right = ParenthesizedExpression(right);
        }

        return BinaryExpression(binaryOperation.ExpressionSyntaxKind, left, right);
    }

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(DataMemberAccess dataMemberAccess) => dataMemberAccess.IdentifierName;

    private static ExpressionSyntax GenerateExpressionSyntax(Number number) => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(number.NumberString, number.Value));

    [Pure]
    private static ExpressionSyntax GenerateExpressionSyntax(RegisterAccess registerAccess) => registerAccess.IdentifierName;

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateStatementSyntaxes(Assignment assignment, List<string> commentsAheadOfNextStatement)
    {
        var target = GenerateExpressionSyntax(assignment.Target);
        var value = GenerateExpressionSyntax(assignment.Value);

        if (target.ToString() == value.ToString())
        {
            commentsAheadOfNextStatement.Add($"Skipping {assignment}.");
            yield break;
        }

        if (assignment.Value.Type != assignment.Target.Type)
        {
            if (assignment.Value is BinaryOperation)
            {
                value = ParenthesizedExpression(value);
            }

            value = CastExpression(assignment.Target.TypeSyntax, value);
        }

        yield return ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, target, value));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateStatementSyntaxes(RequestAction requestAction)
    {
        yield return
            ReturnStatement(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(ActionRequiredEnumName),
                    IdentifierName(requestAction.Name)));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToOpcodeReadStatementSyntaxes()
    {
        yield return CreateSetStep(0);
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateOpcodeJump()
    {
        // TODO: Version without bounds checks, don't rely on the JIT. Maybe wait until prefixes are added.
        var getOpcode = ElementAccessExpression(
                IdentifierName(KnownDataMember.OpcodeStepTable.Name),
                BracketedArgumentList(
                    SingletonSeparatedList(
                        Argument(
                            IdentifierName(KnownDataMember.Opcode.Name)))));

        yield return CreateSetStep(getOpcode);
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateOverlappedOpcodeReadStatementSyntaxes(List<string> commentsAheadOfNextStatement)
    {
        commentsAheadOfNextStatement.Add("Overlapped opcode read.");

        // Set step = 1 so we start on step 1 after the next Step() call.
        yield return CreateSetStep(1);

        // goto case 0 to perform step 0.
        yield return GotoStatement(SyntaxKind.GotoCaseStatement, Token(SyntaxKind.CaseKeyword), GetNumericLiteralExpression(0));
    }

    [Pure]
    private static StatementSyntax CreateSetStep(int step) => CreateSetStep(GetNumericLiteralExpression(step));

    [Pure]
    private static StatementSyntax CreateSetStep(ExpressionSyntax value) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(StepVariableName),
                value));
}