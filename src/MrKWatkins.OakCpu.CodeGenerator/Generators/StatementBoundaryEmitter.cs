using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.GeneratorSymbols;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

internal static class StatementBoundaryEmitter
{
    [Pure]
    public static IEnumerable<StatementSyntax> GenerateHandled(StatementGeneratorContext context)
    {
        if (context.Step != null)
        {
            throw new InvalidOperationException("Cannot use handled() inside an instruction.");
        }

        if (!context.InstructionEmulatorMode)
        {
            yield return ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName(ActionRequiredParameterName),
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(ActionRequiredEnumName), IdentifierName(Action.None.EnumName))));
        }

        yield return ReturnStatement(LiteralExpression(SyntaxKind.TrueLiteralExpression));
    }

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateHandleInterrupts(StatementGeneratorContext context)
    {
        if (context.InstructionCompletionMode)
        {
            yield return ExpressionStatement(
                InvocationExpression(IdentifierName(HandleInterruptsMethodName))
                    .WithArgumentList(ArgumentList([CreateEmulatorArgument()])));
            yield break;
        }

        yield return GenerateHandleInterruptsAndReturnIfHandled(context);
    }

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateInstructionComplete(StatementGeneratorContext context)
    {
        if (context.InstructionStepMode)
        {
            foreach (var statement in GenerateCompleteInstructionAndReturn(context))
            {
                yield return statement;
            }
            yield break;
        }

        if (context.InstructionEmulatorMode)
        {
            throw new InvalidOperationException("instruction_complete is only supported when generating instruction-emulator steps.");
        }

        foreach (var statement in context.GeneratorContext.OnInstructionComplete.SelectMany(s => StatementStatementEmitter.Generate(context, s)))
        {
            yield return statement;
        }

        yield return StatementTransitionEmitter.CreateSetStep(context.GeneratorContext.OpcodeRead.FirstStep).WithLeadingTrivia(Comment("// Finish instruction."));
    }

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateBoundaryStatements(StatementGeneratorContext context)
    {
        if (context.Step is not { QueuesOverlapStep: true } step)
        {
            return [];
        }

        if (context.InstructionEmulatorMode)
        {
            return
            [
                GenerateHandleInterruptsAndReturnIfHandled(context).WithLeadingTrivia(Comment("// Check interrupts at the instruction boundary."))
            ];
        }

        return
        [
            StatementTransitionEmitter.GenerateQueueOverlap(context.GeneratorContext, step.QueuedOverlapStep).WithLeadingTrivia(Comment("// Queue overlap step.")),
            GenerateHandleInterruptsAndReturnIfHandled(context).WithLeadingTrivia(Comment("// Check interrupts at the instruction boundary."))
        ];
    }

    [Pure]
    public static IfStatementSyntax GenerateHandleInterruptsAndReturnIfHandled(StatementGeneratorContext context)
    {
        if (context.InstructionStepMode)
        {
            var instructionStep = context.RequiredInstructionStep;
            if (context.Step?.Sequence is not Instruction)
            {
                return IfStatement(
                    InvocationExpression(IdentifierName(HandleInterruptsMethodName))
                        .WithArgumentList(ArgumentList([CreateEmulatorArgument()])),
                    Block(SingletonList<StatementSyntax>(
                        ReturnStatement(GenerateNumericLiteralExpression(instructionStep.TStatesBeforeStep + 1)))));
            }

            return IfStatement(
                InvocationExpression(IdentifierName(HandleInterruptsMethodName))
                    .WithArgumentList(ArgumentList([CreateEmulatorArgument()])),
                Block(GenerateCompleteInstructionAndReturn(context)));
        }

        if (context.InstructionEmulatorMode)
        {
            throw new InvalidOperationException("handle_interrupts is only supported when generating instruction-emulator steps.");
        }

        return IfStatement(
            InvocationExpression(IdentifierName(HandleInterruptsMethodName))
                .WithArgumentList(ArgumentList(
                [
                    CreateEmulatorArgument(),
                    Argument(RefExpression(IdentifierName(ActionRequiredParameterName)))
                ])),
            Block(ReturnStatement()));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateCompleteInstructionAndReturn(StatementGeneratorContext context)
    {
        if (!context.InstructionStepMode)
        {
            throw new InvalidOperationException("Instruction completion statements can only be generated for instruction-emulator steps.");
        }

        var instructionStep = context.RequiredInstructionStep;

        if (instructionStep.ExitOverlapStep != null)
        {
            foreach (var statement in StatementGenerator.GenerateOverlapStatements(context.GeneratorContext, instructionStep.ExitOverlapStep, context.GeneratorContext.GetImplicitInstructionCompleteStatementCount(instructionStep.ExitOverlapStep)))
            {
                yield return statement;
            }
        }

        yield return ReturnStatement(
            InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(EmulatorParameterName),
                        IdentifierName(CompleteInstructionMethodName)))
                .WithArgumentList(
                    ArgumentList(
                    [
                        Argument(GenerateInstructionUpdatesFlagsLiteralExpression(context)),
                        Argument(GenerateNumericLiteralExpression(instructionStep.TStatesBeforeStep + 1))
                    ])));
    }

    [Pure]
    private static LiteralExpressionSyntax GenerateInstructionUpdatesFlagsLiteralExpression(StatementGeneratorContext context) =>
        context.Step?.Sequence is Instruction instruction
            ? LiteralExpression(instruction.UpdatesFlags ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression)
            : throw new InvalidOperationException("Instruction completion can only be generated inside an instruction.");
}