using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

[SuppressMessage("Maintainability", "CA1506:Avoid excessive class coupling", Justification = "Generator code necessarily composes many Roslyn syntax node types.")]
public sealed class InstructionEmulatorGenerator : TypeGenerator
{
    internal const string ExecuteDecodedInstructionMethodName = "ExecuteDecodedInstruction";
    internal const string OpcodeReadStep0FieldName = "OpcodeReadStep0";
    private const string NextInstructionVariableNamePrefix = "nextInstruction";
    private const ushort NoNextInstructionValue = ushort.MaxValue;
    public static readonly InstructionEmulatorGenerator Instance = new();

    private InstructionEmulatorGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{GetInstructionEmulatorClassName(context)}.instructions";

    protected override BaseTypeDeclarationSyntax CreateType(GeneratorContext context) =>
        PopulateClass(
            context,
            ClassDeclaration(GetInstructionEmulatorClassName(context)).AddModifiers(Public, Sealed, Unsafe, Partial));

    [Pure]
    private static ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration)
    {
        var members = new List<MemberDeclarationSyntax>
        {
            CreateDispatchConstants(context),
            CreateErrorMethod(context)
        };

        members.AddRange(GetDispatchableSequences(context)
            .Select(sequence => (Sequence: sequence, Steps: GetRegularSteps(sequence)))
            .Select(x => CreateInstructionMethod(context, x.Sequence, x.Steps)));

        return classDeclaration.AddMembers(members.ToArray());
    }

    [Pure]
    internal static IEnumerable<StepSequence> GetDispatchableSequences(GeneratorContext context) => context.InstructionEmulatorSequences;

    [Pure]
    internal static string GetInstructionMethodName(GeneratorContext context, StepSequence sequence)
    {
        var baseName = GetInstructionMethodBaseName(sequence);
        var collisions = GetDispatchableSequences(context)
            .Where(other => string.Equals(GetInstructionMethodBaseName(other), baseName, StringComparison.Ordinal))
            .ToList();
        if (collisions.Count == 1)
        {
            return baseName;
        }

        var canonical = collisions
            .OrderBy(GetInstructionMethodCollisionRank)
            .ThenBy(context.GetInstructionEmulatorSequenceIndex)
            .First();

        return ReferenceEquals(canonical, sequence)
            ? baseName
            : $"{baseName}_{GetInstructionMethodEncodingSuffix(sequence)}";
    }

    [Pure]
    private static int GetInstructionMethodCollisionRank(StepSequence sequence) =>
        sequence switch
        {
            Instruction { Prefix: null, OpcodeTable: null } => 0,
            Instruction { Prefix: { }, OpcodeTable: null } => 1,
            Instruction { Prefix: null, OpcodeTable: { } } => 2,
            Instruction { Prefix: { }, OpcodeTable: { } } => 3,
            PrefixJump => 4,
            _ => 5
        };

    [Pure]
    private static string GetInstructionMethodBaseName(StepSequence sequence) =>
        sequence switch
        {
            Instruction instruction => SanitizeIdentifier(instruction.Mnemonic),
            PrefixJump prefixJump => $"Prefix_{prefixJump.Prefix:X2}",
            NamedStepSequence namedSequence => SanitizeIdentifier(GetNamedSequenceDisplayName(namedSequence.Name ?? namedSequence.FirstStep.Name)),
            _ => SanitizeIdentifier(sequence.Name ?? sequence.FirstStep.Name)
        };

    [Pure]
    private static string GetInstructionMethodEncodingSuffix(StepSequence sequence) =>
        sequence switch
        {
            Instruction { OpcodeTable: { } opcodeTable, Prefix: { } prefix, Opcode: var opcode } => $"{SanitizeIdentifier(opcodeTable)}_{prefix:X2}_{opcode:X2}",
            Instruction { Prefix: { } prefix, Opcode: var opcode } => $"{prefix:X2}_{opcode:X2}",
            Instruction { OpcodeTable: { } opcodeTable, Opcode: var opcode } => $"{SanitizeIdentifier(opcodeTable)}_{opcode:X2}",
            Instruction { Opcode: var opcode } => $"{opcode:X2}",
            PrefixJump { Prefix: var prefix } => $"{prefix:X2}",
            NamedStepSequence namedSequence => SanitizeIdentifier(GetNamedSequenceDisplayName(namedSequence.Name ?? namedSequence.FirstStep.Name)),
            _ => SanitizeIdentifier(sequence.Name ?? sequence.FirstStep.Name)
        };

    [Pure]
    private static string GetInstructionMethodComment(StepSequence sequence) =>
        sequence switch
        {
            Instruction instruction => instruction.Mnemonic,
            PrefixJump prefixJump => $"Read opcode after prefix 0x{prefixJump.Prefix:X2}",
            NamedStepSequence namedSequence => GetNamedSequenceDisplayName(namedSequence.Name ?? namedSequence.FirstStep.Name),
            _ => sequence.Name ?? sequence.FirstStep.Name
        };

    [Pure]
    private static string GetNamedSequenceDisplayName(string name) =>
        name switch
        {
            "opcode_read" => "Opcode read",
            "halted" => "Halt cycle",
            "halted_cycle" => "Halt cycle",
            _ when name.StartsWith("interrupt_mode_", StringComparison.Ordinal) &&
                   byte.TryParse(name["interrupt_mode_".Length..], out var mode)
                => $"Interrupt Mode {mode}",
            _ => name.Replace('_', ' ')
        };

    [Pure]
    private static string SanitizeIdentifier(string value)
    {
        var builder = new StringBuilder();
        var previousUnderscore = false;

        foreach (var c in value)
        {
            var replacement = c switch
            {
                '\'' => "_shadow",
                '+' => "_plus_",
                ' ' or ',' or '-' or '/' or '.' => "_",
                '(' or ')' => "",
                _ when char.IsLetterOrDigit(c) => c.ToString(),
                _ => "_"
            };

            foreach (var replacementCharacter in replacement)
            {
                if (replacementCharacter == '_')
                {
                    if (previousUnderscore)
                    {
                        continue;
                    }

                    previousUnderscore = true;
                    builder.Append(replacementCharacter);
                }
                else
                {
                    previousUnderscore = false;
                    builder.Append(replacementCharacter);
                }
            }
        }

        var result = builder.ToString().Trim('_');
        if (result.Length == 0)
        {
            return "Instruction";
        }

        return char.IsDigit(result[0]) ? $"_{result}" : result;
    }

    [Pure]
    private static IReadOnlyList<Step> GetRegularSteps(StepSequence sequence) => sequence.Steps.Where(step => !step.ExecutesAsOverlapOnly).ToList();

    [Pure]
    private static bool ContainsCall(IEnumerable<Statement> statements, PreDefinedFunction function) => statements.Any(statement => ContainsCall(statement, function));

    [Pure]
    private static bool ContainsCall(AstNode node, PreDefinedFunction function)
    {
        if (node is Call call && call.Function == function)
        {
            return true;
        }

        return node.Children.Any(child => ContainsCall(child, function));
    }

    [Pure]
    private static MemberDeclarationSyntax CreateDispatchConstants(GeneratorContext context)
    {
        var opcodeReadStart = context.GetInstructionEmulatorSequenceIndex(context.OpcodeRead);

        return FieldDeclaration(
                VariableDeclaration(UShortType)
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(OpcodeReadStep0FieldName)
                                .WithInitializer(EqualsValueClause(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(opcodeReadStart)))))))
            .WithModifiers([Private, Token(SyntaxKind.ConstKeyword)]);
    }

    [Pure]
    private static MemberDeclarationSyntax CreateErrorMethod(GeneratorContext context)
    {
        context.RequiredUsings.Add(typeof(NotSupportedException).Namespace!);

        return MethodDeclaration(IntType, Identifier(ErrorMethodName))
            .WithModifiers([Private, Static])
            .WithParameterList(
                ParameterList(
                [
                    CreateInstructionEmulatorParameter(context),
                    CreateInstructionActionCallbackParameter()
                ]))
            .WithBody(
                Block(
                    ThrowStatement(
                        ObjectCreationExpression(IdentifierName(nameof(NotSupportedException)))
                            .WithArgumentList(
                                ArgumentList(
                                [
                                    Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("Opcode not supported")))
                                ])))));
    }

