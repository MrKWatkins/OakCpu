using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class InstructionEmulatorTablesGenerator : TypeGenerator
{
    public static readonly InstructionEmulatorTablesGenerator Instance = new();

    private InstructionEmulatorTablesGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{GetInstructionEmulatorClassName(context)}.tables";

    protected override BaseTypeDeclarationSyntax CreateType(GeneratorContext context)
    {
        var members = TableGeneration.CreateStepTableFields(context, GetSequenceGroupStepTableFieldName)
            .Prepend(CreateInstructionsField(context))
            .Append(CreateStaticConstructor(context))
            .ToArray();

        return ClassDeclaration(GetInstructionEmulatorClassName(context))
            .AddModifiers(Public, Sealed, Unsafe, Partial)
            .AddMembers(members);
    }

    [Pure]
    private static MemberDeclarationSyntax CreateInstructionsField(GeneratorContext context) =>
        FieldDeclaration(
                VariableDeclaration(
                        ArrayType(CreateInstructionHandlerType(context))
                            .WithRankSpecifiers([ArrayRankSpecifier([OmittedArraySizeExpression()])]))
                    .WithVariables([VariableDeclarator(Identifier(InstructionHandlersFieldName))]))
            .AddModifiers(Private, Static, ReadOnly);

    [Pure]
    private static ConstructorDeclarationSyntax CreateStaticConstructor(GeneratorContext context)
    {
        var statements = new List<StatementSyntax>
        {
            TableGeneration.CreateLittleEndianStatement(context),
            CreateInstructionsInitializationStatement(context)
        };

        statements.AddRange(TableGeneration.CreateTableInitializationStatements(context, CreateOpcodeStepTableInitializationStatement, CreateSequenceGroupStepTableInitializationStatement));

        return ConstructorDeclaration(GetInstructionEmulatorClassName(context))
            .WithModifiers(TokenList(Static))
            .WithBody(Block(statements));
    }

    [Pure]
    private static StatementSyntax CreateInstructionsInitializationStatement(GeneratorContext context)
    {
        var dispatchableSequences = InstructionEmulatorGenerator.GetDispatchableSequences(context).ToList();
        var handlers = Enumerable.Repeat<ExpressionSyntax>(PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(ErrorMethodName)), context.InstructionEmulatorDispatchCount).ToArray();

        foreach (var sequence in dispatchableSequences)
        {
            handlers[context.GetInstructionEmulatorSequenceIndex(sequence)] =
                PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(InstructionEmulatorGenerator.GetInstructionMethodName(context, sequence)));
        }

        return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(InstructionHandlersFieldName),
                CollectionExpression(SeparatedList<CollectionElementSyntax>(handlers.Select(ExpressionElement)))));
    }

    [Pure]
    private static StatementSyntax CreateOpcodeStepTableInitializationStatement(GeneratorContext context, OpcodeStepTable opcodeStepTable, IEnumerable<Instruction> instructions, IReadOnlyList<(byte Opcode, Step Step)> duplicates)
    {
        var stepIndices = Enumerable.Repeat(context.InstructionEmulatorErrorIndex, 256).ToArray();

        if (opcodeStepTable == OpcodeStepTable.NoPrefix)
        {
            foreach (var prefixJump in context.PrefixJumps.Values)
            {
                stepIndices[prefixJump.Prefix] = context.GetInstructionEmulatorSequenceIndex(prefixJump);
            }
        }
        else if (opcodeStepTable.Prefix is { } prefix && context.PrefixJumps.ContainsKey(prefix))
        {
            stepIndices[prefix] = context.GetInstructionEmulatorSequenceIndex(context.PrefixJumps[prefix]);
        }

        foreach (var instruction in instructions)
        {
            stepIndices[instruction.Opcode] = context.GetInstructionEmulatorSequenceIndex(instruction);
        }

        foreach (var duplicate in duplicates)
        {
            stepIndices[duplicate.Opcode] = context.GetInstructionEmulatorSequenceIndex(duplicate.Step.Sequence);
        }

        return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(opcodeStepTable.FieldName),
                CollectionExpression(SeparatedList<CollectionElementSyntax>(stepIndices.Select(index => ExpressionElement(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(index))))))));
    }

    [Pure]
    private static StatementSyntax CreateSequenceGroupStepTableInitializationStatement(GeneratorContext context, SequenceGroup sequenceGroup)
    {
        var stepIndices = Enumerable.Repeat(context.InstructionEmulatorErrorIndex, sequenceGroup.MaximumNumber + 1).ToArray();
        foreach (var member in sequenceGroup.Members)
        {
            stepIndices[member.Key] = context.GetInstructionEmulatorSequenceIndex(member.Value);
        }

        return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(GetSequenceGroupStepTableFieldName(sequenceGroup)),
                CollectionExpression(SeparatedList<CollectionElementSyntax>(stepIndices.Select(index => ExpressionElement(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(index))))))));
    }
}