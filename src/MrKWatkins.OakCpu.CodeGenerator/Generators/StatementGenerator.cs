using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class StatementGenerator : Generator
{
    [Pure]
    public static IEnumerable<StatementSyntax> GenerateStatementSyntaxes(GeneratorInput input, Step step)
    {
        var context = new StepContext(input, step);

        if (step.Instruction is { Prefix: not null } && step.Instruction.Steps[0] == step)
        {
            yield return GenerateResetPrefix();
        }

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
            Assignment assignment => GenerateAssignment(context, assignment),
            IfStatement ifStatement => GenerateIf(context, ifStatement),
            MoveToOpcodeRead => GenerateMoveToOpcodeReadStatement(),
            MoveToOpcode => GenerateMoveToOpcode(),
            OverlappedOpcodeRead => GenerateOverlappedOpcodeRead(context),
            RequestAction requestAction => GenerateRequestAction(requestAction),
            CallStatement callStatement => GenerateCall(context, callStatement),
            _ => throw new NotSupportedException($"The statement type {statement.GetType().Name} is not supported.")
        };

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateCall(StepContext context, CallStatement callStatement)
    {
        if (callStatement.Call.Function == PreDefinedFunction.Flags)
        {
            return FlagsGenerator.GenerateFlagsStatements(context);
        }
        if (callStatement.Call.Function == PreDefinedFunction.FinishInstruction)
        {
            return GenerateFinishInstruction(context, callStatement.Call);
        }

        throw new NotSupportedException($"The function {callStatement.Call.Function} is not supported.");
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateFinishInstruction(StepContext context, Call call)
    {
        yield return CreateSetStep(0).WithLeadingTrivia(Comment("// Finish instruction."));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateAssignment(StepContext context, Assignment assignment)
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
            if (context.InitializedTemporaryVariables.Add(temporaryVariableAccess.Name))
            {
                yield return InitializeVariableStatement(temporaryVariableAccess.Name, value);
                yield break;
            }

            target = temporaryVariableAccess.Identifier;
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
    private static IEnumerable<StatementSyntax> GenerateIf(StepContext context, IfStatement ifStatement)
    {
        var condition = ExpressionGenerator.GenerateExpressionSyntax(context.WithBooleanContext(), ifStatement.Condition);

        var ifBlock = Block(ifStatement.IfStatements.SelectMany(statement => GenerateStatementSyntaxes(context, statement)));

        if (ifStatement.ElseStatements.Any())
        {
            var elseBlock = Block(ifStatement.ElseStatements.SelectMany(statement => GenerateStatementSyntaxes(context, statement)));

            yield return IfStatement(condition, ifBlock, ElseClause(elseBlock));
        }
        else
        {
            yield return IfStatement(condition, ifBlock);
        }
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateRequestAction(RequestAction requestAction)
    {
        yield return
            ReturnStatement(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(ActionRequiredEnumName),
                    IdentifierName(requestAction.Action.EnumName)));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToOpcodeReadStatement()
    {
        yield return CreateSetStep(0);
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToOpcode()
    {
        // TODO: Version without bounds checks, don't rely on the JIT.
        var getOpcode =
            ElementAccessExpression(
                ElementAccessExpression(
                    IdentifierName(DataMember.OpcodeStepTable.Name),
                    BracketedArgumentList([Argument(IdentifierName(DataMember.Prefix.Name))])),
                BracketedArgumentList([Argument(IdentifierName(DataMember.Data.Name))]));

        yield return CreateSetStep(getOpcode);
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateOverlappedOpcodeRead(StepContext context)
    {
        context.CommentsAheadOfNextStatement.Add("Overlapped opcode read.");

        // Set step = 1 so we start on step 1 after the next Step() call.
        yield return CreateSetStep(1);

        // goto case 0 to perform step 0.
        yield return GotoStatement(SyntaxKind.GotoCaseStatement, Token(SyntaxKind.CaseKeyword), GenerateNumericLiteralExpression(0));
    }

    [Pure]
    private static StatementSyntax GenerateResetPrefix() =>
        ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(DataMember.Prefix.Name), GenerateNumericLiteralExpression(0)));

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