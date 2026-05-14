using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Statements;

internal static class StatementCallEmitter
{
    private static readonly IReadOnlyDictionary<PreDefinedFunction, Func<StatementGeneratorContext, Call, IEnumerable<StatementSyntax>>> emitters =
        new Dictionary<PreDefinedFunction, Func<StatementGeneratorContext, Call, IEnumerable<StatementSyntax>>>
        {
            { PreDefinedFunction.Flags, static (context, _) => FlagsGenerator.GenerateFlagsStatements(context) },
            { PreDefinedFunction.InstructionComplete, static (context, _) => StatementBoundaryEmitter.GenerateInstructionComplete(context) },
            { PreDefinedFunction.Handled, static (context, _) => StatementBoundaryEmitter.GenerateHandled(context) },
            { PreDefinedFunction.HandleInterrupts, static (context, _) => StatementBoundaryEmitter.GenerateHandleInterrupts(context) },
            { PreDefinedFunction.MoveToInterruptMode, static (context, call) => StatementTransitionEmitter.GenerateMoveToInterruptMode(context, call) },
            { PreDefinedFunction.MoveToOpcode, static (context, _) => StatementTransitionEmitter.GenerateMoveToOpcode(context) },
            { PreDefinedFunction.MoveToSequenceGroup, static (context, call) => StatementTransitionEmitter.GenerateMoveToSequenceGroup(context, call) },
            { PreDefinedFunction.MoveToSequence, static (context, call) => StatementTransitionEmitter.GenerateMoveToSequence(context, call) },
            { PreDefinedFunction.Request, static (_, call) => GenerateRequest((call.Arguments.FirstOrDefault() as ActionAccess)?.Action ?? throw new InvalidOperationException("The request function must have an action as the first argument.")) },
            { PreDefinedFunction.SetOpcodeStepTable, static (context, call) => StatementTransitionEmitter.GenerateSetOpcodeStepTable(context, call) }
        };

    [Pure]
    internal static IEnumerable<StatementSyntax> GenerateCall(StatementGeneratorContext context, CallStatement callStatement)
    {
        if (context.Mode.SkipHandleInterruptsCall && callStatement.Call.Function == PreDefinedFunction.HandleInterrupts)
        {
            return [];
        }

        if (callStatement.Call.Function is not PreDefinedFunction function || !emitters.TryGetValue(function, out var emitter))
        {
            throw new NotSupportedException($"The function {callStatement.Call.Function} is not supported.");
        }

        return emitter(context, callStatement.Call);
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateRequest(Action action)
    {
        yield return
            ReturnStatement(
                MemberAccessExpression(
                    Microsoft.CodeAnalysis.CSharp.SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(TypeName.ActionRequiredEnum),
                    IdentifierName(action.EnumName)));
    }
}