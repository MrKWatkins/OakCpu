using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Field = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Field;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class InstructionEmulatorTablesGenerator : TypeGenerator
{
    public static readonly InstructionEmulatorTablesGenerator Instance = new();

    private InstructionEmulatorTablesGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{Class.Name.InstructionEmulator(context)}.tables";

    protected override BaseTypeDeclarationSyntax CreateType(FileGeneratorContext context)
    {
        var members = TableGeneration.CreateStepTableFields(context, Field.Name.SequenceGroupStepTable)
            .Prepend(CreateDispatchHolder(context))
            .Append(CreateStaticConstructor(context))
            .ToArray();

        return ClassDeclaration(Class.Name.InstructionEmulator(context))
            .AddModifiers(Public, Sealed, Unsafe, Partial)
            .AddMembers(members);
    }

    [Pure]
    private static MemberDeclarationSyntax CreateDispatchHolder(FileGeneratorContext context)
    {
        // The dispatch table holds function pointers to the generic instruction methods, so it must live inside a generic
        // holder class. Each closed THandler gets its own monomorphised table and instruction methods, letting the JIT
        // inline the handler's OnActionRequired call into every bus access.
        var instructionsField =
            FieldDeclaration(
                    VariableDeclaration(
                            ArrayType(InstructionHandlerSyntax.InstructionHandlerType(context.GeneratorContext))
                                .WithRankSpecifiers([ArrayRankSpecifier([OmittedArraySizeExpression()])]))
                        .WithVariables(
                        [
                            VariableDeclarator(Identifier(Field.Name.Instructions))
                                .WithInitializer(EqualsValueClause(CreateInstructionsCollectionExpression(context)))
                        ]))
                .AddModifiers(Public, Static, ReadOnly);

        return ClassDeclaration(InstructionHandlerSyntax.DispatchHolderName)
            .AddModifiers(Private, Static, Unsafe)
            .WithTypeParameterList(InstructionHandlerSyntax.TypeParameters)
            .WithConstraintClauses(InstructionHandlerSyntax.ConstraintClauses(context.GeneratorContext))
            .AddMembers(instructionsField);
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateStaticConstructor(FileGeneratorContext context)
    {
        var statements = new List<StatementSyntax>
        {
            TableGeneration.CreateLittleEndianStatement(context)
        };

        statements.AddRange(TableGeneration.CreateTableInitializationStatements(context, CreateOpcodeStepTableInitializationStatement, CreateSequenceGroupStepTableInitializationStatement));

        return ConstructorDeclaration(Class.Name.InstructionEmulator(context))
            .WithModifiers(TokenList(Static))
            .WithBody(Block(statements));
    }

    [Pure]
    private static ExpressionSyntax CreateInstructionsCollectionExpression(FileGeneratorContext context)
    {
        var dispatchableSequences = InstructionEmulatorGenerator.GetDispatchableSequences(context).ToList();
        var handlers = Enumerable.Repeat(InstructionHandlerSyntax.AddressOfInstructionMethod(Method.Name.Error), context.GeneratorContext.InstructionEmulatorDispatchCount).ToArray();

        foreach (var sequence in dispatchableSequences)
        {
            handlers[context.GeneratorContext.GetInstructionEmulatorSequenceIndex(sequence)] =
                InstructionHandlerSyntax.AddressOfInstructionMethod(InstructionEmulatorGenerator.GetInstructionMethodName(context, sequence));
        }

        return CollectionExpression(SeparatedList<CollectionElementSyntax>(handlers.Select(ExpressionElement)));
    }

    [Pure]
    private static StatementSyntax CreateOpcodeStepTableInitializationStatement(FileGeneratorContext context, OpcodeStepTable opcodeStepTable, IEnumerable<Instruction> instructions, IReadOnlyList<(byte Opcode, Step Step)> duplicates)
    {
        var stepIndices = Enumerable.Repeat(context.GeneratorContext.InstructionEmulatorErrorIndex, 256).ToArray();

        if (opcodeStepTable == OpcodeStepTable.NoPrefix)
        {
            foreach (var prefixJump in context.GeneratorContext.PrefixJumps.Values)
            {
                stepIndices[prefixJump.Prefix] = context.GeneratorContext.GetInstructionEmulatorSequenceIndex(prefixJump);
            }
        }
        else if (opcodeStepTable.Prefix is { } prefix && context.GeneratorContext.PrefixJumps.ContainsKey(prefix))
        {
            stepIndices[prefix] = context.GeneratorContext.GetInstructionEmulatorSequenceIndex(context.GeneratorContext.PrefixJumps[prefix]);
        }

        foreach (var instruction in instructions)
        {
            stepIndices[instruction.Opcode] = context.GeneratorContext.GetInstructionEmulatorSequenceIndex(instruction);
        }

        foreach (var duplicate in duplicates)
        {
            stepIndices[duplicate.Opcode] = context.GeneratorContext.GetInstructionEmulatorSequenceIndex(context.GeneratorContext.GetStepLayout(duplicate.Step).Sequence);
        }

        return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(opcodeStepTable.FieldName),
                CollectionExpression(SeparatedList<CollectionElementSyntax>(stepIndices.Select(index => ExpressionElement(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(index))))))));
    }

    [Pure]
    private static StatementSyntax CreateSequenceGroupStepTableInitializationStatement(FileGeneratorContext context, SequenceGroup sequenceGroup)
    {
        var stepIndices = Enumerable.Repeat(context.GeneratorContext.InstructionEmulatorErrorIndex, sequenceGroup.MaximumNumber + 1).ToArray();
        foreach (var member in sequenceGroup.Members)
        {
            stepIndices[member.Key] = context.GeneratorContext.GetInstructionEmulatorSequenceIndex(member.Value);
        }

        return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(Field.Name.SequenceGroupStepTable(sequenceGroup)),
                CollectionExpression(SeparatedList<CollectionElementSyntax>(stepIndices.Select(index => ExpressionElement(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(index))))))));
    }
}