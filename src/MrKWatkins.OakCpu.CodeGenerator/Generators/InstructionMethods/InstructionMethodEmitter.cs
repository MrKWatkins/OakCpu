using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Parameter = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Parameter;
using Field = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Field;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.InstructionMethods;

internal static class InstructionMethodEmitter
{
    private const ushort NoNextInstructionValue = ushort.MaxValue;

    [Pure]
    internal static MemberDeclarationSyntax CreateMethod(GeneratorContext context, InstructionMethodPlan plan)
    {
        var statements = plan.Steps.Count == 0
            ? CreateEmptyBodyStatements(context, plan)
            : CreateStepBodyStatements(context, plan);

        return MethodDeclaration(IntType, Identifier(plan.MethodName))
            .WithModifiers([Private, Static])
            .WithTypeParameterList(InstructionHandlerSyntax.TypeParameters)
            .WithParameterList(
                ParameterList(
                [
                    Parameter.Syntax.InstructionEmulator(context),
                    InstructionHandlerSyntax.MethodParameter
                ]))
            .WithConstraintClauses(InstructionHandlerSyntax.ConstraintClauses(context))
            .WithLeadingTrivia(Comment($"// {plan.Comment}"))
            .WithBody(Block(statements));
    }

    [Pure]
    private static IReadOnlyList<StatementSyntax> CreateEmptyBodyStatements(GeneratorContext context, InstructionMethodPlan plan)
    {
        var statements = new List<StatementSyntax>();
        statements.AddRange(plan.OverlapStatements);

        if (plan.DeferredNextSequence != null)
        {
            statements.Add(CreateSetNextSequence(context, plan.DeferredNextSequence));
        }

        statements.Add(plan.CompletesInstructionImplicitly
            ? CreateCompleteInstructionReturnStatement(plan.Sequence, 0)
            : CreateInstructionTStatesReturnStatement(0));

        return statements;
    }

    [Pure]
    private static IReadOnlyList<StatementSyntax> CreateStepBodyStatements(GeneratorContext context, InstructionMethodPlan plan)
    {
        var statements = new List<StatementSyntax>();
        var terminated = false;

        foreach (var (stepPlan, index) in plan.Steps.Select((stepPlan, index) => (stepPlan, index)))
        {
            if (stepPlan.NextInstructionVariableName != null)
            {
                // Seed the next-instruction variable with the 65535 "no redirect" sentinel. A step that
                // unconditionally redirects always overwrites this before it is read, but the seed - together with the
                // always-true guard emitted by CreateExecuteNextInstructionAndReturn - must stay. See the detailed note
                // on that method: the guard is a JIT code-generation boundary worth ~2.5x on the hot dispatch path, so
                // this is deliberately NOT collapsed to an unconditional assignment even when the redirect is certain.
                statements.Add(
                    InitializeVariableStatement(
                        stepPlan.NextInstructionVariableName,
                        GenerateNumericLiteralExpression(NoNextInstructionValue),
                        UShortType));
            }

            if (stepPlan.StepStatements.Count != 0)
            {
                var emitActionBeforeStatements = stepPlan.Action != Action.None && stepPlan.StepStatements[^1] is ReturnStatementSyntax;
                if (emitActionBeforeStatements)
                {
                    statements.Add(CreateActionCallbackStatement(stepPlan.Action));
                }

                if (stepPlan.RequiresBlock)
                {
                    statements.Add(Block(stepPlan.StepStatements));
                }
                else
                {
                    statements.AddRange(stepPlan.StepStatements);
                }
            }

            if (stepPlan.RollsBackOpcodeRead)
            {
                statements.AddRange(CreateRollbackOpcodeReadAndReturnStatements(index));
                terminated = true;
                break;
            }

            if (stepPlan.Action != Action.None && stepPlan.StepStatements.LastOrDefault() is not ReturnStatementSyntax)
            {
                statements.Add(CreateActionCallbackStatement(stepPlan.Action));
            }

            if (stepPlan.StepStatements.LastOrDefault() is ReturnStatementSyntax)
            {
                terminated = true;
                break;
            }

            if (stepPlan.NextInstructionVariableName != null)
            {
                statements.Add(CreateExecuteNextInstructionAndReturn(stepPlan.NextInstructionVariableName, index + 1));
            }
        }

        if (!terminated)
        {
            statements.AddRange(plan.OverlapStatements);
            if (plan.DeferredNextSequence != null)
            {
                statements.Add(CreateSetNextSequence(context, plan.DeferredNextSequence));
            }

            statements.Add(plan.CompletesInstructionImplicitly
                ? CreateCompleteInstructionReturnStatement(plan.Sequence, plan.Steps.Count)
                : CreateInstructionTStatesReturnStatement(plan.Steps.Count));
        }

        return statements;
    }

