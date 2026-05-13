using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Parameter = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Parameter;
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

        if (!context.Mode.IsInstructionEmulatorMode)
        {
            yield return ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName(Parameter.Name.ActionRequired),
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(TypeName.ActionRequiredEnum), IdentifierName(Action.None.EnumName))));
        }

        yield return ReturnStatement(LiteralExpression(SyntaxKind.TrueLiteralExpression));
    }

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateHandleInterrupts(StatementGeneratorContext context) =>
        context.Mode is StatementGenerationMode.InstructionCompletionMode
            ? [CreateHandleInterruptsStatement()]
            : [GenerateHandleInterruptsAndReturnIfHandled(context)];

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateInstructionComplete(StatementGeneratorContext context) =>
        context.Mode switch
        {
            StatementGenerationMode.InstructionStepMode => GenerateCompleteInstructionAndReturn(context),
            StatementGenerationMode.InstructionEmulatorMode => throw new InvalidOperationException("instruction_complete is only supported when generating instruction-emulator steps."),
            _ => GenerateStepInstructionComplete(context)
        };

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateBoundaryStatements(StatementGeneratorContext context)
    {
        if (context.Step is not { QueuesOverlapStep: true } step)
        {
            return [];
        }

        return context.Mode switch
        {
            StatementGenerationMode.InstructionEmulatorMode or StatementGenerationMode.InstructionStepMode =>
            [
                GenerateHandleInterruptsAndReturnIfHandled(context).WithLeadingTrivia(Comment("// Check interrupts at the instruction boundary."))
            ],
            _ =>
            [
                StatementTransitionEmitter.GenerateQueueOverlap(context.GeneratorContext, step.QueuedOverlapStep).WithLeadingTrivia(Comment("// Queue overlap step.")),
                GenerateHandleInterruptsAndReturnIfHandled(context).WithLeadingTrivia(Comment("// Check interrupts at the instruction boundary."))
            ]
        };
    }

    [Pure]
    public static IfStatementSyntax GenerateHandleInterruptsAndReturnIfHandled(StatementGeneratorContext context) =>
        context.Mode switch
        {
            StatementGenerationMode.InstructionStepMode => GenerateInstructionStepHandleInterruptsAndReturnIfHandled(context),
            StatementGenerationMode.InstructionEmulatorMode => throw new InvalidOperationException("handle_interrupts is only supported when generating instruction-emulator steps."),
            _ => GenerateStepHandleInterruptsAndReturnIfHandled()
        };

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateCompleteInstructionAndReturn(StatementGeneratorContext context)
    {
        var instructionStep = context.RequiredInstructionStep;

        if (instructionStep.ExitOverlapStep != null)
        {
            foreach (var statement in StatementGenerator.GenerateOverlapStatements(context.FileContext, instructionStep.ExitOverlapStep, context.GeneratorContext.GetImplicitInstructionCompleteStatementCount(instructionStep.ExitOverlapStep)))
            {
                yield return statement;
            }
        }

        yield return ReturnStatement(
            InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(Parameter.Name.Emulator),
                        IdentifierName(Method.Name.CompleteInstruction)))
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

    [Pure]
    private static StatementSyntax CreateHandleInterruptsStatement() =>
        ExpressionStatement(
            InvocationExpression(IdentifierName(Method.Name.HandleInterrupts))
                .WithArgumentList(ArgumentList([CreateEmulatorArgument()])));

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateStepInstructionComplete(StatementGeneratorContext context)
    {
        foreach (var statement in context.GeneratorContext.OnInstructionComplete.SelectMany(s => StatementStatementEmitter.Generate(context, s)))
        {
            yield return statement;
        }

        yield return StatementTransitionEmitter.CreateSetStep(context.GeneratorContext.OpcodeRead.FirstStep).WithLeadingTrivia(Comment("// Finish instruction."));
    }

    [Pure]
    private static IfStatementSyntax GenerateInstructionStepHandleInterruptsAndReturnIfHandled(StatementGeneratorContext context)
    {
        var instructionStep = context.RequiredInstructionStep;
        if (context.Step?.Sequence is not Instruction)
        {
            return IfStatement(
                InvocationExpression(IdentifierName(Method.Name.HandleInterrupts))
                    .WithArgumentList(ArgumentList([CreateEmulatorArgument()])),
                Block(SingletonList<StatementSyntax>(
                    ReturnStatement(GenerateNumericLiteralExpression(instructionStep.TStatesBeforeStep + 1)))));
        }

        return IfStatement(
            InvocationExpression(IdentifierName(Method.Name.HandleInterrupts))
                .WithArgumentList(ArgumentList([CreateEmulatorArgument()])),
            Block(GenerateCompleteInstructionAndReturn(context)));
    }

    [Pure]
    private static IfStatementSyntax GenerateStepHandleInterruptsAndReturnIfHandled() =>
        IfStatement(
            InvocationExpression(IdentifierName(Method.Name.HandleInterrupts))
                .WithArgumentList(ArgumentList(
                [
                    CreateEmulatorArgument(),
                    Argument(RefExpression(IdentifierName(Parameter.Name.ActionRequired)))
                ])),
            Block(ReturnStatement()));
}