using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

// TODO: Optimise A ^ A to 0.
public abstract class StatementGenerator : Generator
{
    [Pure]
    public static IEnumerable<StatementSyntax> GenerateStatements(GeneratorContext input, Step step)
    {
        var context = new StatementGeneratorContext(input, step);

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
            NextOpcodeMode.Read => GenerateMoveToOpcodeRead(context),
            NextOpcodeMode.Overlapped => GenerateOverlappedOpcodeRead(context),
            NextOpcodeMode.Custom => [],
            NextOpcodeMode.Loop => [],
            null => [],
            _ => throw new NotSupportedException($"The next opcode mode {step.NextOpcode} is not supported.")
        };

        foreach (var statement in trailingStatements)
        {
            yield return statement;
        }
    }

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateStatements(GeneratorContext context, IEnumerable<Statement> statements)
    {
        var statementContext = new StatementGeneratorContext(context, null);

        return statements.SelectMany(statement => GenerateStatements(statementContext, statement));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateStatements(StatementGeneratorContext context, Statement statement) =>
        statement switch
        {
            Assignment assignment => GenerateAssignment(context, assignment),
            IfStatement ifStatement => GenerateIf(context, ifStatement),
            CallStatement callStatement => GenerateCall(context, callStatement),
            TemporaryVariableDeclarationStatement temporaryVariableDeclaration => GenerateTemporaryVariableDeclaration(context, temporaryVariableDeclaration.Variable),
            _ => throw new NotSupportedException($"The statement type {statement.GetType().Name} is not supported.")
        };

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateCall(StatementGeneratorContext context, CallStatement callStatement)
    {
        if (callStatement.Call.Function == PreDefinedFunction.Flags)
        {
            return FlagsGenerator.GenerateFlagsStatements(context);
        }
        if (callStatement.Call.Function == PreDefinedFunction.InstructionComplete)
        {
            return GenerateInstructionComplete(context);
        }
        if (callStatement.Call.Function == PreDefinedFunction.Handled)
        {
            return GenerateHandled(context);
        }
        if (callStatement.Call.Function == PreDefinedFunction.MoveToHaltedCycle)
        {
            return GenerateMoveToHaltedCycle(context);
        }
        if (callStatement.Call.Function == PreDefinedFunction.HandleInterrupts)
        {
            return GenerateHandleInterrupts();
        }
        if (callStatement.Call.Function == PreDefinedFunction.MoveToInterruptMode)
        {
            return GenerateMoveToInterruptMode(context, callStatement.Call);
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
    private static IEnumerable<StatementSyntax> GenerateTemporaryVariableDeclaration(StatementGeneratorContext context, TemporaryVariable temporaryVariable)
    {
        if (!context.InitializedTemporaryVariables.Add(temporaryVariable.Name))
        {
            throw new InvalidOperationException($"The temporary variable {temporaryVariable.Name} has already been initialized.");
        }

        yield return LocalDeclarationStatement(VariableDeclaration(temporaryVariable.Type.TypeSyntax())
            .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(temporaryVariable.Name)))));
    }

    [Pure]
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private static IEnumerable<StatementSyntax> GenerateHandled(StatementGeneratorContext context)
    {
        if (context.Step != null)
        {
            throw new InvalidOperationException("Cannot use handled() inside an instruction.");
        }

        yield return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression, IdentifierName(ActionRequiredParameterName),
                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(ActionRequiredEnumName), IdentifierName(Action.None.EnumName))));

        yield return ReturnStatement(LiteralExpression(SyntaxKind.TrueLiteralExpression));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateHandleInterrupts()
    {
        yield return IfStatement(
                InvocationExpression(IdentifierName(HandleInterruptsMethodName))
                    .WithArgumentList(ArgumentList(SeparatedList([
                        Argument(IdentifierName(EmulatorParameterName)),
                        Argument(IdentifierName(ActionRequiredParameterName)).WithRefKindKeyword(Ref)
                    ]))),
                Block(ReturnStatement()));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateInstructionComplete(StatementGeneratorContext context)
    {
        foreach (var statement in context.GeneratorContext.OnInstructionComplete.SelectMany(s => GenerateStatements(context, s)))
        {
            yield return statement;
        }

        yield return CreateSetStep(context.GeneratorContext.OpcodeRead.FirstStep).WithLeadingTrivia(Comment("// Finish instruction."));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateAssignment(StatementGeneratorContext context, Assignment assignment)
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
    private static IEnumerable<StatementSyntax> GenerateIf(StatementGeneratorContext context, IfStatement ifStatement)
    {
        var condition = ExpressionGenerator.GenerateExpressionSyntax(context.WithBooleanContext(), ifStatement.Condition);

        var ifContext = context.WithChildVariableScope();
        var ifStatements = ifStatement.IfStatements.SelectMany(statement => GenerateStatements(ifContext, statement));

        var elseContext = context.WithChildVariableScope();
        var elseStatements = ifStatement.ElseStatements.SelectMany(statement => GenerateStatements(elseContext, statement));

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
    private static IEnumerable<StatementSyntax> GenerateMoveToOpcodeRead(StatementGeneratorContext context)
    {
        yield return CreateSetStep(context.GeneratorContext.OpcodeRead.FirstStep);
    }

    // TODO: Could we make more generic, i.e. if overlapped do this? Only bother if needed for other chips.
    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToHaltedCycle(StatementGeneratorContext context)
    {
        // Need to overlap the first step of the halted cycle.
        yield return CreateSetStep(context.GeneratorContext.Interrupts.HaltedCycle.Steps[1]).WithLeadingTrivia(Comment("// Move to halted cycle."));
        yield return GenerateCallStep(context.GeneratorContext.Interrupts.HaltedCycle.FirstStep);
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToInterruptMode(StatementGeneratorContext context, Call call)
    {
        if (call.Arguments.Count != 1)
        {
            throw new InvalidOperationException($"Calls to {PreDefinedFunction.MoveToInterruptMode} must have exactly one argument.");
        }

        var getOpcode = CreateArrayGetWithoutBoundsCheck(
            context.GeneratorContext.RequiredUsings,
            IdentifierName(InterruptModeStepTableFieldName),
            ExpressionGenerator.GenerateExpressionSyntax(context, call.Arguments[0]));

        yield return CreateSetStep(getOpcode);
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToOpcode(StatementGeneratorContext context)
    {
        var getOpcode = CreateArrayGetWithoutBoundsCheck(
            context.GeneratorContext.RequiredUsings,
            EmulatorMemberIdentifier(PreDefinedDataMember.OpcodeStepTable.FieldName),
            EmulatorMemberIdentifier(PreDefinedDataMember.Data.FieldName));

        yield return CreateSetStep(getOpcode);
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateOverlappedOpcodeRead(StatementGeneratorContext context)
    {
        // Execute step 0. No need to set step 1; the NextOpcode handling will cover that.
        yield return GenerateCallStep(context.GeneratorContext.OpcodeRead.FirstStep)
            .WithLeadingTrivia(Comment("// Overlapped opcode read."));
    }

    [Pure]
    private static StatementSyntax GenerateCallStep(Step step) =>
        ExpressionStatement(
            InvocationExpression(IdentifierName(GetStepFunctionName(step)))
                .WithArgumentList(ArgumentList(
                [
                    CreateEmulatorArgument(),
                    Argument(RefExpression(IdentifierName(ActionRequiredParameterName)))
                ])));

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateSetOpcodeStepTable(StatementGeneratorContext context, Call callStatementCall)
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
    private static StatementSyntax CreateSetStep(Step step) => CreateSetStep(GenerateNumericLiteralExpression(step.Index));

    [Pure]
    private static StatementSyntax CreateSetStep(ExpressionSyntax value) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                EmulatorMemberIdentifier(PreDefinedDataMember.CurrentStep.FieldName),
                value));
}