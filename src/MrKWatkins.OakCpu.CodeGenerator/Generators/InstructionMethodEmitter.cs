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

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

internal static class InstructionMethodEmitter
{
    private const ushort NoNextInstructionValue = ushort.MaxValue;

    [Pure]
    public static MemberDeclarationSyntax CreateMethod(GeneratorContext context, InstructionMethodPlan plan)
    {
        var statements = plan.Steps.Count == 0
            ? CreateEmptyBodyStatements(context, plan)
            : CreateStepBodyStatements(context, plan);

        return MethodDeclaration(IntType, Identifier(plan.MethodName))
            .WithModifiers([Private, Static])
            .WithParameterList(
                ParameterList(
                [
                    Parameter.Syntax.InstructionEmulator(context),
                    Parameter.Syntax.InstructionActionCallback()
                ]))
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
                statements.Add(
                    InitializeVariableStatement(
                        stepPlan.NextInstructionVariableName,
                        GenerateNumericLiteralExpression(NoNextInstructionValue),
                        UShortType));
            }

            if (stepPlan.StepStatements.Count != 0)
            {
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

            if (stepPlan.Action != Action.None)
            {
                statements.Add(CreateActionCallbackStatement(stepPlan.Action));
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
            InvocationExpression(IdentifierName(Parameter.Name.InstructionActionCallback))
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
                                    Argument(IdentifierName(Parameter.Name.InstructionActionCallback))
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