using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class StatementGenerator : Generator
{
    [Pure]
    public static IEnumerable<StatementSyntax> GenerateStatementSyntaxes(GeneratorInput input, Step step)
    {
        var commentsAheadOfNextStatement = new List<string>();
        foreach (var stepStatement in step.Statements)
        {
            foreach (var statement in GenerateStatementSyntaxes(input, step, stepStatement, commentsAheadOfNextStatement))
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
    public static IEnumerable<StatementSyntax> GenerateStatementSyntaxes(GeneratorInput input, Step step, Statement statement, List<string> commentsAheadOfNextStatement) =>
        statement switch
        {
            Assignment assignment => GenerateStatementSyntaxes(assignment, commentsAheadOfNextStatement),
            MoveToOpcodeRead => GenerateMoveToOpcodeReadStatementSyntaxes(),
            OpcodeJump => GenerateOpcodeJump(),
            OverlappedOpcodeRead => GenerateOverlappedOpcodeReadStatementSyntaxes(commentsAheadOfNextStatement),
            RequestAction requestAction => GenerateStatementSyntaxes(requestAction),
            ExpressionStatement { Expression: Call call } => GenerateCallStatementSyntax(input, step, call),
            _ => throw new NotSupportedException($"The statement type {statement.GetType().Name} is not supported.")
        };

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateCallStatementSyntax(GeneratorInput input, Step step, Call call)
    {
        if (call.Function == PreDefinedFunction.Flags)
        {
            return FlagsGenerator.GenerateFlagsStatements(input, step);
        }

        return [];
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateStatementSyntaxes(Assignment assignment, List<string> commentsAheadOfNextStatement)
    {
        // TODO: AssignmentEqual if possible, i.e. A |= D rather than A = (byte)(A | D). Probably generates the same code though...
        var target = ExpressionGenerator.GenerateExpressionSyntax(assignment.Target);
        var value = ExpressionGenerator.GenerateExpressionSyntax(assignment.Value);

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
                IdentifierName(DataMember.OpcodeStepTable.Name),
                BracketedArgumentList(
                    SingletonSeparatedList(
                        Argument(
                            IdentifierName(DataMember.Opcode.Name)))));

        yield return CreateSetStep(getOpcode);
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateOverlappedOpcodeReadStatementSyntaxes(List<string> commentsAheadOfNextStatement)
    {
        commentsAheadOfNextStatement.Add("Overlapped opcode read.");

        // Set step = 1 so we start on step 1 after the next Step() call.
        yield return CreateSetStep(1);

        // goto case 0 to perform step 0.
        yield return GotoStatement(SyntaxKind.GotoCaseStatement, Token(SyntaxKind.CaseKeyword), GenerateNumericLiteralExpression(0));
    }

    [Pure]
    private static StatementSyntax CreateSetStep(int step) => CreateSetStep(GenerateNumericLiteralExpression(step));

    [Pure]
    private static StatementSyntax CreateSetStep(ExpressionSyntax value) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(StepVariableName),
                value));
}