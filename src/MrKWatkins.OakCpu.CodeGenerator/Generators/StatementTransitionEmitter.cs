using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.GeneratorSymbols;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

internal static class StatementTransitionEmitter
{
    [Pure]
    public static IEnumerable<StatementSyntax> GenerateMoveToSequence(StatementGeneratorContext context, Call call)
    {
        if (call.Arguments.Count != 1)
        {
            throw new InvalidOperationException($"Calls to {PreDefinedFunction.MoveToSequence.Name} must have exactly one argument.");
        }

        return call.Arguments[0] is SequenceAccess sequenceAccess
            ? GenerateMoveToSequence(context, context.GeneratorContext.GetSequence(sequenceAccess.SequenceName))
            : throw new InvalidOperationException($"Calls to {PreDefinedFunction.MoveToSequence.Name} must use a sequence.<name> argument.");
    }

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateMoveToInterruptMode(StatementGeneratorContext context, Call call) =>
        call.Arguments.Count == 1
            ? GenerateMoveToSequenceGroup(context, context.GeneratorContext.GetSequenceGroup(InterruptMode.SequenceGroupName), call.Arguments[0])
            : throw new InvalidOperationException($"Calls to {PreDefinedFunction.MoveToInterruptMode} must have exactly one argument.");

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateMoveToSequenceGroup(StatementGeneratorContext context, Call call)
    {
        if (call.Arguments.Count != 2)
        {
            throw new InvalidOperationException($"Calls to {PreDefinedFunction.MoveToSequenceGroup.Name} must have exactly two arguments.");
        }

        return call.Arguments[0] is SequenceGroupAccess sequenceGroupAccess
            ? GenerateMoveToSequenceGroup(context, context.GeneratorContext.GetSequenceGroup(sequenceGroupAccess.SequenceGroupName), call.Arguments[1])
            : throw new InvalidOperationException($"Calls to {PreDefinedFunction.MoveToSequenceGroup.Name} must use a sequence_group.<name> argument.");
    }

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateMoveToOpcode(StatementGeneratorContext context)
    {
        var getOpcode = CreateArrayGetWithoutBoundsCheck(
            context.GeneratorContext.RequiredUsings,
            EmulatorMemberIdentifier(PreDefinedDataMember.OpcodeStepTable.FieldName),
            EmulatorMemberIdentifier(PreDefinedDataMember.Data.FieldName));

        if (context.InstructionStepMode)
        {
            yield return CreateSetNextInstruction(context, getOpcode);
            yield break;
        }

        const string selectedStepVariableName = "selectedStep";

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
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(selectedStepVariableName), IdentifierName(StepNextStepFieldName)))),
            StatementBoundaryEmitter.GenerateHandleInterruptsAndReturnIfHandled(context).WithLeadingTrivia(Comment("// Check interrupts at the instruction boundary."))
        };

        yield return IfStatement(
            BinaryExpression(
                SyntaxKind.NotEqualsExpression,
                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(selectedStepVariableName), IdentifierName(StepOverlapFieldName)),
                LiteralExpression(SyntaxKind.DefaultLiteralExpression)),
            Block(overlapStatements));
    }

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateMoveToSequenceStart(StepSequence sequence)
    {
        yield return CreateSetStep(sequence.FirstStep);
    }

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateExecuteSequenceOnStart(StepSequence sequence, string comment)
    {
        yield return GenerateCallStep(sequence.FirstStep)
            .WithLeadingTrivia(Comment($"// {comment}"));
    }

    [Pure]
    public static ExpressionStatementSyntax GenerateQueueOverlap(GeneratorContext context, Step step) =>
        GenerateQueueOverlap(PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(GetOverlapMethodName(context, step))));

    [Pure]
    public static ExpressionStatementSyntax GenerateQueueOverlap(ExpressionSyntax overlap) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                EmulatorMemberIdentifier(PreDefinedDataMember.OverlapPipeline.FieldName),
                overlap));

    [Pure]
    public static ExpressionStatementSyntax GenerateExecuteOverlap() =>
        ExpressionStatement(
            InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(EmulatorParameterName), IdentifierName(ExecuteOverlapMethodName)))
                .WithArgumentList(ArgumentList()));

    [Pure]
    public static ExpressionStatementSyntax GenerateSetOpcodeStepTable(OpcodeStepTable opcodeStepTable) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                EmulatorMemberIdentifier(PreDefinedDataMember.OpcodeStepTable.FieldName),
                IdentifierName(opcodeStepTable.FieldName)));

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateSetOpcodeStepTable(StatementGeneratorContext context, Call callStatementCall)
    {
        if (callStatementCall.Arguments.Count == 0)
        {
            return [GenerateSetOpcodeStepTable(context.Configuration.OpcodeStepTables.NoPrefix)];
        }

        var argument = callStatementCall.Arguments[0];
        return argument switch
        {
            Number number => [GenerateSetOpcodeStepTable(context.Configuration.OpcodeStepTables.GetForPrefix((byte)number.Value))],
            OpcodeStepTableAccess opcodeStepTableAccess => [GenerateSetOpcodeStepTable(opcodeStepTableAccess.OpcodeStepTable)],
            _ => throw new NotSupportedException($"The argument {argument} is not supported for {PreDefinedFunction.SetOpcodeStepTable.Name}.")
        };
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToSequenceGroup(StatementGeneratorContext context, SequenceGroup sequenceGroup, Expression selector)
    {
        var getSequence = CreateArrayGetWithoutBoundsCheck(
            context.GeneratorContext.RequiredUsings,
            IdentifierName(GetSequenceGroupStepTableFieldName(sequenceGroup)),
            ExpressionGenerator.GenerateExpressionSyntax(context, selector));

        if (context.InstructionStepMode)
        {
            yield return CreateSetNextInstruction(context, getSequence);
            yield break;
        }

        if (context.InstructionEmulatorMode || context.InstructionCompletionMode)
        {
            yield return CreateSetNextSequence(getSequence);
            yield break;
        }

        yield return CreateSetStep(getSequence).WithLeadingTrivia(Comment($"// Move to {sequenceGroup.Name.Replace('_', ' ')}."));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToSequence(StatementGeneratorContext context, StepSequence sequence)
    {
        if (context.InstructionStepMode)
        {
            return [CreateSetNextInstruction(context, GenerateNumericLiteralExpression(context.GeneratorContext.GetInstructionEmulatorSequenceIndex(sequence)))];
        }

        if (context.InstructionEmulatorMode || context.InstructionCompletionMode)
        {
            return [CreateSetNextSequence(GenerateNumericLiteralExpression(context.GeneratorContext.GetInstructionEmulatorSequenceIndex(sequence)))];
        }

        return GenerateMoveToSequenceStart(sequence);
    }

    [Pure]
    private static ExpressionStatementSyntax GenerateCallStep(Step step) =>
        ExpressionStatement(
            InvocationExpression(IdentifierName(GetStepMethodName(step)))
                .WithArgumentList(
                    ArgumentList(
                    [
                        CreateEmulatorArgument(),
                        Argument(RefExpression(IdentifierName(ActionRequiredParameterName)))
                    ])));

    [Pure]
    private static ExpressionStatementSyntax CreateSetNextInstruction(StatementGeneratorContext context, ExpressionSyntax index)
    {
        if (!context.InstructionStepMode)
        {
            throw new InvalidOperationException("Instruction-emulator step redirects require a next-instruction variable.");
        }

        var instructionStep = context.RequiredInstructionStep;
        if (instructionStep.NextInstructionVariableName == null)
        {
            throw new InvalidOperationException("Instruction-emulator step redirects require a next-instruction variable.");
        }

        return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(instructionStep.NextInstructionVariableName),
                index));
    }

    [Pure]
    private static ExpressionStatementSyntax CreateSetNextSequence(ExpressionSyntax index) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(EmulatorParameterName),
                    IdentifierName(NextSequenceStepFieldName)),
                index));

    [Pure]
    internal static ExpressionStatementSyntax CreateSetStep(Step step) => CreateSetStep(GenerateNumericLiteralExpression(step.Index));

    [Pure]
    internal static ExpressionStatementSyntax CreateSetStep(ExpressionSyntax value) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                EmulatorMemberIdentifier(PreDefinedDataMember.CurrentStep.FieldName),
                value));
}