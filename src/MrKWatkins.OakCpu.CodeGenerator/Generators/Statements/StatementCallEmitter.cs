using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags;
using MrKWatkins.OakCpu.Definitions.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Action = MrKWatkins.OakCpu.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Statements;

internal static class StatementCallEmitter
{
    private static readonly IReadOnlyDictionary<PreDefinedFunction, Func<StatementGeneratorContext, Call, IEnumerable<StatementSyntax>>> emitters =
        new Dictionary<PreDefinedFunction, Func<StatementGeneratorContext, Call, IEnumerable<StatementSyntax>>>
        {
            { PreDefinedFunction.Flags, static (context, _) => FlagsGenerator.GenerateFlagsStatements(context) },
            { PreDefinedFunction.InstructionComplete, static (context, call) => StatementBoundaryEmitter.GenerateInstructionComplete(context, call) },
            { PreDefinedFunction.Handled, static (context, _) => StatementBoundaryEmitter.GenerateHandled(context) },
            { PreDefinedFunction.HandleInterrupts, static (context, _) => StatementBoundaryEmitter.GenerateHandleInterrupts(context) },
            { PreDefinedFunction.MoveToInterruptMode, static (context, call) => StatementTransitionEmitter.GenerateMoveToInterruptMode(context, call) },
            { PreDefinedFunction.MoveToOpcode, static (context, _) => StatementTransitionEmitter.GenerateMoveToOpcode(context) },
            { PreDefinedFunction.MoveToSequenceGroup, static (context, call) => StatementTransitionEmitter.GenerateMoveToSequenceGroup(context, call) },
            { PreDefinedFunction.MoveToSequence, static (context, call) => StatementTransitionEmitter.GenerateMoveToSequence(context, call) },
            { PreDefinedFunction.Request, static (context, call) => GenerateRequest(context, (call.Arguments.FirstOrDefault() as ActionAccess)?.Action ?? throw new InvalidOperationException("The request function must have an action as the first argument.")) },
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

    /// <summary>
    /// Generates a request that is not the step's specified action, i.e. one inside an if statement, so the action a machine cycle
    /// performs depends on the state of the CPU. The Z80N block transfers need this; they only write the byte they have read if it
    /// does not match A, and read from the destination instead if it does.
    /// </summary>
    [Pure]
    private static IEnumerable<StatementSyntax> GenerateRequest(StatementGeneratorContext context, Action action)
    {
        var actionRequired =
            MemberAccessExpression(
                Microsoft.CodeAnalysis.CSharp.SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(TypeName.ActionRequiredEnum),
                IdentifierName(action.EnumName));

        if (context.Mode.IsInstructionEmulatorMode)
        {
            // The instruction emulator calls the bus handler directly at the point the action is performed.
            yield return
                ExpressionStatement(
                    InvocationExpression(
                            MemberAccessExpression(
                                Microsoft.CodeAnalysis.CSharp.SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(InstructionHandlerSyntax.ParameterName),
                                IdentifierName(InstructionHandlerSyntax.OnActionRequiredMethodName)))
                        .WithArgumentList(
                            ArgumentList(
                            [
                                Argument(actionRequired),
                                Argument(EmulatorMemberIdentifier(PreDefinedDataMember.Address.FieldName)),
                                Argument(EmulatorMemberIdentifier(PreDefinedDataMember.Data.FieldName))
                            ])));
            yield break;
        }

        // The step emulator returns the action to the host from the step it is requested in. The step's handler takes the action by
        // reference so that it can replace the one the step table holds.
        yield return
            ExpressionStatement(
                AssignmentExpression(
                    Microsoft.CodeAnalysis.CSharp.SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName(Identifiers.Parameter.Name.ActionRequired),
                    actionRequired));
    }
}