    [Pure]
    private static StatementSyntax CreateActionCallbackStatement(Action action) =>
        ExpressionStatement(
            InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(InstructionHandlerSyntax.ParameterName),
                        IdentifierName(InstructionHandlerSyntax.OnActionRequiredMethodName)))
                .WithArgumentList(
                    ArgumentList(
                    [
                        Argument(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(TypeName.ActionRequiredEnum),
                                IdentifierName(action.EnumName))),
                        Argument(EmulatorMemberIdentifier(PreDefinedDataMember.Address.FieldName)),
                        Argument(EmulatorMemberIdentifier(PreDefinedDataMember.Data.FieldName))
                    ])));

    [Pure]
    private static StatementSyntax CreateSetNextSequence(GeneratorContext context, StepSequence sequence) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(Parameter.Name.Emulator),
                    IdentifierName(Field.Name.NextSequenceStep)),
                GenerateNumericLiteralExpression(context.GetInstructionEmulatorSequenceIndex(sequence))));

    [Pure]
    private static IEnumerable<StatementSyntax> CreateRollbackOpcodeReadAndReturnStatements(int tStates)
    {
        yield return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SubtractAssignmentExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(Parameter.Name.Emulator),
                    IdentifierName("PC")),
                GenerateNumericLiteralExpression(0x01)));
        yield return ReturnStatement(
            GenerateNumericLiteralExpression(tStates));
    }

    // Emits the next-instruction dispatch as a guarded return:
    //
    //     if (nextInstructionN != 65535)
    //     {
    //         return tStates + emulator.ExecuteDecodedInstruction(nextInstructionN, ref handler);
    //     }
    //     // ...falls through to the step's normal "return tStates" / CompleteInstruction(...) tail.
    //
    // For a step that unconditionally redirects (move_to_opcode / move_to_sequence / move_to_interrupt_mode) the
    // variable is always reassigned away from the 65535 sentinel before we reach here, so the guard is provably always
    // true and the trailing return is unreachable. It therefore looks like obviously removable dead code - DO NOT
    // remove it, and do not special-case unconditional redirects to emit the dispatch without the guard.
    //
    // The always-true branch is load-bearing for performance. ExecuteDecodedInstruction is [AggressiveInlining] and
    // dispatches through the function-pointer table into the recursive instruction chain; the guard acts as a
    // code-generation boundary that the JIT needs to optimise the hot opcode-read path well. Dropping it (so the
    // dispatch becomes the method's unconditional tail) regressed the Z80 instruction emulator from ~x338 to ~x128
    // real-Spectrum speed in the ZEXALL "aluop a,nn" benchmark - a ~2.5x slowdown - measured 2026-06-26. The branch is
    // free at runtime (predicted not-taken, never mispredicts) and pays for itself many times over, so keep it.
    [Pure]
    private static StatementSyntax CreateExecuteNextInstructionAndReturn(string nextInstructionVariableName, int tStates) =>
        IfStatement(
            BinaryExpression(
                SyntaxKind.NotEqualsExpression,
                IdentifierName(nextInstructionVariableName),
                GenerateNumericLiteralExpression(NoNextInstructionValue)),
            Block(
                ReturnStatement(
                    BinaryExpression(
                        SyntaxKind.AddExpression,
                        GenerateNumericLiteralExpression(tStates),
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(Parameter.Name.Emulator),
                                    IdentifierName(InstructionEmulatorGenerator.ExecuteDecodedInstructionMethodName)))
                            .WithArgumentList(
                                ArgumentList(
                                [
                                    Argument(IdentifierName(nextInstructionVariableName)),
                                    InstructionHandlerSyntax.Argument
                                ]))))));

    [Pure]
    private static ReturnStatementSyntax CreateInstructionTStatesReturnStatement(int tStates) =>
        ReturnStatement(GenerateNumericLiteralExpression(tStates));

    [Pure]
    private static ReturnStatementSyntax CreateCompleteInstructionReturnStatement(StepSequence sequence, int tStates)
    {
        if (sequence is not Instruction instruction)
        {
            throw new InvalidOperationException("Only instructions can use the instruction-complete helper.");
        }

        return ReturnStatement(
            InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(Parameter.Name.Emulator),
                        IdentifierName(Method.Name.CompleteInstruction)))
                .WithArgumentList(
                    ArgumentList(
                    [
                        Argument(LiteralExpression(instruction.UpdatesFlags ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression)),
                        Argument(GenerateNumericLiteralExpression(tStates))
                    ])));
    }

}