using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Parameter = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Parameter;
using Field = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Field;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Statements;

internal static class StatementTransitionEmitter
{
    private const string SelectedStepVariableName = "selectedStep";

    [Pure]
    internal static IEnumerable<StatementSyntax> GenerateMoveToSequence(StatementGeneratorContext context, Call call)
    {
        if (call.Arguments.Count != 1)
        {
            throw new InvalidOperationException($"Calls to {PreDefinedFunction.MoveToSequence.Name} must have exactly one argument.");
        }

        return call.Arguments[0] is SequenceAccess sequenceAccess
            ? GenerateMoveToSequence(context, context.GeneratorContext.GetSequence(sequenceAccess.SequenceName))
            : throw new InvalidOperationException($"Calls to {PreDefinedFunction.MoveToSequence.Name} must use a sequence.<name> argument.");
    }

    [MustUseReturnValue]
    internal static IEnumerable<StatementSyntax> GenerateMoveToInterruptMode(StatementGeneratorContext context, Call call) =>
        call.Arguments.Count == 1
            ? GenerateMoveToSequenceGroup(context, context.GeneratorContext.GetSequenceGroup(InterruptMode.SequenceGroupName), call.Arguments[0])
            : throw new InvalidOperationException($"Calls to {PreDefinedFunction.MoveToInterruptMode} must have exactly one argument.");

    [MustUseReturnValue]
    internal static IEnumerable<StatementSyntax> GenerateMoveToSequenceGroup(StatementGeneratorContext context, Call call)
    {
        if (call.Arguments.Count != 2)
        {
            throw new InvalidOperationException($"Calls to {PreDefinedFunction.MoveToSequenceGroup.Name} must have exactly two arguments.");
        }

        return call.Arguments[0] is SequenceGroupAccess sequenceGroupAccess
            ? GenerateMoveToSequenceGroup(context, context.GeneratorContext.GetSequenceGroup(sequenceGroupAccess.SequenceGroupName), call.Arguments[1])
            : throw new InvalidOperationException($"Calls to {PreDefinedFunction.MoveToSequenceGroup.Name} must use a sequence_group.<name> argument.");
    }

    [MustUseReturnValue]
    internal static IEnumerable<StatementSyntax> GenerateMoveToOpcode(StatementGeneratorContext context)
    {
        var opcodeStep = CreateArrayGetWithoutBoundsCheck(
            context.RequiredUsings,
            EmulatorMemberIdentifier(PreDefinedDataMember.OpcodeStepTable.FieldName),
            EmulatorMemberIdentifier(PreDefinedDataMember.Data.FieldName));

        if (context.Mode.SequenceTransitionTarget == SequenceTransitionTarget.NextInstruction)
        {
            yield return CreateSetNextInstruction(context, opcodeStep);
            yield break;
        }

        foreach (var statement in GenerateMoveToOpcodeTransition(context, opcodeStep))
        {
            yield return statement;
        }
    }

    [Pure]
    internal static IEnumerable<StatementSyntax> GenerateMoveToSequenceStart(GeneratorContext context, StepSequence sequence)
    {
        yield return CreateSetStep(context, sequence.FirstStep);
    }

    [Pure]
    internal static IEnumerable<StatementSyntax> GenerateExecuteSequenceOnStart(GeneratorContext context, StepSequence sequence, string comment)
    {
        yield return GenerateCallStep(context, sequence.FirstStep)
            .WithLeadingTrivia(Comment($"// {comment}"));
    }