#pragma warning disable CA1506
    [Pure]
    private static MemberDeclarationSyntax CreateInstructionMethod(GeneratorContext context, StepSequence sequence, IReadOnlyList<Step> steps)
    {
        if (sequence is PrefixJump prefixJump)
        {
            return CreatePrefixJumpMethod(context, prefixJump, steps);
        }

        var overlapStep = sequence.Steps.FirstOrDefault(step => step.ExecutesAsOverlapOnly);
        var overlapTrailingStatementsToSkip = overlapStep == null ? 0 : context.GetImplicitInstructionCompleteStatementCount(overlapStep);
        var overlapStatements = overlapStep == null ? [] : StatementGenerator.GenerateOverlapStatements(context, overlapStep, overlapTrailingStatementsToSkip).ToArray();
        var statements = new List<StatementSyntax>();
        var terminated = false;
        var completesInstructionImplicitly = overlapStep != null
            ? overlapTrailingStatementsToSkip != 0
            : steps.Count != 0 && context.GetImplicitInstructionCompleteStatementCount(steps[^1]) != 0;
        var deferredNextSequence = GetDeferredNextSequence(context, sequence);

        var comments = new[] { Comment($"// {GetInstructionMethodComment(sequence)}") };

        if (steps.Count == 0)
        {
            if (overlapStep != null)
            {
                statements.AddRange(overlapStatements);
            }
            if (deferredNextSequence != null)
            {
                statements.Add(CreateSetNextSequence(context, deferredNextSequence));
            }

            statements.Add(completesInstructionImplicitly
                ? CreateCompleteInstructionReturnStatement(sequence, 0)
                : CreateInstructionTStatesReturnStatement(0));

            return MethodDeclaration(IntType, Identifier(GetInstructionMethodName(context, sequence)))
                .WithModifiers([Private, Static])
                .WithParameterList(
                    ParameterList(
                    [
                        CreateInstructionEmulatorParameter(context),
                        CreateInstructionActionCallbackParameter()
                    ]))
                .WithLeadingTrivia(comments)
                .WithBody(Block(statements));
        }

        var stepInfos = steps.Select((step, index) => CreateInstructionStepInfo(context, step, overlapStep, index)).ToList();
        var localDeclarationCounts = GetLocalDeclarationCounts(stepInfos);

        foreach (var (stepInfo, index) in stepInfos.Select((stepInfo, index) => (stepInfo, index)))
        {
            if (stepInfo.NextInstructionVariableName != null)
            {
                statements.Add(
                    InitializeVariableStatement(
                        stepInfo.NextInstructionVariableName,
                        GenerateNumericLiteralExpression(NoNextInstructionValue),
                        UShortType));
            }

            if (stepInfo.StepStatements.Count != 0)
            {
                if (RequiresStepBlock(stepInfo.StepStatements, localDeclarationCounts))
                {
                    statements.Add(Block(stepInfo.StepStatements));
                }
                else
                {
                    statements.AddRange(stepInfo.StepStatements);
                }
            }

            if (stepInfo.RollsBackOpcodeRead)
            {
                statements.AddRange(CreateRollbackOpcodeReadAndReturnStatements(index));
                terminated = true;
                break;
            }

            if (stepInfo.Action != Action.None)
            {
                statements.Add(CreateActionCallbackStatement(stepInfo.Action));
            }

            if (stepInfo.NextInstructionVariableName != null)
            {
                statements.Add(CreateExecuteNextInstructionAndReturn(stepInfo.NextInstructionVariableName, index + 1));
            }
        }

        if (!terminated)
        {
            statements.AddRange(overlapStatements);
            if (deferredNextSequence != null)
            {
                statements.Add(CreateSetNextSequence(context, deferredNextSequence));
            }
            statements.Add(completesInstructionImplicitly
                ? CreateCompleteInstructionReturnStatement(sequence, steps.Count)
                : CreateInstructionTStatesReturnStatement(steps.Count));
        }

        return MethodDeclaration(IntType, Identifier(GetInstructionMethodName(context, sequence)))
            .WithModifiers([Private, Static])
            .WithParameterList(
                ParameterList(
                [
                    CreateInstructionEmulatorParameter(context),
                    CreateInstructionActionCallbackParameter()
                ]))
            .WithLeadingTrivia(comments)
            .WithBody(Block(statements));
    }

    [Pure]
    private static StepSequence? GetDeferredNextSequence(GeneratorContext context, StepSequence sequence) =>
        sequence.NextOpcode switch
        {
            NextOpcodeMode.Loop => sequence,
            NextOpcodeMode.Overlapped when sequence.OverlappedSequenceName is { } overlappedSequenceName => context.GetSequence(overlappedSequenceName),
            _ => null
        };

    [Pure]
    private static MemberDeclarationSyntax CreatePrefixJumpMethod(GeneratorContext context, PrefixJump sequence, IReadOnlyList<Step> steps)
    {
        var comments = new[] { Comment($"// {GetInstructionMethodComment(sequence)}") };
        var statements = new List<StatementSyntax>();

        foreach (var step in steps)
        {
            var stepStatements = StatementGenerator.GenerateInstructionStatements(context, step, null, null, 0, 0).ToList();
            if (stepStatements.Count != 0)
            {
                statements.AddRange(stepStatements);
            }
        }

        statements.Add(
            ReturnStatement(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(EmulatorParameterName),
                            IdentifierName(ExecuteDecodedInstructionMethodName)))
                    .WithArgumentList(
                        ArgumentList(
                        [
                            Argument(IdentifierName(OpcodeReadStep0FieldName)),
                            Argument(IdentifierName(InstructionActionCallbackParameterName))
                        ]))));

        return MethodDeclaration(IntType, Identifier(GetInstructionMethodName(context, sequence)))
            .WithModifiers([Private, Static])
            .WithParameterList(
                ParameterList(
                [
                    CreateInstructionEmulatorParameter(context),
                    CreateInstructionActionCallbackParameter()
                ]))
            .WithLeadingTrivia(comments)
            .WithBody(Block(statements));
    }

    [Pure]
    private static bool ContainsRedirectCall(Step step) =>
        ContainsCall(step.Statements, PreDefinedFunction.MoveToInterruptMode) ||
        ContainsCall(step.Statements, PreDefinedFunction.MoveToOpcode) ||
        ContainsCall(step.Statements, PreDefinedFunction.MoveToSequence) ||
        ContainsCall(step.Statements, PreDefinedFunction.MoveToSequenceGroup);

    [Pure]
    private static bool ShouldRollbackOpcodeRead(Step step, Action action) =>
        action.Name == "opcode_read" && ContainsCurrentStepAssignment(step.Statements, 1);

    [Pure]
    private static bool ContainsCurrentStepAssignment(IEnumerable<Statement> statements, int value) =>
        statements.Any(statement => ContainsCurrentStepAssignment(statement, value));

    [Pure]
    private static bool ContainsCurrentStepAssignment(AstNode node, int value) =>
        node is Assignment
        {
            Target: DataMemberAccess { DataMember: var dataMember },
            Value: Number { Value: var assignmentValue }
        } && dataMember == PreDefinedDataMember.CurrentStep && assignmentValue == value ||
        node.Children.Any(child => ContainsCurrentStepAssignment(child, value));

    [Pure]
    private static InstructionStepInfo CreateInstructionStepInfo(GeneratorContext context, Step step, Step? instructionExitOverlapStep, int instructionTStatesBeforeStep)
    {
        var action = StepMetadata.GetAction(context, step);
        var containsRedirect = ContainsRedirectCall(step);
        var rollsBackOpcodeRead = ShouldRollbackOpcodeRead(step, action);
        var nextInstructionVariableName = containsRedirect ? $"{NextInstructionVariableNamePrefix}{step.Index}" : null;
        var trailingStatementsToSkip = context.GetImplicitInstructionCompleteStatementCount(step);
        var requiresBody = !step.DoesNothing || step.QueuesOverlapStep || containsRedirect || ContainsCall(step.Statements, PreDefinedFunction.HandleInterrupts) || ContainsCall(step.Statements, PreDefinedFunction.InstructionComplete);
        var stepStatements = requiresBody
            ? StatementGenerator.GenerateInstructionStatements(context, step, nextInstructionVariableName, instructionExitOverlapStep, instructionTStatesBeforeStep, trailingStatementsToSkip).ToList()
            : [];

        return new InstructionStepInfo(action, rollsBackOpcodeRead, nextInstructionVariableName, stepStatements);
    }

    [Pure]
    private static IReadOnlyDictionary<string, int> GetLocalDeclarationCounts(IEnumerable<InstructionStepInfo> stepInfos)
    {
        var declarationNames = stepInfos
            .SelectMany(stepInfo => stepInfo.StepStatements)
            .SelectMany(statement => statement.DescendantNodesAndSelf().OfType<LocalDeclarationStatementSyntax>())
            .SelectMany(statement => statement.Declaration.Variables.Select(variable => variable.Identifier.ValueText));

        return declarationNames
            .GroupBy(name => name)
            .ToDictionary(group => group.Key, group => group.Count());
    }

    [Pure]
    private static bool RequiresStepBlock(IEnumerable<StatementSyntax> stepStatements, IReadOnlyDictionary<string, int> localDeclarationCounts) =>
        stepStatements
            .SelectMany(statement => statement.DescendantNodesAndSelf().OfType<LocalDeclarationStatementSyntax>())
            .SelectMany(statement => statement.Declaration.Variables.Select(variable => variable.Identifier.ValueText))
            .Any(name => localDeclarationCounts.TryGetValue(name, out var count) && count > 1);

    [Pure]
    private static StatementSyntax CreateActionCallbackStatement(Action action) =>
        ExpressionStatement(
            InvocationExpression(IdentifierName(InstructionActionCallbackParameterName))
                .WithArgumentList(
                    ArgumentList(
                    [
                        Argument(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(ActionRequiredEnumName),
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
                    IdentifierName(EmulatorParameterName),
                    IdentifierName(NextSequenceStepFieldName)),
                GenerateNumericLiteralExpression(context.GetInstructionEmulatorSequenceIndex(sequence))));

    [Pure]
    private static IEnumerable<StatementSyntax> CreateRollbackOpcodeReadAndReturnStatements(int tStates)
    {
        yield return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SubtractAssignmentExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(EmulatorParameterName),
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
                                    IdentifierName(EmulatorParameterName),
                                    IdentifierName(ExecuteDecodedInstructionMethodName)))
                            .WithArgumentList(
                                ArgumentList(
                                [
                                    Argument(IdentifierName(nextInstructionVariableName)),
                                    Argument(IdentifierName(InstructionActionCallbackParameterName))
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
                        IdentifierName(EmulatorParameterName),
                        IdentifierName(CompleteInstructionMethodName)))
                .WithArgumentList(
                    ArgumentList(
                    [
                        Argument(LiteralExpression(instruction.UpdatesFlags ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression)),
                        Argument(GenerateNumericLiteralExpression(tStates))
                    ])));
    }

    private sealed record InstructionStepInfo(
        Action Action,
        bool RollsBackOpcodeRead,
        string? NextInstructionVariableName,
        IReadOnlyList<StatementSyntax> StepStatements);

#pragma warning restore CA1506
}