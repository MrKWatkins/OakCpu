using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

// TODO: Optimise A ^ A to 0.
public abstract class StepGenerator : Generator
{
    [Pure]
    public static IEnumerable<StatementSyntax> GenerateStatements(GeneratorContext input, Step step)
    {
        var context = new StepContext(input, step);

        if (step.DoesNothing)
        {
            throw new InvalidOperationException("Trying to generate statements for a step that does nothing.");
        }

        // Reset the step table if we've started a prefixed instruction.
        if (step.RequiresPrefixReset)
        {
            yield return GenerateSetOpcodeStepTable(input.Configuration.OpcodeStepTables.NoPrefix);
        }

        foreach (var stepStatement in step.Statements)
        {
            foreach (var statement in GenerateStatements(context, stepStatement))
            {
                yield return statement;
            }
        }

        var trailingStatements = step.NextOpcode switch
        {
            NextOpcodeMode.Read => GenerateMoveToOpcodeRead(),
            NextOpcodeMode.Overlapped => GenerateOverlappedOpcodeRead(context),
            NextOpcodeMode.Custom => [],
            null => [],
            _ => throw new NotSupportedException($"The next opcode mode {step.NextOpcode} is not supported.")
        };

        foreach (var statement in trailingStatements)
        {
            yield return statement;
        }
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateStatements(StepContext context, Statement statement) =>
        statement switch
        {
            Assignment assignment => GenerateAssignment(context, assignment),
            IfStatement ifStatement => GenerateIf(context, ifStatement),
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
            return GenerateFinishInstruction(context);
        }
        if (callStatement.Call.Function == PreDefinedFunction.MoveToOpcode)
        {
            return GenerateMoveToOpcode(context);
        }
        if (callStatement.Call.Function == PreDefinedFunction.Request)
        {
            return GenerateRequest((callStatement.Call.Arguments.FirstOrDefault() as ActionAccess)?.Action ?? throw new InvalidOperationException("The request function must have an action as the first argument."));
        }
        if (callStatement.Call.Function == PreDefinedFunction.SetOpcodeStepTable)
        {
            return GenerateSetOpcodeStepTable(context, callStatement.Call);
        }

        throw new NotSupportedException($"The function {callStatement.Call.Function} is not supported.");
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateFinishInstruction(StepContext context)
    {
        foreach (var statement in context.GeneratorContext.OnInstructionComplete.SelectMany(s => GenerateStatements(context, s)))
        {
            yield return statement;
        }

        yield return CreateSetStep(0).WithLeadingTrivia(Comment("// Finish instruction."));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateAssignment(StepContext context, Assignment assignment)
    {
        // Skip self-assignments.
        if (assignment.Target == assignment.Value)
        {
            yield break;
        }

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

        yield return ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, target, value));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateIf(StepContext context, IfStatement ifStatement)
    {
        var condition = ExpressionGenerator.GenerateExpressionSyntax(context.WithBooleanContext(), ifStatement.Condition);

        var ifStatements = ifStatement.IfStatements.SelectMany(statement => GenerateStatements(context, statement));
        var elseStatements = ifStatement.ElseStatements.SelectMany(statement => GenerateStatements(context, statement));

        // We might have true or false for the condition, which we can optimise.
        if (condition is LiteralExpressionSyntax literal)
        {
            var constant = (bool)literal.Token.Value!;
            return constant ? ifStatements : elseStatements;
        }

        return ifStatement.ElseStatements.Any()
            ? [IfStatement(condition, Block(ifStatements), ElseClause(Block(elseStatements)))]
            : [IfStatement(condition, Block(ifStatements))];
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateRequest(Action action)
    {
        yield return
            ReturnStatement(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(ActionRequiredEnumName),
                    IdentifierName(action.EnumName)));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToOpcodeRead()
    {
        yield return CreateSetStep(0);
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToOpcode(StepContext context)
    {
        // TODO: Version without bounds checks, don't rely on the JIT.
        var getOpcode = CreateArrayGetWithoutBoundsCheck(
            context.GeneratorContext,
            EmulatorMemberIdentifier(PreDefinedDataMember.OpcodeStepTable.FieldName),
            EmulatorMemberIdentifier(PreDefinedDataMember.Data.FieldName));

        yield return CreateSetStep(getOpcode);
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateOverlappedOpcodeRead(StepContext context)
    {
        // Execute step 0. No need to set step 1; the NextOpcode handling above will cover that.
        yield return ExpressionStatement(
            InvocationExpression(IdentifierName(GetStepFunctionName(context.GeneratorContext.OpcodeReadFirstStep)))
                .WithArgumentList(ArgumentList([CreateEmulatorArgument()])))
            .WithLeadingTrivia(NewlineComment, Comment("// Overlapped opcode read."));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateSetOpcodeStepTable(StepContext context, Call callStatementCall)
    {
        if (callStatementCall.Arguments.Count == 0)
        {
            return [GenerateSetOpcodeStepTable(context.Configuration.OpcodeStepTables.NoPrefix)];
        }

        var argument = callStatementCall.Arguments[0];
        if (argument is Number number)
        {
            return [GenerateSetOpcodeStepTable(context.Configuration.OpcodeStepTables.GetForPrefix((byte)number.Value))];
        }

        if (argument is OpcodeStepTableAccess opcodeStepTableAccess)
        {
            return [GenerateSetOpcodeStepTable(opcodeStepTableAccess.OpcodeStepTable)];
        }

        throw new NotSupportedException($"The argument {argument} is not supported for {PreDefinedFunction.SetOpcodeStepTable.Name}.");
    }

    [Pure]
    private static StatementSyntax GenerateSetOpcodeStepTable(OpcodeStepTable opcodeStepTable) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                EmulatorMemberIdentifier(PreDefinedDataMember.OpcodeStepTable.FieldName),
                IdentifierName(opcodeStepTable.FieldName)));

    [Pure]
    private static StatementSyntax CreateSetStep(int step) => CreateSetStep(GenerateNumericLiteralExpression(step));

    [Pure]
    private static StatementSyntax CreateSetStep(ExpressionSyntax value) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                EmulatorMemberIdentifier(PreDefinedDataMember.CurrentStep.FieldName),
                value));
}