    [Pure]
    internal static ExpressionStatementSyntax GenerateQueueOverlap(GeneratorContext context, Step step) =>
        GenerateQueueOverlap(PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(Method.Name.Overlap(context, step))));

    [Pure]
    private static ExpressionStatementSyntax GenerateQueueOverlap(ExpressionSyntax overlap) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                EmulatorMemberIdentifier(PreDefinedDataMember.OverlapPipeline.FieldName),
                overlap));

    [Pure]
    internal static ExpressionStatementSyntax GenerateExecuteOverlap() =>
        ExpressionStatement(
            InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(Parameter.Name.Emulator), IdentifierName(Method.Name.ExecuteOverlap)))
                .WithArgumentList(ArgumentList()));

    [Pure]
    internal static ExpressionStatementSyntax GenerateSetOpcodeStepTable(OpcodeStepTable opcodeStepTable) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                EmulatorMemberIdentifier(PreDefinedDataMember.OpcodeStepTable.FieldName),
                IdentifierName(opcodeStepTable.FieldName)));

    [Pure]
    internal static IEnumerable<StatementSyntax> GenerateSetOpcodeStepTable(StatementGeneratorContext context, Call callStatementCall)
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

    [MustUseReturnValue]
    private static IEnumerable<StatementSyntax> GenerateMoveToSequenceGroup(StatementGeneratorContext context, SequenceGroup sequenceGroup, Expression selector)
    {
        var getSequence = CreateArrayGetWithoutBoundsCheck(
            context.RequiredUsings,
            IdentifierName(Field.Name.SequenceGroupStepTable(sequenceGroup)),
            ExpressionGenerator.GenerateExpressionSyntax(context, selector));

        foreach (var statement in GenerateSequenceTransition(context, getSequence, $"Move to {sequenceGroup.Name.Replace('_', ' ')}."))
        {
            yield return statement;
        }
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateMoveToSequence(StatementGeneratorContext context, StepSequence sequence)
    {
        var index = GenerateNumericLiteralExpression(context.GeneratorContext.GetInstructionEmulatorSequenceIndex(sequence));

        return context.Mode.SequenceTransitionTarget switch
        {
            SequenceTransitionTarget.NextInstruction => [CreateSetNextInstruction(context, index)],
            SequenceTransitionTarget.NextSequence => [CreateSetNextSequence(index)],
            _ => GenerateMoveToSequenceStart(context.GeneratorContext, sequence)
        };
    }

    [MustUseReturnValue]
    private static IEnumerable<StatementSyntax> GenerateMoveToOpcodeTransition(StatementGeneratorContext context, ExpressionSyntax opcodeStep)
    {
        yield return CreateSetStep(opcodeStep);
        yield return CreateSelectedStepDeclaration(context);
        yield return CreateQueuedOverlapTransition(context);
    }

    [MustUseReturnValue]
    private static IEnumerable<StatementSyntax> GenerateSequenceTransition(StatementGeneratorContext context, ExpressionSyntax index, string? comment = null)
    {
        switch (context.Mode.SequenceTransitionTarget)
        {
            case SequenceTransitionTarget.NextInstruction:
                yield return CreateSetNextInstruction(context, index);
                yield break;

            case SequenceTransitionTarget.NextSequence:
                yield return CreateSetNextSequence(index);
                yield break;

            default:
                var setStep = CreateSetStep(index);
                yield return comment == null ? setStep : setStep.WithLeadingTrivia(Comment($"// {comment}"));
                yield break;
        }
    }

    [MustUseReturnValue]
    private static LocalDeclarationStatementSyntax CreateSelectedStepDeclaration(StatementGeneratorContext context) =>
        LocalDeclarationStatement(
            VariableDeclaration(IdentifierName("var"))
                .WithVariables([
                    VariableDeclarator(SelectedStepVariableName)
                        .WithInitializer(
                            EqualsValueClause(
                                CreateArrayGetWithoutBoundsCheck(
                                    context.RequiredUsings,
                                    IdentifierName("Steps"),
                                    EmulatorMemberIdentifier(PreDefinedDataMember.CurrentStep.FieldName))))
                ]));

    [Pure]
    private static IfStatementSyntax CreateQueuedOverlapTransition(StatementGeneratorContext context)
    {
        var condition = BinaryExpression(
            SyntaxKind.NotEqualsExpression,
            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(SelectedStepVariableName), IdentifierName(Field.Name.Overlap)),
            LiteralExpression(SyntaxKind.DefaultLiteralExpression));

        var queueOverlap = GenerateQueueOverlap(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(SelectedStepVariableName), IdentifierName(Field.Name.Overlap)))
            .WithLeadingTrivia(Comment("// Queue overlap step."));
        var setNextStep = ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                EmulatorMemberIdentifier(PreDefinedDataMember.CurrentStep.FieldName),
                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(SelectedStepVariableName), IdentifierName(Field.Name.NextStep))));
        var handleInterrupts = StatementBoundaryEmitter.GenerateHandleInterruptsAndReturnIfHandled(context)
            .WithLeadingTrivia(Comment("// Check interrupts at the instruction boundary."));

        return IfStatement(condition, Block(queueOverlap, setNextStep, handleInterrupts));
    }

    [Pure]
    private static ExpressionStatementSyntax GenerateCallStep(GeneratorContext context, Step step) =>
        ExpressionStatement(
            InvocationExpression(IdentifierName(Method.Name.Step(context, step)))
                .WithArgumentList(
                    ArgumentList(
                    [
                        CreateEmulatorArgument(),
                        Argument(RefExpression(IdentifierName(Parameter.Name.ActionRequired)))
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
                    IdentifierName(Parameter.Name.Emulator),
                    IdentifierName(Field.Name.NextSequenceStep)),
                index));

    [Pure]
    internal static ExpressionStatementSyntax CreateSetStep(GeneratorContext context, Step step) => CreateSetStep(GenerateNumericLiteralExpression(context.GetStepLayout(step).Index));

    [Pure]
    internal static ExpressionStatementSyntax CreateSetStep(ExpressionSyntax value) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                EmulatorMemberIdentifier(PreDefinedDataMember.CurrentStep.FieldName),
                value));
}