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
    private const string InstructionVariableName = "instruction";

    [Pure]
    internal static MemberDeclarationSyntax CreateMethod(FileGeneratorContext context, InstructionMethodPlan plan)
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
    private static IReadOnlyList<StatementSyntax> CreateStepBodyStatements(FileGeneratorContext context, InstructionMethodPlan plan)
    {
        var statements = new List<StatementSyntax>();
        var terminated = false;

        foreach (var (stepPlan, index) in plan.Steps.Select((stepPlan, index) => (stepPlan, index)))
        {
            if (stepPlan.NextInstructionVariableName != null)
            {
                // A move-to-opcode redirect dispatches on the just-read opcode via the swappable opcodeStepTable. When the
                // CPU has prefixes the no-prefix table is by far the most common, so peel it onto a single opcode ->
                // function-pointer load + call instead of the two-level opcodeStepTable -> step -> Instructions dispatch.
                if (stepPlan.IsMoveToOpcode && context.GeneratorContext.Configuration.OpcodeStepTables.HasPrefixes)
                {
                    statements.Add(CreateNoPrefixOpcodeDispatch(context, index + 1));
                }

                // Seed the next-instruction variable with the 65535 "no redirect" sentinel. A conditional redirect
                // leaves the variable at the sentinel when it does not fire, so the guard emitted by
                // CreateExecuteNextInstructionAndReturn skips the dispatch and falls through to the step's normal tail.
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
    // The guard exists because a redirect can be conditional: when it does not fire the variable keeps its 65535
    // sentinel, so the guard skips the dispatch and the step falls through to its normal tail. For a step that
    // unconditionally redirects (move_to_opcode / move_to_sequence / move_to_interrupt_mode) the variable is always
    // reassigned before we reach here, so the guard is always true - but it is emitted uniformly rather than
    // special-cased per redirect kind.
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

    // Emits the peeled fast path for a move-to-opcode redirect, taken when the active opcode table is the no-prefix table:
    //
    //     // Optimise for the common no-prefixed case.
    //     if (emulator.opcodeStepTable == OpcodeStepTableNoPrefix)
    //     {
    //         var instruction = (delegate*<{Emulator}, ref THandler, int>)Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(Dispatch<THandler>.NoPrefixInstructions), emulator.data);
    //         return tStates + instruction(emulator, ref handler);
    //     }
    //
    // This collapses the usual two dependent loads (opcodeStepTable[data] -> step, then Instructions[step] -> fnptr) into
    // a single opcode -> function-pointer load on the hot path; prefixed tables fall through to the original dispatch.
    [Pure]
    private static StatementSyntax CreateNoPrefixOpcodeDispatch(FileGeneratorContext context, int tStates)
    {
        var lookup =
            CastExpression(
                InstructionHandlerSyntax.InstructionHandlerType(context.GeneratorContext),
                CreateArrayGetWithoutBoundsCheck(
                    context.RequiredUsings,
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        InstructionHandlerSyntax.DispatchHolderType,
                        IdentifierName(InstructionEmulatorGenerator.NoPrefixInstructionsFieldName)),
                    EmulatorMemberIdentifier(PreDefinedDataMember.Data.FieldName)));

        var dispatch =
            ReturnStatement(
                BinaryExpression(
                    SyntaxKind.AddExpression,
                    GenerateNumericLiteralExpression(tStates),
                    InvocationExpression(IdentifierName(InstructionVariableName))
                        .WithArgumentList(
                            ArgumentList(
                            [
                                CreateEmulatorArgument(),
                                InstructionHandlerSyntax.Argument
                            ]))));

        return IfStatement(
                BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    EmulatorMemberIdentifier(PreDefinedDataMember.OpcodeStepTable.FieldName),
                    IdentifierName(context.GeneratorContext.Configuration.OpcodeStepTables.NoPrefix.FieldName)),
                Block(
                    InitializeVariableStatement(InstructionVariableName, lookup),
                    dispatch))
            .WithLeadingTrivia(Comment("// Optimise for the common no-prefixed case."));
    }

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