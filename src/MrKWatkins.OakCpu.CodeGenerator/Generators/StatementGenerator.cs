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
        var context = new StepContext(input, step);
        foreach (var stepStatement in step.Statements)
        {
            foreach (var statement in GenerateStatementSyntaxes(context, stepStatement))
            {
                if (context.CommentsAheadOfNextStatement.Any())
                {
                    yield return statement.WithLeadingTrivia(context.CommentsAheadOfNextStatement.Select(comment => Comment($"// {comment}")));
                    context.CommentsAheadOfNextStatement.Clear();
                }
                else
                {
                    yield return statement;
                }
            }
        }
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateStatementSyntaxes(StepContext context, Statement statement) =>
        statement switch
        {
            Assignment assignment => GenerateStatementSyntaxes(context, assignment),
            MoveToOpcodeRead => GenerateMoveToOpcodeReadStatementSyntaxes(),
            OpcodeJump => GenerateOpcodeJump(),
            OverlappedOpcodeRead => GenerateOverlappedOpcodeReadStatementSyntaxes(context),
            RequestAction requestAction => GenerateStatementSyntaxes(requestAction),
            ExpressionStatement { Expression: Call call } => GenerateCallStatementSyntax(context, call),
            _ => throw new NotSupportedException($"The statement type {statement.GetType().Name} is not supported.")
        };

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateCallStatementSyntax(StepContext context, Call call)
    {
        if (call.Function == PreDefinedFunction.Flags)
        {
            return FlagsGenerator.GenerateFlagsStatements(context);
        }

        return [];
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateStatementSyntaxes(StepContext context, Assignment assignment)
    {
        // TODO: AssignmentEqual if possible, i.e. A |= D rather than A = (byte)(A | D). Probably generates the same code though...

        var value = ExpressionGenerator.GenerateExpressionSyntax(context, assignment.Value);

        if (assignment.Value.Type != assignment.Target.Type)
        {
            if (assignment.Value is BinaryOperation)
            {
                value = ParenthesizedExpression(value);
            }

            value = CastExpression(assignment.Target.TypeSyntax, value);
        }


        // If we're assigning to a temporary variable, initialize if necessary.
        ExpressionSyntax target;
        if (assignment.Target is TemporaryVariableAccess temporaryVariableAccess)
        {
            if (!context.TemporaryVariableNames.TryGetValue(temporaryVariableAccess.TemporaryVariable, out var temporaryName))
            {
                temporaryName = context.Step.ScopedVariableName(temporaryVariableAccess.TemporaryVariable.Name);
                context.TemporaryVariableNames[temporaryVariableAccess.TemporaryVariable] = temporaryName;
                yield return InitializeVariableStatement(temporaryName, value);
                yield break;
            }

            target = IdentifierName(temporaryName);
        }
        else
        {
            target = ExpressionGenerator.GenerateExpressionSyntax(context, assignment.Target);
        }

        if (target.ToString() == value.ToString())
        {
            context.CommentsAheadOfNextStatement.Add($"Skipping {assignment}.");
            yield break;
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
    private static IEnumerable<StatementSyntax> GenerateOverlappedOpcodeReadStatementSyntaxes(StepContext context)
    {
        context.CommentsAheadOfNextStatement.Add("Overlapped opcode read.");

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