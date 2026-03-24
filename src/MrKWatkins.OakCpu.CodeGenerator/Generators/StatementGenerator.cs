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

        foreach (var statement in GenerateStepStatements(context, step))
        {
            yield return statement;
        }

        foreach (var statement in GenerateBoundaryStatements(context))
        {
            yield return statement;
        }

        var trailingStatements = step.NextOpcode switch
        {
            NextOpcodeMode.Read => GenerateMoveToSequenceStart(context.GeneratorContext.OpcodeRead),
            NextOpcodeMode.Overlapped when step.Sequence is PrefixJump => GenerateExecuteSequenceOnStart(context.GeneratorContext.OpcodeRead, "Overlapped opcode read."),
            NextOpcodeMode.Overlapped => [],
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
    public static IEnumerable<StatementSyntax> GenerateOverlapStatements(GeneratorContext input, Step step)
    {
        var context = new StatementGeneratorContext(input, step).WithoutHandleInterrupts();

        if (step.DoesNothing)
        {
            throw new InvalidOperationException("Trying to generate overlap statements for a step that does nothing.");
        }

        foreach (var statement in GenerateStepStatements(context, step))
        {
            yield return statement;
        }
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateStepStatements(StatementGeneratorContext context, Step step)
    {
        if (step.RequiresPrefixReset)
        {
            yield return GenerateSetOpcodeStepTable(context.Configuration.OpcodeStepTables.NoPrefix);
        }

        if (step.ExecutesStoredOverlapOnStart)
        {
            yield return GenerateExecuteOverlap().WithLeadingTrivia(Comment("// Execute queued overlap."));
        }

        foreach (var stepStatement in step.Statements)
        {
            foreach (var statement in GenerateStatements(context, stepStatement))
            {
                yield return statement;
            }
        }
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
        if (context.SkipHandleInterrupts && callStatement.Call.Function == PreDefinedFunction.HandleInterrupts)
        {
            return [];
        }

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
        if (callStatement.Call.Function == PreDefinedFunction.MoveToSequenceGroup)
        {
            return GenerateMoveToSequenceGroup(context, callStatement.Call);
        }
        if (callStatement.Call.Function == PreDefinedFunction.MoveToSequence)
        {
            return GenerateMoveToSequence(context, callStatement.Call);
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

    // TODO: If no overlapped read, skip the if altogether.
    [Pure]
    private static IEnumerable<StatementSyntax> GenerateHandleInterrupts()
    {
        yield return GenerateHandleInterruptsAndReturnIfHandled();
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

        if (TryGenerateCompoundAssignment(context, assignment, out var statement))
        {
            yield return statement;
            yield break;
        }

        // Might need to cast to the correct type. We can ignore the cast if the value is a numeric literal; the compiler will type the number correctly for us.
        var value = ExpressionGenerator.GenerateExpressionSyntax(context, assignment.Value);
        if (assignment.Target.Type != assignment.Value.Type && assignment.Value is not Number)
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
    private static bool TryGenerateCompoundAssignment(StatementGeneratorContext context, Assignment assignment, [MaybeNullWhen(false)] out StatementSyntax statement)
    {
        //  We don't check for X = Y + X; just write compounds with the left as the target.
        if (assignment.Value is not BinaryOperation binary ||
            binary.Left != assignment.Target ||
            binary.Operator.CompoundAssignmentSyntaxKind == null)
        {
            statement = null;
            return false;
        }

        // If the type of the right-hand side does not match the target, then skip the assignment. Exception to this is if it's a numeric constant - that will be
        // typed by the compiler to the target type.
        if (assignment.Target.Type != binary.Right.Type && binary.Right is not Number)
        {
            statement = null;
            return false;
        }

        var value = ExpressionGenerator.GenerateExpressionSyntax(context, binary.Right);

        var target = assignment.Target is TemporaryVariableAccess temporaryVariableAccess ? temporaryVariableAccess.Identifier : ExpressionGenerator.GenerateExpressionSyntax(context, assignment.Target);

        statement = ExpressionStatement(AssignmentExpression(binary.Operator.CompoundAssignmentSyntaxKind.Value, target, value));
        return true;
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
    private static IEnumerable<StatementSyntax> GenerateMoveToSequence(StatementGeneratorContext context, Call call)
    {
        if (call.Arguments.Count != 1)
        {
            throw new InvalidOperationException($"Calls to {PreDefinedFunction.MoveToSequence.Name} must have exactly one argument.");
        }

        if (call.Arguments[0] is not SequenceAccess sequenceAccess)
        {
            throw new InvalidOperationException($"Calls to {PreDefinedFunction.MoveToSequence.Name} must use a sequence.<name> argument.");
        }

        return GenerateMoveToSequence(context.GeneratorContext.GetSequence(sequenceAccess.SequenceName));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToInterruptMode(StatementGeneratorContext context, Call call)
    {
        if (call.Arguments.Count != 1)
        {
            throw new InvalidOperationException($"Calls to {PreDefinedFunction.MoveToInterruptMode} must have exactly one argument.");
        }

        return GenerateMoveToSequenceGroup(context, context.GeneratorContext.GetSequenceGroup(InterruptMode.SequenceGroupName), call.Arguments[0]);
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToSequenceGroup(StatementGeneratorContext context, Call call)
    {
        if (call.Arguments.Count != 2)
        {
            throw new InvalidOperationException($"Calls to {PreDefinedFunction.MoveToSequenceGroup.Name} must have exactly two arguments.");
        }

        if (call.Arguments[0] is not SequenceGroupAccess sequenceGroupAccess)
        {
            throw new InvalidOperationException($"Calls to {PreDefinedFunction.MoveToSequenceGroup.Name} must use a sequence_group.<name> argument.");
        }

        return GenerateMoveToSequenceGroup(context, context.GeneratorContext.GetSequenceGroup(sequenceGroupAccess.SequenceGroupName), call.Arguments[1]);
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToSequenceGroup(StatementGeneratorContext context, SequenceGroup sequenceGroup, Expression selector)
    {
        var getSequence = CreateArrayGetWithoutBoundsCheck(
            context.GeneratorContext.RequiredUsings,
            IdentifierName(GetSequenceGroupStepTableFieldName(sequenceGroup)),
            ExpressionGenerator.GenerateExpressionSyntax(context, selector));

        yield return CreateSetStep(getSequence).WithLeadingTrivia(Comment($"// Move to {sequenceGroup.Name.Replace('_', ' ')}."));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToOpcode(StatementGeneratorContext context)
    {
        const string selectedStepVariableName = "selectedStep";

        var getOpcode = CreateArrayGetWithoutBoundsCheck(
            context.GeneratorContext.RequiredUsings,
            EmulatorMemberIdentifier(PreDefinedDataMember.OpcodeStepTable.FieldName),
            EmulatorMemberIdentifier(PreDefinedDataMember.Data.FieldName));

        yield return CreateSetStep(getOpcode);

        yield return LocalDeclarationStatement(
            VariableDeclaration(IdentifierName("var"))
                .WithVariables([
                    VariableDeclarator(selectedStepVariableName)
                        .WithInitializer(
                            EqualsValueClause(
                                CreateArrayGetWithoutBoundsCheck(
                                    context.GeneratorContext.RequiredUsings,
                                    IdentifierName("Steps"),
                                    EmulatorMemberIdentifier(PreDefinedDataMember.CurrentStep.FieldName))))
                ]));

        var overlapStatements = new List<StatementSyntax>
        {
            GenerateQueueOverlap(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(selectedStepVariableName), IdentifierName(StepOverlapFieldName)))
                .WithLeadingTrivia(Comment("// Queue overlap step.")),
            ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    EmulatorMemberIdentifier(PreDefinedDataMember.CurrentStep.FieldName),
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(selectedStepVariableName), IdentifierName(StepNextStepFieldName))))
        };
        overlapStatements.Add(GenerateHandleInterruptsAndReturnIfHandled().WithLeadingTrivia(Comment("// Check interrupts at the instruction boundary.")));

        yield return IfStatement(
            BinaryExpression(
                SyntaxKind.NotEqualsExpression,
                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(selectedStepVariableName), IdentifierName(StepOverlapFieldName)),
                LiteralExpression(SyntaxKind.DefaultLiteralExpression)),
            Block(overlapStatements));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToSequence(StepSequence sequence) => GenerateMoveToSequenceStart(sequence);

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToSequenceStart(StepSequence sequence)
    {
        yield return CreateSetStep(sequence.FirstStep);
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateExecuteSequenceOnStart(StepSequence sequence, string comment)
    {
        yield return GenerateCallStep(sequence.FirstStep)
            .WithLeadingTrivia(Comment($"// {comment}"));
    }

    [Pure]
    private static StatementSyntax GenerateQueueOverlap(GeneratorContext context, Step step) =>
        GenerateQueueOverlap(PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(GetOverlapMethodName(context, step))));

    [Pure]
    private static StatementSyntax GenerateQueueOverlap(ExpressionSyntax overlap) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                EmulatorMemberIdentifier(PreDefinedDataMember.OverlapPipeline.FieldName),
                overlap));

    [Pure]
    private static StatementSyntax GenerateExecuteOverlap() =>
        ExpressionStatement(
            InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(EmulatorParameterName), IdentifierName(ExecuteOverlapMethodName)))
                .WithArgumentList(ArgumentList()));

    [Pure]
    private static StatementSyntax GenerateCallStep(Step step) =>
        ExpressionStatement(
            InvocationExpression(IdentifierName(GetStepMethodName(step)))
                .WithArgumentList(
                    ArgumentList(
                    [
                        CreateEmulatorArgument(),
                        Argument(RefExpression(IdentifierName(ActionRequiredParameterName)))
                    ])));

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateBoundaryStatements(StatementGeneratorContext context)
    {
        if (context.Step is not { QueuesOverlapStep: true } step)
        {
            return [];
        }

        return
        [
            GenerateQueueOverlap(context.GeneratorContext, step.QueuedOverlapStep).WithLeadingTrivia(Comment("// Queue overlap step.")),
            GenerateHandleInterruptsAndReturnIfHandled().WithLeadingTrivia(Comment("// Check interrupts at the instruction boundary."))
        ];
    }

    [Pure]
    private static StatementSyntax GenerateHandleInterruptsAndReturnIfHandled() =>
        IfStatement(
            InvocationExpression(IdentifierName(HandleInterruptsMethodName))
                .WithArgumentList(ArgumentList(
                [
                    CreateEmulatorArgument(),
                    Argument(RefExpression(IdentifierName(ActionRequiredParameterName)))
                ])),
            Block(ReturnStatement()));

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
