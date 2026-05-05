using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.GeneratorSymbols;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

internal static class StatementCallEmitter
{
    [Pure]
    public static IEnumerable<StatementSyntax> GenerateCall(StatementGeneratorContext context, CallStatement callStatement)
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
            return StatementBoundaryEmitter.GenerateInstructionComplete(context);
        }
        if (callStatement.Call.Function == PreDefinedFunction.Handled)
        {
            return StatementBoundaryEmitter.GenerateHandled(context);
        }
        if (callStatement.Call.Function == PreDefinedFunction.HandleInterrupts)
        {
            return StatementBoundaryEmitter.GenerateHandleInterrupts(context);
        }
        if (callStatement.Call.Function == PreDefinedFunction.MoveToInterruptMode)
        {
            return StatementTransitionEmitter.GenerateMoveToInterruptMode(context, callStatement.Call);
        }
        if (callStatement.Call.Function == PreDefinedFunction.MoveToOpcode)
        {
            return StatementTransitionEmitter.GenerateMoveToOpcode(context);
        }
        if (callStatement.Call.Function == PreDefinedFunction.MoveToSequenceGroup)
        {
            return StatementTransitionEmitter.GenerateMoveToSequenceGroup(context, callStatement.Call);
        }
        if (callStatement.Call.Function == PreDefinedFunction.MoveToSequence)
        {
            return StatementTransitionEmitter.GenerateMoveToSequence(context, callStatement.Call);
        }
        if (callStatement.Call.Function == PreDefinedFunction.Request)
        {
            return GenerateRequest((callStatement.Call.Arguments.FirstOrDefault() as ActionAccess)?.Action ?? throw new InvalidOperationException("The request function must have an action as the first argument."));
        }
        if (callStatement.Call.Function == PreDefinedFunction.SetOpcodeStepTable)
        {
            return StatementTransitionEmitter.GenerateSetOpcodeStepTable(context, callStatement.Call);
        }

        throw new NotSupportedException($"The function {callStatement.Call.Function} is not supported.");
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateRequest(Action action)
    {
        yield return
            ReturnStatement(
                MemberAccessExpression(
                    Microsoft.CodeAnalysis.CSharp.SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(ActionRequiredEnumName),
                    IdentifierName(action.EnumName)));
    }
}