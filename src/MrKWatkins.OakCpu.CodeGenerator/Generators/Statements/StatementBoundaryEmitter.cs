using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Parameter = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Parameter;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Statements;

internal static class StatementBoundaryEmitter
{
    [Pure]
    internal static IEnumerable<StatementSyntax> GenerateHandled(StatementGeneratorContext context)
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
    internal static IEnumerable<StatementSyntax> GenerateHandleInterrupts(StatementGeneratorContext context) =>
        context.Mode is StatementGenerationMode.InstructionCompletionMode
            ? [CreateHandleInterruptsStatement()]
            : [GenerateHandleInterruptsAndReturnIfHandled(context)];

    [Pure]
    internal static IEnumerable<StatementSyntax> GenerateInstructionComplete(StatementGeneratorContext context) =>
        context.Mode switch
        {
            StatementGenerationMode.InstructionStepMode => GenerateCompleteInstructionAndReturn(context),
            StatementGenerationMode.InstructionEmulatorMode => throw new InvalidOperationException("instruction_complete is only supported when generating instruction-emulator steps."),
            _ => GenerateStepInstructionComplete(context)
        };

    [Pure]
    internal static IEnumerable<StatementSyntax> GenerateBoundaryStatements(StatementGeneratorContext context)
    {
        if (context.Step is not { } step || !context.GeneratorContext.GetStepLayout(step).QueuesOverlapStep)
        {
            return [];
        }

        return context.Mode switch
        {
            StatementGenerationMode.InstructionEmulatorMode or StatementGenerationMode.InstructionStepMode =>
                GenerateInstructionCompleteStatements(context)
                    .Select((statement, index) => index == 0 ? statement.WithLeadingTrivia(Comment("// Check interrupts at the instruction boundary.")) : statement),
            _ =>
            [
                StatementTransitionEmitter.GenerateQueueOverlap(context.GeneratorContext, context.GeneratorContext.GetStepLayout(step).QueuedOverlapStep).WithLeadingTrivia(Comment("// Queue overlap step.")),
                ..GenerateInstructionCompleteStatements(context).Select((statement, index) => index == 0 ? statement.WithLeadingTrivia(Comment("// Check interrupts at the instruction boundary.")) : statement)
            ]
        };
    }

    [Pure]
    internal static IfStatementSyntax GenerateHandleInterruptsAndReturnIfHandled(StatementGeneratorContext context) =>
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
            foreach (var statement in StatementGenerator.GenerateOverlapStatements(context.FileContext, instructionStep.ExitOverlapStep, context.GeneratorContext.GetImplicitInstructionStepsCompleteStatementCount(instructionStep.ExitOverlapStep)))
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
        context.Step is { } step && context.GeneratorContext.GetStepLayout(step).Sequence is Instruction instruction
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
        if (context.Step is { } step && context.GeneratorContext.GetStepLayout(step).Sequence is Instruction)
        {
            foreach (var statement in context.GeneratorContext.OnInstructionStepsComplete.SelectMany(s => StatementStatementEmitter.Generate(context, s)))
            {
                yield return statement;
            }
        }

        foreach (var statement in GenerateInstructionCompleteStatements(context))
        {
            yield return statement;
        }

        yield return StatementTransitionEmitter.CreateSetStep(context.GeneratorContext, context.GeneratorContext.OpcodeRead.FirstStep).WithLeadingTrivia(Comment("// Finish instruction."));
    }

    [Pure]
    private static IfStatementSyntax GenerateInstructionStepHandleInterruptsAndReturnIfHandled(StatementGeneratorContext context)
    {
        var instructionStep = context.RequiredInstructionStep;
        if (context.Step is not { } step || context.GeneratorContext.GetStepLayout(step).Sequence is not Instruction)
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

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateInstructionCompleteStatements(StatementGeneratorContext context) =>
        context.GeneratorContext.OnInstructionComplete.SelectMany(s => StatementStatementEmitter.Generate(context, s));
}